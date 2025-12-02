using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    public bool IsGrounded { get; private set; }

    #region Veriables

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.15f;
    [SerializeField] private LayerMask groundLayer;

    private Rigidbody2D rb;
    private PlayerInputHandler input;

    #endregion

    #region Unity Methods

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        input = GetComponent<PlayerInputHandler>();
        groundCheck = groundCheck ?? throw new System.Exception("groundCheck not assigned!");
    }

    void Update()
    {
        IsGrounded = Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius,
            groundLayer
        );
    }

    private void FixedUpdate()
    {
        float x = input.MoveInput.x;
        rb.linearVelocity = new Vector2(x * moveSpeed, rb.linearVelocity.y);
    }

    #endregion

    private void OnDrawGizmosSelected()
    {
        if (!groundCheck) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}
