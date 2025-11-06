using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class Boss : MonoBehaviour awgaergaer gaerg aerg aergaer
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 12f;

    [Header("Ground Check Settings")]
    public LayerMask groundLayer;
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;

    private Rigidbody2D rb2d;
    private bool isGrounded;

    void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();

        // Ensure Rigidbody2D settings for platformer
        rb2d.gravityScale = 3f;        // realistic gravity
        rb2d.freezeRotation = true;    // prevent tipping over

        // Auto-create GroundCheck if not assigned
        if (groundCheck == null)
        {
            GameObject gc = new GameObject("GroundCheck");
            gc.transform.parent = transform;
            gc.transform.localPosition = new Vector3(0, -0.5f, 0); // position at feet
            groundCheck = gc.transform;
        }
    }

    void Update()
    {
        // Check if grounded
        if (groundCheck != null)
        {
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        }

        // Horizontal movement
        float moveInput = Input.GetAxis("Horizontal");
        rb2d.linearVelocity = new Vector2(moveInput * moveSpeed, rb2d.linearVelocity.y);

        // Jump
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb2d.linearVelocity = new Vector2(rb2d.linearVelocity.x, jumpForce);
        }
    }

    // Draw ground check in Scene view
    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}

balls