using System.Collections;
using UnityEngine;

namespace TRIA.Enemies
{
    public class JumpingEnemy : Enemy
    {
        [Header("Patrol Settings")]
        [SerializeField]
        private float jumpDistance = 3f; // Length of a single jump

        [SerializeField]
        private int jumpsInOneDirection = 3; // How many jumps before turning

        [SerializeField]
        private float jumpCooldown = 0.5f; // Time between jumps

        [SerializeField]
        private float endpointWaitTime = 1.5f; // Delay when turning around

        [Header("Physics")]
        [SerializeField]
        private float jumpHeight = 3f;

        [Header("Detection")]
        [SerializeField]
        private LayerMask groundLayer;

        [SerializeField]
        private float groundCheckWidth = 0.8f;

        [SerializeField]
        private float groundCheckHeight = 0.1f;

        private int _currentJumpCount = 0;
        private int _direction = 1; // 1 = Right, -1 = Left
        private float _nextJumpTime;
        private bool _isGrounded;
        private bool _isWaitingAtEnd;
        private SpriteRenderer _sprite;

        protected override void Start()
        {
            base.Start();
            _sprite = GetComponent<SpriteRenderer>();
        }

        private void Update()
        {
            if (isDead)
            {
                rb.linearVelocity = Vector2.zero;
                return;
            }

            CheckGrounded();

            // Only try to jump if grounded, not currently waiting, and cooldown is over
            if (_isGrounded && !_isWaitingAtEnd && Time.time >= _nextJumpTime)
            {
                PerformPreciseJump();
            }

            if (_sprite != null)
                _sprite.flipX = _direction < 0;
        }

        private void PerformPreciseJump()
        {
            // MATH: Calculate velocities based on gravity for 100% precision
            float gravity = Mathf.Abs(Physics2D.gravity.y * rb.gravityScale);

            // 1. Vertical velocity to reach exactly jumpHeight
            float vy = Mathf.Sqrt(2 * gravity * jumpHeight);

            // 2. Total time spent in the air (Total trajectory)
            float timeInAir = 2 * (vy / gravity);

            // 3. Horizontal velocity to reach exactly jumpDistance in that time
            float vx = (jumpDistance / timeInAir) * _direction;

            // Apply velocity
            rb.linearVelocity = new Vector2(vx, vy);

            // Update jump counting
            _currentJumpCount++;
            _nextJumpTime = Time.time + jumpCooldown;

            // Check if we reached the end of the patrol
            if (_currentJumpCount >= jumpsInOneDirection)
            {
                StartCoroutine(WaitAtEndpoint());
            }
        }

        private IEnumerator WaitAtEndpoint()
        {
            _isWaitingAtEnd = true;
            _currentJumpCount = 0;

            yield return new WaitForSeconds(endpointWaitTime);

            _direction *= -1; // Turn around
            _isWaitingAtEnd = false;
        }

        private void CheckGrounded()
        {
            if (_sprite == null)
                return;
            Vector2 checkPos = new Vector2(_sprite.bounds.center.x, _sprite.bounds.min.y);
            _isGrounded = Physics2D.OverlapBox(
                checkPos,
                new Vector2(groundCheckWidth, groundCheckHeight),
                0,
                groundLayer
            );
        }

        public override void TakeDamage(float amount)
        {
            base.TakeDamage(amount);
            StartCoroutine(HitFlash());
        }

        private IEnumerator HitFlash()
        {
            _sprite.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            _sprite.color = Color.white;
        }

        private void OnDrawGizmosSelected()
        {
            if (_sprite == null)
                _sprite = GetComponent<SpriteRenderer>();
            if (_sprite == null)
                return;

            // Visualize ground check
            Gizmos.color = _isGrounded ? Color.green : Color.red;
            Vector2 checkPos = new Vector2(_sprite.bounds.center.x, _sprite.bounds.min.y);
            Gizmos.DrawWireCube(checkPos, new Vector3(groundCheckWidth, groundCheckHeight, 0));

            // Visualize jump distance prediction
            Gizmos.color = Color.yellow;
            Vector3 start = transform.position;
            Vector3 end = start + new Vector3(jumpDistance * _direction, 0, 0);
            Gizmos.DrawLine(start, end);
            Gizmos.DrawWireSphere(end, 0.2f);
        }
    }
}
