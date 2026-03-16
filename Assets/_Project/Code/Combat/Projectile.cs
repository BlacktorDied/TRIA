using TRIA.Core; // Ensure this is here for IDamageable
using UnityEngine;

namespace TRIA.Combat
{
    public class Projectile : MonoBehaviour
    {
        [HideInInspector]
        public Vector2 direction;

        [HideInInspector]
        public float speed;

        [HideInInspector]
        public float damage;

        [HideInInspector]
        public GameObject shooter;

        private Rigidbody2D _rb;

        void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            // Continuous detection prevents "phasing" through thin floors
            _rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            _rb.gravityScale = 0f;
        }

        void FixedUpdate()
        {
            _rb.linearVelocity = direction * speed;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            // 1. Ignore the shooter
            if (shooter != null && other.gameObject == shooter)
                return;

            // 2. Damage logic: Search parent/children if the hit object itself isn't IDamageable
            // Some players have multiple colliders; we want the one with the health script
            IDamageable target = other.GetComponentInParent<IDamageable>();

            if (target != null)
            {
                // Only hit the Player if this bullet was shot by an Enemy
                // (Prevents enemies from accidentally shooting each other if you want)
                target.TakeDamage(damage);
                Kill();
                return;
            }

            // 3. Environment logic: Kill on Ground or any solid non-trigger
            // IMPORTANT: Make sure the Player's layer is NOT "Ground"
            if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
            {
                Kill();
                return;
            }

            // 4. Fallback: If we hit a solid wall (not a trigger), destroy the bullet
            if (!other.isTrigger)
            {
                Kill();
            }
        }

        public void Kill()
        {
            // You can add an impact particle here later
            Destroy(gameObject);
        }
    }
}
