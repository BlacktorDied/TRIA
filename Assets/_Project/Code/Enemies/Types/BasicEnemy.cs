using UnityEngine;

namespace TRIA.Enemies
{
    public class BasicEnemy : Enemy
    {
        [Header("Patrol Settings")]
        [SerializeField]
        private float speed = 2.5f;

        [SerializeField]
        private float patrolDistance = 4f;

        [Header("Wall Detection")]
        [SerializeField]
        private float wallCheckDistance = 0.2f;

        [SerializeField]
        private float shellOffset = 0.6f; // Adjust this so the red line starts outside the body

        [SerializeField]
        private LayerMask groundLayer;

        private Vector2 _startPos;
        private int _direction = 1; // 1 = Right, -1 = Left
        private SpriteRenderer _sprite;
        private float _lastFlipTime;
        private const float FLIP_COOLDOWN = 0.2f;

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
                rb.linearVelocity = Vector2.zero;
                return;
            }

            HandleMovement();
        }

        private void HandleMovement()
        {
            float distanceFromStart = transform.position.x - _startPos.x;

            if (Time.time > _lastFlipTime + FLIP_COOLDOWN)
            {
                // Flip if we hit the distance limit
                if (distanceFromStart >= patrolDistance && _direction == 1)
                {
                    Flip();
                }
                else if (distanceFromStart <= -patrolDistance && _direction == -1)
                {
                    Flip();
                }
                // OR flip if we hit a wall
                else if (IsHittingWall())
                {
                    Flip();
                }
            }

            // Apply movement (keep Y velocity for gravity)
            rb.linearVelocity = new Vector2(_direction * speed, rb.linearVelocity.y);

            // Sync visuals
            if (_sprite != null)
                _sprite.flipX = _direction < 0;
        }

        private void Flip()
        {
            _direction *= -1;
            _lastFlipTime = Time.time;
        }

        private bool IsHittingWall()
        {
            // Start the ray in front of the enemy center to avoid self-collision
            Vector2 rayOrigin = new Vector2(
                transform.position.x + (shellOffset * _direction),
                transform.position.y
            );

            RaycastHit2D hit = Physics2D.Raycast(
                rayOrigin,
                Vector2.right * _direction,
                wallCheckDistance,
                groundLayer
            );

            // Visual Debug: Red line in Scene View shows the wall check
            Debug.DrawRay(rayOrigin, Vector2.right * _direction * wallCheckDistance, Color.red);

            return hit.collider != null;
        }

        private void OnDrawGizmosSelected()
        {
            if (!Application.isPlaying)
                _startPos = transform.position;

            Gizmos.color = Color.cyan;
            Vector3 leftLimit = new Vector3(_startPos.x - patrolDistance, _startPos.y, 0);
            Vector3 rightLimit = new Vector3(_startPos.x + patrolDistance, _startPos.y, 0);

            Gizmos.DrawLine(leftLimit, rightLimit);
            Gizmos.DrawWireSphere(leftLimit, 0.2f);
            Gizmos.DrawWireSphere(rightLimit, 0.2f);
        }
    }
}
