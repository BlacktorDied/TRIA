using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Settings")]
    [SerializeField]
    protected float maxHealthPoints = 3f;

    [SerializeField]
    protected float healthPoints;

    [SerializeField]
    protected int damage = 1;

    [Header("Health Drop Settings")]
    [SerializeField]
    private GameObject healthPickupPrefab; // Assign health coin prefab in Inspector

    [SerializeField, Range(0f, 1f)]
    private float healthDropChance = 0.3f; // 30% chance to drop health

    [Header("Attack Settings")]
    [SerializeField]
    protected float attackCooldown = 1f;

    [Header("Knockback Settings")]
    [SerializeField]
    protected float playerKnockbackForce = 6f;

    [SerializeField]
    protected float playerKnockbackUpward = 0.5f;

    protected Rigidbody2D rb;
    protected float lastAttackTime;

    protected bool isDead = false;

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Ensure enemy starts at full health
        healthPoints = maxHealthPoints;
    }

    public virtual void EnemyHit(float damageDone)
    {
        if (isDead)
            return;

        healthPoints -= damageDone;
        healthPoints = Mathf.Clamp(healthPoints, 0, maxHealthPoints);

        if (healthPoints <= 0f)
        {
            Die();
        }
    }

    public virtual void Heal(float amount)
    {
        if (isDead)
            return;

        healthPoints += amount;
        healthPoints = Mathf.Clamp(healthPoints, 0, maxHealthPoints);
    }

    protected virtual void Die()
    {
        if (isDead)
            return;
        isDead = true;

        DropHealthPickup();

        Destroy(gameObject);
    }

    private void DropHealthPickup()
    {
        if (healthPickupPrefab == null)
            return;

        if (Random.value <= healthDropChance) // Random chance
        {
            GameObject pickup = Instantiate(
                healthPickupPrefab,
                transform.position,
                Quaternion.identity
            );

            Rigidbody2D pickupRb = pickup.GetComponent<Rigidbody2D>();
            if (pickupRb != null)
            {
                // Give a small random velocity so it "flies out"
                float forceX = Random.Range(-2f, 2f);
                float forceY = Random.Range(2f, 4f);
                pickupRb.linearVelocity = new Vector2(forceX, forceY);
            }
        }
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDead)
            return;
        if (!collision.collider.CompareTag("Player"))
            return;

        if (Time.time < lastAttackTime + attackCooldown)
            return;

        // Apply knockback
        //PlayerMovement playerMovement = collision.collider.GetComponent<PlayerMovement>();
        //if (playerMovement != null)
        //{
        //    Vector2 knockbackDir = (collision.transform.position - transform.position).normalized;
        //    knockbackDir.y = playerKnockbackUpward;
        //    playerMovement.ApplyKnockback(knockbackDir, playerKnockbackForce);
        //}

        //// Deal damage
        //PlayerHealth health = collision.collider.GetComponent<PlayerHealth>();
        //if (health != null)
        //{
        //    health.TakeDamage(damage);
        //    Debug.Log($"{name} damaged player for {damage} HP!");
        //}

        lastAttackTime = Time.time;
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (isDead)
            return;
        if (!other.CompareTag("Player"))
            return;

        if (Time.time < lastAttackTime + attackCooldown)
            return;

        // Apply knockback
        //PlayerMovement playerMovement = other.GetComponent<PlayerMovement>();
        //if (playerMovement != null)
        //{
        //    Vector2 knockbackDir = (other.transform.position - transform.position).normalized;
        //    knockbackDir.y = playerKnockbackUpward;
        //    playerMovement.ApplyKnockback(knockbackDir, playerKnockbackForce);
        //}

        //// Deal damage
        //PlayerHealth health = other.GetComponent<PlayerHealth>();
        //if (health != null)
        //{
        //    health.TakeDamage(damage);
        //    Debug.Log($"{name} damaged player for {damage} HP!");
        //}

        //lastAttackTime = Time.time;
    }
}
