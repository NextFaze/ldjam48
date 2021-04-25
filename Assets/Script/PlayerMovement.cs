using UnityEngine;

[RequireComponent(typeof(CharacterController2D))]
public class PlayerMovement : MonoBehaviour {
    
    CharacterController2D controller;
    public Animator animator;

    public float runSpeed =  40f;
    float horizontalMove = 0f;
    bool jump = false;

    private void Awake() {
        controller = GetComponent<CharacterController2D>();
    }

    private void Update() {
        horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;
        animator.SetFloat("Speed", Mathf.Abs(horizontalMove));

        if (Input.GetButtonDown("Jump")) {
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