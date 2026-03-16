using TRIA.Core;
using UnityEngine;

namespace TRIA.Enemies
{
    public class BasicFollowEnemy : Enemy
    {
        [Header("Movement Settings")]
        [SerializeField]
        private float patrolSpeed = 2.5f;

        [SerializeField]
        private float chaseSpeed = 4.5f;

        [SerializeField]
        private float patrolDistance = 5f;

        [SerializeField]
        private float waitTimeAtEdge = 1.5f;

        [Header("Detection (Aggro)")]
        [SerializeField]
        private float detectionRange = 6f;

        [SerializeField]
        private float stopFollowingRange = 10f;

        private Vector2 _startPos;
        private int _direction = 1; // 1 = Right, -1 = Left
        private bool _isChasing;
        private bool _isWaiting;
        private float _waitTimer;
        private SpriteRenderer _sprite;

        protected override void Start()
        {
            base.Start();
            _sprite = GetComponent<SpriteRenderer>();
            _startPos = transform.position;
        }

        private void FixedUpdate()
        {
            if (isDead)
            {
                rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
                return;
            }

            HandleAggroLogic();

            if (_isChasing && playerTransform != null)
            {
                ChaseBehavior();
            }
            else
            {
                PatrolBehavior();
            }

            SyncVisuals();
        }

        private void HandleAggroLogic()
        {
            if (playerTransform == null)
                return;

            float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

            // Trigger Aggro
            if (!_isChasing && distanceToPlayer <= detectionRange)
            {
                _isChasing = true;
                _isWaiting = false; // Break out of wait if we see the player
            }
            // Drop Aggro
            else if (_isChasing && distanceToPlayer > stopFollowingRange)
            {
                _isChasing = false;
                // Optional: reset start position to here so they patrol the new area
                // _startPos = transform.position;
            }
        }

        private void ChaseBehavior()
        {
            // Simple horizontal chase
            _direction = playerTransform.position.x > transform.position.x ? 1 : -1;
            rb.linearVelocity = new Vector2(_direction * chaseSpeed, rb.linearVelocity.y);
        }

        private void PatrolBehavior()
        {
            if (_isWaiting)
            {
                rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
                _waitTimer -= Time.fixedDeltaTime;
                if (_waitTimer <= 0)
                    _isWaiting = false;
                return;
            }

            float distanceFromStart = transform.position.x - _startPos.x;

            // Check if we hit the boundaries
            if (
                (distanceFromStart >= patrolDistance && _direction == 1)
                || (distanceFromStart <= -patrolDistance && _direction == -1)
            )
            {
                _direction *= -1;
                _isWaiting = true;
                _waitTimer = waitTimeAtEdge;
                return;
            }

            rb.linearVelocity = new Vector2(_direction * patrolSpeed, rb.linearVelocity.y);
        }

        private void SyncVisuals()
        {
            if (_sprite != null && Mathf.Abs(rb.linearVelocity.x) > 0.1f)
            {
                _sprite.flipX = _direction < 0;
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (!Application.isPlaying)
                _startPos = transform.position;

            // Patrol Line
            Gizmos.color = Color.cyan;
            Vector3 left = new Vector3(_startPos.x - patrolDistance, _startPos.y, 0);
            Vector3 right = new Vector3(_startPos.x + patrolDistance, _startPos.y, 0);
            Gizmos.DrawLine(left, right);

            // Detection Zones
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, detectionRange);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, stopFollowingRange);
        }
    }
}
