using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    private AudioSource audioSource;
    public AudioClip start;
    public AudioClip death;
    public static GameManager Instance { get { return _instance; } }

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.PlayOneShot(start, 0.7F);
    }

    private void Awake() {
        if(_instance == null) {
            _instance = this;
        }    
        else {
            this.enabled = false;
        }
    }

    public Vector3 lastSpawnPosition;
    public Rigidbody2D playerRigidBody;
    public void TeleportPlayer(float scaleFactor) {
        transform.localScale /= scaleFactor;
    }

    public void KillPlayer() {
        audioSource.PlayOneShot(death, 0.7F);
        RespawnPlayer(lastSpawnPosition);
    }
    public void RespawnPlayer(Vector3 position) {
        // Replace with animated to new position
        lastSpawnPosition = position;
        playerRigidBody.isKinematic = true;
        playerRigidBody.transform.position = position;
        playerRigidBody.velocity = Vector3.zero;
        playerRigidBody.isKinematic = false;
    }
}
