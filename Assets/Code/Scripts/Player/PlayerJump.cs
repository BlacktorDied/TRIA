using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]

public class PlayerJump : MonoBehaviour
{
    #region Variables

    [Header("Jump Settings")]
    [SerializeField] private float jumpForce = 9f;
    [SerializeField] private int maxAirJumps = 0;
    private int jumpsRemaining;

    [Header("Variable Jump Settings")]
    [SerializeField] private float jumpCutMultiplier = 0.5f;
    [SerializeField] private float fallMultiplier = 2f;

    [Header("Timers Settings")]
    [SerializeField] private float coyoteTime = 0.1f;
    [SerializeField] private float jumpBufferTime = 0.1f;
    private float coyoteTimer;
    private float jumpBufferTimer;

    private Rigidbody2D rb;
    private PlayerInputHandler input;
    private PlayerMovement movement;
    private PlayerAudio playerAudio;

    #endregion

    #region Unity Methods

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        input = GetComponent<PlayerInputHandler>();
        movement = GetComponent<PlayerMovement>();
        playerAudio = GetComponent<PlayerAudio>();
    }

    void Update()
    {
        HandleTimers();
        HandleJump();
    }

    void FixedUpdate()
    {
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

        coyoteTimer = Mathf.Max(coyoteTimer, 0f);
        jumpBufferTimer = Mathf.Max(jumpBufferTimer, 0f);
    }

    private void HandleJump()
    {
        if (jumpBufferTimer <= 0f) return;

        if (movement.IsGrounded || coyoteTimer > 0f)
        {
            PerformJump();
            jumpsRemaining = maxAirJumps;
        }
        else if (jumpsRemaining > 0)
        {
            PerformJump();
            jumpsRemaining--;
        }

        jumpBufferTimer = 0f;
    }

    private void PerformJump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        coyoteTimer = 0f;

        playerAudio?.PlayJump();
    }


    private void ApplyVariableJump()
    {
        if (!input.JumpHeld && rb.linearVelocity.y > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * jumpCutMultiplier);
        }

        if (rb.linearVelocity.y < 0)
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
    }

    private void ResetJumpsIfGrounded()
    {
        if (movement.IsGrounded) jumpsRemaining = maxAirJumps;
    }

    #endregion
}
