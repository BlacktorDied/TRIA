using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerJump : MonoBehaviour
{
    #region Variables

    [Header("Jump Settings")]
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private int maxJumps = 2;

    [Header("Coyote Time")]
    [SerializeField] private float coyoteTime = 0.1f;
    private float coyoteTimer;

    [Header("Jump Buffer")]
    [SerializeField] private float jumpBufferTime = 0.1f;
    private float jumpBufferTimer;

    private Rigidbody2D rb;
    private PlayerInputHandler input;
    private PlayerMovement movement;

    private int jumpCount;

    #endregion

    #region Unity Methods

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        input = GetComponent<PlayerInputHandler>();
        movement = GetComponent<PlayerMovement>();
    }

    void Update()
    {
        HandleTimers();
        HandleJump();
        ResetJumpsIfGrounded();
    }

    #endregion

    #region Jump Logic

    private void HandleTimers()
    {
        // Coyote time
        if (movement.IsGrounded)
            coyoteTimer = coyoteTime;
        else
            coyoteTimer -= Time.deltaTime;

        // Jump buffer
        if (input.JumpPressed)
            jumpBufferTimer = jumpBufferTime;
        else
            jumpBufferTimer -= Time.deltaTime;
    }

    private void HandleJump()
    {
        if (jumpBufferTimer > 0f && (coyoteTimer > 0f || jumpCount < maxJumps))
        {
            PerformJump();
            jumpBufferTimer = 0f;
        }
    }

    private void PerformJump()
    {
        // Reset vertical velocity for consistent jumps
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);

        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

        jumpCount++;
        coyoteTimer = 0f; // no coyote after jumping
    }

    private void ResetJumpsIfGrounded()
    {
        if (movement.IsGrounded)
            jumpCount = 0;
    }

    #endregion
}
