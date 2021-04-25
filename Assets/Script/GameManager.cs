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

    public Transform levelTransform;
    public Vector3 lastSpawnPosition;
    public Rigidbody2D playerRigidBody;
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

        playerRigidBody = player.GetComponent<Rigidbody2D>();
        DeathCount = 0;

        gameStartTime = DateTime.Now;
        gameEndTime = DateTime.MinValue;
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
    public void TeleportPlayer(float scaleFactor) {
        levelTransform.localScale /= scaleFactor;
    }

    public void KillPlayer() {
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
