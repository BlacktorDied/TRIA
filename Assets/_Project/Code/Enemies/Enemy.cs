using TRIA.Core;
using UnityEngine;
using System.Collections;

namespace TRIA.Enemies
{
    public abstract class Enemy : MonoBehaviour, IDamageable
    {
        [Header("Base Enemy Stats")]
        public float health;
        public float maxHealth = 3f;
        public float contactDamage = 1f;

        [Header("Knockback Settings")]
        [SerializeField] private float knockbackDistance = 2f;
        [SerializeField] private float knockbackUpDistance = 1f;
        [SerializeField] private float knockbackDuration = 0.2f;
        [SerializeField] private LayerMask obstacleLayer;

        [Header("Loot Drop")]
        [SerializeField] private GameObject healthPickupPrefab;
        [SerializeField] private float healthDropChance = 0.0025f; // 0.25%

        protected Rigidbody2D rb;
        protected bool isDead;
        protected Transform playerTransform;
        private bool _isKnockedBack = false;

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

            if (!_isKnockedBack && playerTransform != null)
            {
                Vector3 knockDir = (transform.position - playerTransform.position).normalized;
                knockDir.y = 0;

                Vector3 desiredTarget = transform.position + new Vector3(
                    knockDir.x * knockbackDistance,
                    knockbackUpDistance,
                    0f
                );

                // Prevent knockback through walls
                RaycastHit2D hit = Physics2D.Raycast(transform.position, knockDir, knockbackDistance, obstacleLayer);
                if (hit.collider != null)
                {
                    desiredTarget.x = hit.point.x - knockDir.x * 0.1f;
                }

                StartCoroutine(ApplyKnockbackPosition(desiredTarget, knockbackDuration));
            }

            if (health <= 0)
                Die();
        }

        private IEnumerator ApplyKnockbackPosition(Vector3 targetPos, float duration)
        {
            _isKnockedBack = true;

            Vector3 startPos = transform.position;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;

                transform.position = Vector3.Lerp(startPos, targetPos, t);
                yield return null;
            }

            transform.position = targetPos;
            _isKnockedBack = false;
        }

        protected virtual void OnCollisionEnter2D(Collision2D collision)
        {
            if (isDead)
                return;

            if (collision.gameObject.CompareTag("Player"))
            {
                IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();

                if (damageable != null)
                    damageable.TakeDamage(contactDamage);

                // Apply knockback to player
                TRIA.Player.PlayerMovement player =
                    collision.gameObject.GetComponent<TRIA.Player.PlayerMovement>();

                if (player != null)
                {
                    Vector2 direction = (collision.transform.position - transform.position).normalized;

                    direction.y = 0.4f;

                    player.ApplyKnockback(direction, 10f);
                }
            }
        }

        public void EnemyHit(float amount) => TakeDamage(amount);

        protected virtual void Die()
        {
            if (isDead)
                return;

            isDead = true;

            TryDropHealthPickup();

            Destroy(gameObject);
        }

        private void TryDropHealthPickup()
        {
            if (healthPickupPrefab == null)
                return;

            float roll = Random.value;

            if (roll <= healthDropChance)
            {
                Instantiate(
                    healthPickupPrefab,
                    transform.position,
                    Quaternion.identity
                );
            }
        }
    }
}