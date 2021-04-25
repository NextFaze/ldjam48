using UnityEngine;

[RequireComponent(typeof(CharacterController2D))]
[RequireComponent(typeof(AudioSource))]
public class PlayerMovement : MonoBehaviour {
    
    CharacterController2D controller;
    public Animator animator;

    public float runSpeed =  40f;
    float horizontalMove = 0f;
    bool jump = false;


    private AudioSource audioSource;
    public AudioClip[] jumpAudio;



    public void Disable()
    {
        controller.ToggleFreeze(true);
        animator.speed = 0;
        enabled = false;
    }

    public void Enable()
    {
        controller.ToggleFreeze(false);
        animator.speed = 1;
        enabled = true;
    }

    private void Awake() {
        controller = GetComponent<CharacterController2D>();
        audioSource = GetComponent<AudioSource>();
    }

    private void Update() {
        horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;
        animator.SetFloat("Speed", Mathf.Abs(horizontalMove));

        if (Input.GetButtonDown("Jump")) {
            audioSource.PlayOneShot(jumpAudio[UnityEngine.Random.Range(0, jumpAudio.Length)], 2F);
            jump = true;
            animator.SetBool("isJumping", true);
        }
    }

    public void OnLanding() {
        animator.SetBool("isJumping", false);
        animator.SetBool("isFalling", false);
    }

    public void OnFalling() {
        animator.SetBool("isJumping", false);
        animator.SetBool("isFalling", true);
    }

    private void FixedUpdate() {
        controller.Move(horizontalMove * Time.fixedDeltaTime, false, jump);
        jump = false;
    }
}