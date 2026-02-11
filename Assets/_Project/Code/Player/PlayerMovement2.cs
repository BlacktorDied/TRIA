//using UnityEngine;

//[RequireComponent(typeof(Rigidbody2D))]
//public class PlayerMovement : MonoBehaviour
//{
//    public bool IsGrounded;

//    #region Variables

//    [Header("Movement Settings")]
//    [SerializeField] private float walkSpeed = 5f;

//    [Header("Ground Check Settings")]
//    [SerializeField] private Transform groundCheck;
//    [SerializeField] private LayerMask groundLayer;
//    [SerializeField] private Vector2 boxSize = new Vector2(0.8f, 0.05f);
//    [SerializeField] private float castDistance = 0.1f;

//    [Header("Knockback Settings")]
//    [SerializeField] private float knockbackDuration = 0.15f;

//    private Rigidbody2D rb;
//    private PlayerInputHandler input;

//    private bool isKnockedBack;
//    private float knockbackTimer;

//    #endregion

//    #region Unity Methods

//    private void Awake()
//    {
//        rb = GetComponent<Rigidbody2D>();
//        input = GetComponent<PlayerInputHandler>();
//    }

//    private void Update()
//    {
//        IsGrounded = Physics2D.BoxCast(
//            groundCheck.position,
//            boxSize,
//            0f,
//            Vector2.down,
//            castDistance,
//            groundLayer
//        ).collider != null;

//        // Handle knockback timer
//        if (isKnockedBack)
//        {
//            knockbackTimer -= Time.deltaTime;
//            if (knockbackTimer <= 0f)
//                isKnockedBack = false;
//        }
//    }

//    private void FixedUpdate()
//    {
//        // Disable movement during knockback
//        if (isKnockedBack) return;

//        PlayerDash dash = GetComponent<PlayerDash>();
//        if (dash != null && dash.IsDashing) return;

//        float x = input.MoveInput.x;
//        rb.linearVelocity = new Vector2(x * walkSpeed, rb.linearVelocity.y);
//    }

//    #endregion

//    #region Knockback

//    public void ApplyKnockback(Vector2 direction, float force)
//    {
//        isKnockedBack = true;
//        knockbackTimer = knockbackDuration;

//        rb.linearVelocity = Vector2.zero;
//        rb.AddForce(direction.normalized * force, ForceMode2D.Impulse);
//    }

//    #endregion

//    private void OnDrawGizmos()
//    {
//        if (groundCheck != null)
//        {
//            Gizmos.color = IsGrounded ? Color.green : Color.red;
//            Gizmos.DrawWireCube(
//                (Vector2)groundCheck.position + Vector2.down * castDistance,
//                boxSize
//            );
//        }
//    }
//}
