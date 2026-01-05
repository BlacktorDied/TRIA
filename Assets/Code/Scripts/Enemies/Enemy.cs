using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Settings")]
    [SerializeField] protected float healthPoints;
    [SerializeField] protected int damage;

    [Header("Attack Settings")]
    [SerializeField] private float attackCooldown = 1f;

    protected Rigidbody2D rb;

    private float lastAttackTime;

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    protected virtual void Update()
    {
        if (healthPoints <= 0) Destroy(gameObject);
    }

    public void EnemyHit(float _damageDone)
    {
        healthPoints -= _damageDone;
    }

    protected virtual void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (Time.time >= lastAttackTime + attackCooldown)
        {
            Attack(other);
            lastAttackTime = Time.time;
        }
    }

    protected virtual void Attack(Collider2D player)
    {
        PlayerHealth health = player.GetComponent<PlayerHealth>();
        if (health != null)
        {
            health.TakeDamage(damage);
            Debug.Log("Enemy hit player!");
        }
    }
}

