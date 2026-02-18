using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    public bool IsGrounded;

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
    private PlayerDash dash;
    private Animator anim;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        input = GetComponent<PlayerInputHandler>();
        playerAudio = GetComponent<PlayerAudio>();
        dash = GetComponent<PlayerDash>();
        anim = GetComponent<Animator>();
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
        if (dash != null && dash.IsDashing)
        {
            playerAudio?.StopFootsteps();
            anim?.SetBool("isWalking", false);
            return;
        }

        float x = input.MoveInput.x;

        // Move
        rb.linearVelocity = new Vector2(x * walkSpeed, rb.linearVelocity.y);

        // Anim
        anim?.SetBool("isWalking", Mathf.Abs(x) > 0.1f && IsGrounded);

        // Footsteps
        HandleFootsteps(x);

        // Facing direction
        Flip(x);
    }

    private void HandleFootsteps(float xInput)
    {
        if (playerAudio == null) return;

        bool isWalking = Mathf.Abs(xInput) > 0.1f && IsGrounded;

        if (isWalking) playerAudio.StartFootsteps();
        else playerAudio.StopFootsteps();
    }

    private void Flip(float xInput)
    {
        if (xInput > 0f)
            transform.localScale = new Vector3(1f, 0.875f, 1f);
        else if (xInput < 0f)
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
