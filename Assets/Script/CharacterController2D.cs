using UnityEngine;
using UnityEngine.Events;

// Using Internet 2d Controller to get things rolling
// https://sharpcoderblog.com/blog/2d-platformer-character-controller

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CapsuleCollider2D))]
public class CharacterController2D : MonoBehaviour
{
	public Animator animator;

    // Move player in 2D space
    public float maxSpeed = 3.4f;
    public float jumpHeight = 6.5f;
    public float gravityScale = 1.5f;
	public float jumpScaleFactor = 1f;
    public Camera mainCamera;

    bool facingRight = true;
    bool controllerEnabled = true;
    float moveDirection = 0;
    bool isGrounded = false;
    Vector3 cameraPos;
    Rigidbody2D r2d;
    CapsuleCollider2D mainCollider;
    Transform t;

    public bool Enabled
    {
        get => controllerEnabled;
    }

    public void Disable() {
        r2d.constraints = RigidbodyConstraints2D.FreezeAll;
        animator.speed = 0;
        controllerEnabled = false;
    }

    public void Enable() {
        r2d.constraints = RigidbodyConstraints2D.FreezeRotation;
        animator.speed = 1;
        controllerEnabled = true;
    }

    // Use this for initialization
    void Start()
    {
        t = transform;
        r2d = GetComponent<Rigidbody2D>();
        mainCollider = GetComponent<CapsuleCollider2D>();
        r2d.freezeRotation = true;
        r2d.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        r2d.gravityScale = gravityScale;
        facingRight = t.localScale.x > 0;

        if (mainCamera)
        {
            cameraPos = mainCamera.transform.position;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(!controllerEnabled) {
            return;
        }

        // Movement controls
        if (Input.GetAxis("Horizontal") != 0)
        {
            moveDirection = Input.GetAxis("Horizontal");
        }
        else
        {
            if (isGrounded || r2d.velocity.magnitude < 0.01f)
            {
                moveDirection = 0;
            }
        }

        // Change facing direction
        if (moveDirection != 0)
        {
            if (moveDirection > 0 && !facingRight)
            {
                facingRight = true;
                t.localScale = new Vector3(Mathf.Abs(t.localScale.x), t.localScale.y, transform.localScale.z);
            }
            if (moveDirection < 0 && facingRight)
            {
                facingRight = false;
                t.localScale = new Vector3(-Mathf.Abs(t.localScale.x), t.localScale.y, t.localScale.z);
            }
        }

        // Jumping
        if (Input.GetButton("Jump") && isGrounded)
        {
            r2d.velocity = new Vector2(r2d.velocity.x, jumpHeight) ;
			animator.SetTrigger("jump");
        }


		animator.SetBool("isGrounded", isGrounded);
		animator.SetBool("isFalling", r2d.velocity.y < 0f);
		if (isGrounded) {
			animator.SetBool("isMoving", moveDirection != 0.0f);
		}
    }

    void FixedUpdate()
    {
        if(!controllerEnabled) {
            return;
        }

        Bounds colliderBounds = mainCollider.bounds;
        float colliderRadius = mainCollider.size.x * 0.4f * Mathf.Abs(transform.localScale.x);
        Vector3 groundCheckPos = colliderBounds.min + new Vector3(colliderBounds.size.x * 0.5f, colliderRadius * 0.9f, 0);
        // Check if player is grounded
        Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheckPos, colliderRadius);
        //Check if any of the overlapping colliders are not player collider, if so, set isGrounded to true
        isGrounded = false;
        if (colliders.Length > 0)
        {
            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i] != mainCollider)
                {
                    isGrounded = true;
                    break;
                }
            }
        }

        // Apply movement velocity
        r2d.velocity = new Vector2((moveDirection) * maxSpeed, r2d.velocity.y);

        // Simple debug
        Debug.DrawLine(groundCheckPos, groundCheckPos - new Vector3(0, colliderRadius, 0), isGrounded ? Color.green : Color.red);
        Debug.DrawLine(groundCheckPos, groundCheckPos - new Vector3(colliderRadius, 0, 0), isGrounded ? Color.green : Color.red);
    }

    void LateUpdate()
    {
        Vector3 cameraTargetPosition = new Vector3(t.position.x, t.position.y, cameraPos.z);
        Vector3 smoothedPosition = Vector3.Lerp(mainCamera.transform.position, cameraTargetPosition, 0.005f);

        // Camera follow
        if (mainCamera)
        {
            mainCamera.transform.position = smoothedPosition;
        }
    }
}
