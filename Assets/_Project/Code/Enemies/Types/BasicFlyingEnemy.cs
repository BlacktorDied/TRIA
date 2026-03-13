using TRIA.Core;
using TRIA.Core.Constants;
using UnityEngine;

namespace TRIA.Enemies
{
    [RequireComponent(typeof(Rigidbody2D), typeof(SpriteRenderer), typeof(CircleCollider2D))]
    public class BasicFlyingEnemy : Enemy
    {
        [Header("Aggro Settings")]
        [SerializeField]
        private float detectionRange = 8f;

        [SerializeField]
        private float stopChasingRange = 12f; // Slightly larger to prevent "flickering" aggro

        [Header("Movement")]
        [SerializeField]
        private float moveSpeed = 4f;

        [SerializeField]
        private float acceleration = 5f;

        [Header("Idle Roaming")]
        [SerializeField]
        private float roamRadius = 3f;

        [SerializeField]
        private float roamSpeed = 1.5f;

        [SerializeField]
        private float waitTimeAtPoint = 2f;

        private Vector2 _startPos;
        private Vector2 _roamTarget;
        private float _roamTimer;
        private bool _isChasing;
        private SpriteRenderer _sprite;

        protected override void Start()
        {
            base.Start();
            _sprite = GetComponent<SpriteRenderer>();
            _startPos = transform.position;
            _roamTarget = GetNewRoamPoint();

            // PHYSICS SETUP: Crucial for wall collision
            rb.gravityScale = 0f;
            rb.bodyType = RigidbodyType2D.Dynamic; // Must be Dynamic to collide with walls
            rb.freezeRotation = true;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        }

        private void FixedUpdate()
        {
            if (isDead)
            {
                rb.linearVelocity = Vector2.zero;
                return;
            }

            float distanceToPlayer =
                playerTransform != null
                    ? Vector2.Distance(transform.position, playerTransform.position)
                    : float.MaxValue;

            // Aggro Logic
            if (!_isChasing && distanceToPlayer <= detectionRange)
            {
                _isChasing = true;
            }
            else if (_isChasing && distanceToPlayer > stopChasingRange)
            {
                _isChasing = false;
                _startPos = transform.position; // Reset roam anchor to current position
                _roamTarget = GetNewRoamPoint();
            }

            if (_isChasing && playerTransform != null)
            {
                ChasePlayer();
            }
            else
            {
                RoamBehavior();
            }

            FlipSprite();
        }

        private void ChasePlayer()
        {
            Vector2 direction = (playerTransform.position - transform.position).normalized;

            // Using velocity smoothly instead of snapping position (fixes wall clipping)
            Vector2 targetVelocity = direction * moveSpeed;
            rb.linearVelocity = Vector2.Lerp(
                rb.linearVelocity,
                targetVelocity,
                Time.fixedDeltaTime * acceleration
            );
        }

        private void RoamBehavior()
        {
            float distToPoint = Vector2.Distance(transform.position, _roamTarget);

            if (distToPoint < 0.5f)
            {
                rb.linearVelocity = Vector2.Lerp(
                    rb.linearVelocity,
                    Vector2.zero,
                    Time.fixedDeltaTime * acceleration
                );
                _roamTimer += Time.fixedDeltaTime;

                if (_roamTimer >= waitTimeAtPoint)
                {
                    _roamTarget = GetNewRoamPoint();
                    _roamTimer = 0;
                }
            }
            else
            {
                Vector2 direction = (_roamTarget - (Vector2)transform.position).normalized;
                rb.linearVelocity = Vector2.Lerp(
                    rb.linearVelocity,
                    direction * roamSpeed,
                    Time.fixedDeltaTime * acceleration
                );
            }
        }

        private Vector2 GetNewRoamPoint()
        {
            return _startPos + (Random.insideUnitCircle * roamRadius);
        }

        private void FlipSprite()
        {
            // Look at velocity direction instead of just player position
            if (rb.linearVelocity.x > 0.1f)
                _sprite.flipX = false;
            else if (rb.linearVelocity.x < -0.1f)
                _sprite.flipX = true;
        }

        private void OnDrawGizmosSelected()
        {
            // Visual aids for the editor
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, detectionRange);

            Gizmos.color = Color.yellow;
            if (!Application.isPlaying)
                _startPos = transform.position;
            Gizmos.DrawWireSphere(_startPos, roamRadius);
        }
    }
}
