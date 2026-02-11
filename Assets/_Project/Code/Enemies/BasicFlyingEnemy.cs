using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BasicFlyingEnemy : Enemy
{
    [Header("Movement")]
    [SerializeField]
    private float moveSpeed = 4f;

    [SerializeField]
    private float minSpeedMultiplier = 0.2f; // how slow it can get near player

    [Header("Player Detection")]
    [SerializeField]
    private Transform player;

    [SerializeField]
    private float detectionRange = 8f;

    [Header("Knockback on Hit")]
    [SerializeField]
    private float knockbackForce = 5f;

    [SerializeField]
    private float knockbackUpward = 0.5f;

    protected override void Start()
    {
        base.Start();

        rb.gravityScale = 0f;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
    }

    private void FixedUpdate()
    {
        if (isDead || player == null)
            return;

        Vector2 direction = player.position - transform.position;
        float distance = direction.magnitude;

        if (distance <= detectionRange)
        {
            Vector2 moveDir = direction.normalized;

            // Calculate slowdown multiplier
            float distanceFactor = distance / detectionRange;
            float speedMultiplier = Mathf.Lerp(minSpeedMultiplier, 1f, distanceFactor);

            rb.linearVelocity = moveDir * moveSpeed * speedMultiplier;
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }

        // Flip sprite
        if (player.position.x != transform.position.x)
        {
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Sign(player.position.x - transform.position.x) * Mathf.Abs(scale.x);
            transform.localScale = scale;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDead)
            return;

        if (collision.collider.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.collider.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }

            //PlayerMovement playerMovement = collision.collider.GetComponent<PlayerMovement>();
            //if (playerMovement != null)
            //{
            //    Vector2 knockbackDir = (collision.transform.position - transform.position).normalized;
            //    knockbackDir.y = knockbackUpward;
            //    playerMovement.ApplyKnockback(knockbackDir, knockbackForce);
            //}

            //// Enemy knockback
            //Vector2 enemyKnockback = -(collision.transform.position - transform.position).normalized * (knockbackForce / 2f);
            //rb.AddForce(enemyKnockback, ForceMode2D.Impulse);
        }
    }

    public void SetPlayer(Transform newPlayer)
    {
        player = newPlayer;
    }
}
