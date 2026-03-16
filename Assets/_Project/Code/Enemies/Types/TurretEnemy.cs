using TRIA.Combat;
using TRIA.Core;
using UnityEngine;

namespace TRIA.Enemies
{
    public class TurretEnemy : Enemy
    {
        [Header("Shooting Settings")]
        [SerializeField]
        private GameObject bulletPrefab;

        [SerializeField]
        private Transform shootPoint; // Optional: Assign a child object or leave null to shoot from center

        [SerializeField]
        private float bulletDamage = 1f;

        [SerializeField]
        private float bulletSpeed = 10f;

        [SerializeField]
        private float bulletLifetime = 4f;

        [Header("Combat Timing")]
        [SerializeField]
        private float fireRate = 1f; // Seconds between shots

        [SerializeField]
        private float range = 12f;

        private float _fireTimer;
        private SpriteRenderer _sprite;

        protected override void Start()
        {
            base.Start();
            _sprite = GetComponent<SpriteRenderer>();

            // Static turret doesn't need to move, so we lock its position
            rb.bodyType = RigidbodyType2D.Static;
        }

        private void Update()
        {
            if (isDead || playerTransform == null)
                return;

            float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

            // 1. Only act if player is within range
            if (distanceToPlayer <= range)
            {
                // 2. Face the player
                if (_sprite != null)
                {
                    _sprite.flipX = playerTransform.position.x < transform.position.x;
                }

                // 3. Handle Shooting Timer
                _fireTimer += Time.deltaTime;
                if (_fireTimer >= fireRate)
                {
                    Shoot();
                    _fireTimer = 0;
                }
            }
        }

        private void Shoot()
        {
            if (bulletPrefab == null)
                return;

            // Determine spawn position (use shootPoint if assigned, otherwise enemy center)
            Vector3 spawnPos = shootPoint != null ? shootPoint.position : transform.position;

            GameObject bulletObj = Instantiate(bulletPrefab, spawnPos, Quaternion.identity);
            bulletObj.layer = LayerMask.NameToLayer("Projectile");

            Projectile proj = bulletObj.GetComponent<Projectile>();
            if (proj != null)
            {
                proj.shooter = gameObject; // Prevents bullet from hitting the turret
                proj.direction = (playerTransform.position - spawnPos).normalized;
                proj.speed = bulletSpeed;
                proj.damage = bulletDamage;
            }

            Destroy(bulletObj, bulletLifetime);
        }

        private void OnDrawGizmosSelected()
        {
            // Visualize shooting range
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, range);

            if (shootPoint != null)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(shootPoint.position, 0.2f);
            }
        }
    }
}
