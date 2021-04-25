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
    public bool easeCamera = false;

    public Transform worldTransform;
    Vector3 lastSpawnPosition;
    Rigidbody2D playerRigidBody;
    public int DeathCount {
        get;
        private set;
    }

    public GameObject cloudPrefab;

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
    public float transitionTime = 1f;

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

    private Portal lastPortal;
    private bool lastDirectionIn = false;
    public void TeleportPlayer(Portal portal, bool directionIn) {

        Debug.Log("Checking portal..");
        if (portal == lastPortal && directionIn == lastDirectionIn)
        {
            Debug.Log("Portal already triggered");
            return;
        }

        var scaleFactor = portal.ScaleFactor;
        var portalPositionOffset = portal.PortalPositionOffset;

        if (directionIn)
        {
            levelTransforms.Push(portal.LevelTransform);
        }
        else
        {
            // At the top level, something is fishy
            if (levelTransforms.Count == 1)
            {
                Debug.Log("At top level.. aborting");
                return;
            }

            // Popping out of portal
            levelTransforms.Pop();
            scaleFactor = 1 / scaleFactor;
            portalPositionOffset *= (-1);
        }

        lastPortal = portal;
        lastDirectionIn = directionIn;

        // The top of the stack is which 'world' we are in
        var camTransform = levelTransforms.Peek();

        StartCoroutine(AnimateWorldTransition(scaleFactor, camTransform.position, playerPos: portal.transform.position + portalPositionOffset));
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

    IEnumerator AnimateWorldTransition(float scaleFactor, Vector3 camPosition, Vector3 playerPos)
    {
        // Effect here?
        playerRigidBody.gameObject.SetActive(false);

        Debug.Log($"Scaling world by {scaleFactor}");
        var startScale = worldTransform.localScale;
        var endScale = worldTransform.localScale / scaleFactor;

        var cam = Camera.main.GetComponent<PortalCamera>();
        Vector3 camStartPos = cam.transform.position;
        var camEndPos = camPosition / scaleFactor;
        float elapsedTime = 0;

        var f = Interpolate.Ease(Interpolate.EaseType.EaseInOutCirc);

        while (elapsedTime < transitionTime)
        {
            var percent = elapsedTime / transitionTime;
            Vector3 camPos;
            if (easeCamera)
            {
                camPos = Interpolate.Ease(f, camStartPos, camEndPos - camStartPos, elapsedTime, transitionTime);
                worldTransform.localScale = Interpolate.Ease(f, startScale, endScale - startScale, elapsedTime, transitionTime);
            }
            else
            {
                camPos = Vector3.Lerp(camStartPos, camEndPos, percent);
                worldTransform.localScale = Vector3.Lerp(startScale, endScale, percent);
            }

            cam?.MoveCamera(camPos);

            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        worldTransform.localScale = endScale;
        cam?.MoveCamera(camEndPos);

        var endPlayerPos = playerPos / scaleFactor;
        playerRigidBody.gameObject.SetActive(true);

        Instantiate(cloudPrefab, endPlayerPos, Quaternion.identity);

        RespawnPlayer(endPlayerPos);
    }

    IEnumerator ExecuteAfterTime(float time, VoidCallback callback)
    {
        yield return new WaitForSeconds(time);
        callback();
    }
}
