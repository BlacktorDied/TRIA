using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerJump : MonoBehaviour
{
    #region Variables

    [Header("Jump Settings")]
    [SerializeField] private float jumpForce = 8.85f;
    [SerializeField] private int maxJumps = 2;

    [Header("Variable Jump")]
    public float jumpCutMultiplier = 0.5f;
    public float fallMultiplier = 2f;

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
        ApplyVariableJump();
        ResetJumpsIfGrounded();
    }

    #endregion

    #region Jump Logic

    private void HandleTimers()
    {
        coyoteTimer = movement.IsGrounded
            ? coyoteTime
            : coyoteTimer - Time.deltaTime;

        jumpBufferTimer = input.JumpPressed
            ? jumpBufferTime
            : jumpBufferTimer - Time.deltaTime;
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
        coyoteTimer = 0f;
        movement.IsGrounded = false;
        Debug.Log("isGrounded:" + movement.IsGrounded);
    }

    private void ApplyVariableJump()
    {
        // Cut jump short if player lets go early
        if (!input.JumpHeld && rb.linearVelocity.y > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * jumpCutMultiplier);
        }

        // Faster falling (Mario style)
        if (rb.linearVelocity.y < 0)
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
    }

    private void ResetJumpsIfGrounded()
    {
        if (movement.IsGrounded)
            jumpCount = 0;
    }

    #endregion
}
