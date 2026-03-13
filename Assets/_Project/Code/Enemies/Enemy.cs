using TRIA.Core;
using UnityEngine;

namespace TRIA.Enemies
{
    public abstract class Enemy : MonoBehaviour, IDamageable
    {
        [Header("Base Enemy Stats")]
        public float health;
        public float maxHealth = 3f;
        public float contactDamage = 1f; // How much damage player takes on touch

        protected Rigidbody2D rb;
        protected bool isDead;
        protected Transform playerTransform;

        protected virtual void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            health = maxHealth;

            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null)
                playerTransform = p.transform;
        }

        public virtual void TakeDamage(float amount)
        {
            if (isDead)
                return;
            health -= amount;
            Debug.Log($"{gameObject.name} took damage. Health: {health}");

            if (health <= 0)
                Die();
        }

        // --- ADD THIS LOGIC TO HURT PLAYER ---
        protected virtual void OnCollisionEnter2D(Collision2D collision)
        {
            if (isDead)
                return;

            // Check if what we hit is Damageable (The Player)
            IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();

            if (damageable != null && collision.gameObject.CompareTag("Player"))
            {
                damageable.TakeDamage(contactDamage);
            }
        }

        public void EnemyHit(float amount) => TakeDamage(amount);

        protected virtual void Die()
        {
            isDead = true;
            Destroy(gameObject);
        }
    }
}
