using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]

public class PlayerMovement : MonoBehaviour
{
    public bool IsGrounded;

    #region Variables

    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 5f;

    [Header("Ground Check Settings")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Vector2 boxSize = new Vector2(0.8f, 0.05f);
    [SerializeField] private float castDistance = 0.1f;

    private Rigidbody2D rb;
    private PlayerInputHandler input;
    private PlayerAudio playerAudio;

    #endregion

    #region Unity Methods

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        input = GetComponent<PlayerInputHandler>();
        playerAudio = GetComponent<PlayerAudio>();
    }

    private void Update()
    {
        IsGrounded = Physics2D.BoxCast(
            groundCheck.position,
            boxSize,
            0f,
            Vector2.down,
            castDistance,
            groundLayer
        ).collider != null;
    }
    private void FixedUpdate()
    {
        PlayerDash dash = GetComponent<PlayerDash>();
        if (dash != null && dash.IsDashing)
        {
            playerAudio?.StopFootsteps();
            return;
        }

        float x = input.MoveInput.x;
        rb.linearVelocity = new Vector2(x * walkSpeed, rb.linearVelocity.y);

        HandleFootsteps(x);
        Flip(x);
    }

    #endregion

    private void HandleFootsteps(float xInput)
    {
        bool isWalking = Mathf.Abs(xInput) > 0.1f && IsGrounded;

        if (playerAudio == null) return;

        if (isWalking) playerAudio.StartFootsteps();
        else playerAudio.StopFootsteps();
    }

    private void Flip(float xInput)
    {
        if (xInput > 0)
            transform.localScale = new Vector3(1f, 0.875f, 1f);
        else if (xInput < 0)
            transform.localScale = new Vector3(-1f, 0.875f, 1f);
    }

    private void OnDrawGizmos()
    {
        if (groundCheck != null)
        {
            Gizmos.color = IsGrounded ? Color.green : Color.red;
            Gizmos.DrawWireCube(
                (Vector2)groundCheck.position + Vector2.down * castDistance,
                boxSize
            );
        }
    }
}