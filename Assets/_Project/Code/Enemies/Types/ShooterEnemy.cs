using System.Collections;
using TRIA.Combat;
using TRIA.Core;
using UnityEngine;

namespace TRIA.Enemies
{
    public class ShooterEnemy : Enemy
    {
        [Header("Patrol Settings")]
        [SerializeField]
        private float patrolDistance = 5f;

        [SerializeField]
        private float wanderSpeed = 3f;

        [SerializeField]
        private float stopChance = 0.3f;

        [SerializeField]
        private Vector2 stopTimeRange = new Vector2(1f, 2f);

        [Header("Combat Settings")]
        [SerializeField]
        private GameObject bulletPrefab;

        [SerializeField]
        private float bulletDamage = 1f;

        [SerializeField]
        private float bulletSpeed = 12f;

        [SerializeField]
        private float bulletLifetime = 3f;

        [SerializeField]
        private float detectionRange = 8f;

        [SerializeField]
        private float retreatDistance = 3f;

        [SerializeField]
        private float retreatSpeed = 2f;

        [Header("Burst Logic")]
        [SerializeField]
        private int bulletsPerBurst = 3;

        [SerializeField]
        private float timeBetweenShots = 0.2f;

        [SerializeField]
        private float timeBetweenBursts = 1.5f;

        [Header("Edge & Wall Detection")]
        [SerializeField]
        private LayerMask groundLayer;

        [SerializeField]
        private float edgeCheckOffset = 0.5f;

        [SerializeField]
        private float edgeCheckDistance = 0.5f;

        private Vector2 _startPos;
        private int _direction = 1;
        private bool _isStopping;
        private bool _isShootingBurst;
        private SpriteRenderer _sprite;

        protected override void Start()
        {
            base.Start();
            _sprite = GetComponent<SpriteRenderer>();
            _startPos = transform.position;
        }

        private void Update()
        {
            if (isDead || playerTransform == null)
                return;

            float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

            // Always face the player if they are within detection range
            if (distanceToPlayer <= detectionRange)
            {
                _sprite.flipX = playerTransform.position.x < transform.position.x;

                // Start shooting if not already in a burst cycle
                if (!_isShootingBurst)
                {
                    StartCoroutine(ShootBurstRoutine());
                }
            }
        }

        private void FixedUpdate()
        {
            if (isDead || playerTransform == null)
                return;

            float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

            // Movement State Machine
            if (distanceToPlayer <= retreatDistance)
            {
                RetreatFromPlayer();
            }
            else if (distanceToPlayer <= detectionRange)
            {
                // Stand ground while shooting
                rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            }
            else
            {
                PatrolLogic();
            }
        }

        private void PatrolLogic()
        {
            if (_isStopping)
                return;

            float distanceFromStart = transform.position.x - _startPos.x;

            // Flip if at patrol boundary or edge of platform
            bool atBoundary =
                (distanceFromStart >= patrolDistance && _direction == 1)
                || (distanceFromStart <= -patrolDistance && _direction == -1);

            if (atBoundary || !IsGroundAhead(_direction))
            {
                _direction *= -1;

                if (Random.value < stopChance)
                {
                    StartCoroutine(StopRoutine());
                }
            }

            rb.linearVelocity = new Vector2(_direction * wanderSpeed, rb.linearVelocity.y);

            // Only update flip based on direction when NOT targeting player
            _sprite.flipX = _direction < 0;
        }

        private void RetreatFromPlayer()
        {
            float retreatDir = transform.position.x > playerTransform.position.x ? 1 : -1;

            if (IsGroundAhead(retreatDir))
            {
                rb.linearVelocity = new Vector2(retreatDir * retreatSpeed, rb.linearVelocity.y);
            }
            else
            {
                rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            }
        }

        private bool IsGroundAhead(float dir)
        {
            // Raycast down from the bottom edge of the sprite to detect floor
            Vector2 origin = new Vector2(
                transform.position.x + (edgeCheckOffset * dir),
                _sprite.bounds.min.y
            );
            RaycastHit2D hit = Physics2D.Raycast(
                origin,
                Vector2.down,
                edgeCheckDistance,
                groundLayer
            );

            Debug.DrawRay(origin, Vector2.down * edgeCheckDistance, Color.red);
            return hit.collider != null;
        }

        private IEnumerator StopRoutine()
        {
            _isStopping = true;
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            yield return new WaitForSeconds(Random.Range(stopTimeRange.x, stopTimeRange.y));
            _isStopping = false;
        }

        private IEnumerator ShootBurstRoutine()
        {
            _isShootingBurst = true;

            for (int i = 0; i < bulletsPerBurst; i++)
            {
                if (isDead)
                    break;
                Shoot();
                yield return new WaitForSeconds(timeBetweenShots);
            }

            yield return new WaitForSeconds(timeBetweenBursts);
            _isShootingBurst = false;
        }

        private void Shoot()
        {
            if (bulletPrefab == null || playerTransform == null)
                return;

            // Spawning directly at the enemy center
            GameObject bulletObj = Instantiate(
                bulletPrefab,
                transform.position,
                Quaternion.identity
            );
            bulletObj.layer = LayerMask.NameToLayer("Projectile");

            Projectile proj = bulletObj.GetComponent<Projectile>();
            if (proj != null)
            {
                proj.shooter = gameObject;
                proj.direction = (playerTransform.position - transform.position).normalized;
                proj.speed = bulletSpeed;
                proj.damage = bulletDamage;
            }

            Destroy(bulletObj, bulletLifetime);
        }

        private void OnDrawGizmosSelected()
        {
            Vector2 center = Application.isPlaying ? _startPos : (Vector2)transform.position;

            // Patrol Line & Circles
            Gizmos.color = Color.cyan;
            Vector3 left = new Vector3(center.x - patrolDistance, center.y, 0);
            Vector3 right = new Vector3(center.x + patrolDistance, center.y, 0);
            Gizmos.DrawLine(left, right);
            Gizmos.DrawWireSphere(left, 0.25f);
            Gizmos.DrawWireSphere(right, 0.25f);

            // Detection Zones
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, detectionRange);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, retreatDistance);
        }
    }
}
