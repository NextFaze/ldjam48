using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;

public delegate void VoidCallback();

[RequireComponent(typeof(AudioSource))]
public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance { get { return _instance; } }

    public bool godMode = false;

    public Transform worldTransform;
    Vector3 lastSpawnPosition;
    Rigidbody2D playerRigidBody;
    public int DeathCount {
        get;
        private set;
    }

    private AudioSource audioSource;
    public AudioClip start;
    public AudioClip respawnClip;
    public AudioClip[] deathAudio;

    public DateTime gameStartTime = DateTime.Now;
    public DateTime gameEndTime = DateTime.MinValue;
    public TimeSpan gameRunTime {
        get {
            if(gameStartTime > gameEndTime) {
                return DateTime.Now - gameStartTime;
            }
            return gameEndTime - gameStartTime;
        }
    }

    Stack<Transform> levelTransforms = new Stack<Transform>();

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        StartGame();
    }

    public void StartGame() {
        var player = GameObject.FindWithTag("Player");

        if (player == null) {
            Debug.LogError("NO PLAYER!!");
            this.enabled = false;
        }

        lastSpawnPosition = player.transform.position;
        playerRigidBody = player.GetComponent<Rigidbody2D>();
        DeathCount = 0;

        gameStartTime = DateTime.Now;
        gameEndTime = DateTime.MinValue;
        levelTransforms.Clear();
        levelTransforms.Push(worldTransform);
    }

    private void Awake() {
        if(_instance == null) {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }    
        else {
            this.enabled = false;
        }
    }

    public void TeleportPlayer(Portal portal, bool directionIn) {

        var scaleFactor = portal.ScaleFactor;
        var portalPositionOffset = portal.PortalPositionOffset;

        if (directionIn)
        {
            levelTransforms.Push(portal.LevelTransform);
        }
        else
        {
            // At the top level, something is fishy
            if (levelTransforms.Count == 1) return;
            // Popping out of portal
            levelTransforms.Pop();
            scaleFactor = 1 / scaleFactor;
            portalPositionOffset *= -1;
        }

        // The top of the stack is which 'world' we are in
        var camTransform = levelTransforms.Peek();

        Debug.Log($"Scaling world by {scaleFactor}");
        worldTransform.localScale /= scaleFactor;

        RespawnPlayer(portal.transform.position + portalPositionOffset);

        var cam = FindObjectOfType<PortalCamera>();
        cam?.MoveCamera(camTransform.position);
    }

    public void KillPlayer() {
        if (godMode) return;

        var player = GameObject.FindWithTag("Player");
        if (!player.GetComponent<PlayerMovement>().enabled)
        {
            return;
        }
        player.GetComponent<PlayerMovement>().Disable();

        audioSource.PlayOneShot(deathAudio[UnityEngine.Random.Range(0, deathAudio.Length)], 2F);
        DeathCount++;
        
        StartCoroutine(ExecuteAfterTime(0.2f, () => audioSource.PlayOneShot(respawnClip, 0.7F)));
        StartCoroutine(ExecuteAfterTime(1.6f, RevivePlayer));
    }

    public void RevivePlayer () {
        RespawnPlayer(lastSpawnPosition);
        var player = GameObject.FindWithTag("Player");
        player.GetComponent<PlayerMovement>().Enable();
    }

    public void WinGame() {
        gameEndTime = DateTime.Now;
        
        Debug.Log($"Game Won in {gameRunTime.ToString(@"mm\:ss\:ff")} \\o/");

        SceneManager.LoadScene("end-game");
    }

    public void RespawnPlayer(Vector3 position) {
        // Replace with animated to new position
        lastSpawnPosition = position;
        playerRigidBody.isKinematic = true;
        playerRigidBody.transform.position = position;
        playerRigidBody.velocity = Vector3.zero;
        playerRigidBody.isKinematic = false;
    }

     IEnumerator ExecuteAfterTime(float time, VoidCallback callback)
    {
        yield return new WaitForSeconds(time);
        callback();
    }
}
