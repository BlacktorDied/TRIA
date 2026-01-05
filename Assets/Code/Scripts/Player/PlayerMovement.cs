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

    #endregion

    #region Unity Methods

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        input = GetComponent<PlayerInputHandler>();
    }

    private void Update()
    {
        IsGrounded = Physics2D.BoxCast(
            groundCheck.position,  // The center point of the box (set by your child Transform)
            boxSize,               // The width and height of the box (e.g., 0.8 width, 0.05 height)
            0f,                    // Angle of the box (0 means no rotation)
            Vector2.down,          // Direction of the cast (downwards)
            castDistance,          // How far the box is projected
            groundLayer            // Only checks colliders on this layer
        ).collider != null;        // The BoxCast returns RaycastHit2D; check if a collider was hit
    }

    private void FixedUpdate()
    {
        float x = input.MoveInput.x;
        rb.linearVelocity = new Vector2(x * walkSpeed, rb.linearVelocity.y);
        Flip();
    }

    #endregion

    void Flip()
    {
        if (input.MoveInput.x > 0)
            transform.localScale = new Vector3(1f, 0.875f, 1f);
        else if (input.MoveInput.x < 0)
            transform.localScale = new Vector3(-1f, 0.875f, 1f);
    }

    private void OnDrawGizmos()
    {
        if (groundCheck != null)
        {
            Gizmos.color = IsGrounded ? Color.green : Color.red;
            Gizmos.DrawWireCube((Vector2)groundCheck.position + Vector2.down * castDistance, boxSize);
        }
    }
}