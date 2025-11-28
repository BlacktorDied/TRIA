using UnityEngine;

public class heromove : MonoBehaviour
{
    [Header("Move")]
    public float moveSpeed = 7f;

    [Header("Jump")]
    public float jumpForce = 12f;
    public float fallMultiplier = 2.2f;      
    public float lowJumpMultiplier = 3f;     

    private Rigidbody2D rb;
    private bool isGrounded = false;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
       
        float move = Input.GetAxisRaw("Horizontal"); 
        rb.linearVelocity = new Vector2(move * moveSpeed, rb.linearVelocity.y);

      
        if (move != 0)
            transform.localScale = new Vector3(Mathf.Sign(move), 1, 1);

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

     
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        if (rb.linearVelocity.y < 0)
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (rb.linearVelocity.y > 0 && !Input.GetKey(KeyCode.Space))
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
