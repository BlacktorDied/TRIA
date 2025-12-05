using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    public bool IsGrounded { get; set; }

    #region Variables

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Vector2 boxSize = new Vector2(0.02f, 0.05f);
    [SerializeField] private float castDistance = 0.1f;
    [SerializeField] private LayerMask groundLayer;

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
        rb.linearVelocity = new Vector2(x * moveSpeed, rb.linearVelocity.y);
    }

    #endregion

    // Draw Gizmo
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