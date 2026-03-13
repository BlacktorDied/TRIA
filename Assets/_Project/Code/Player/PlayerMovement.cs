using System.Collections;
using TRIA.Core;
using UnityEngine;

namespace TRIA.Player
{
    [RequireComponent(typeof(Rigidbody2D), typeof(Collider2D), typeof(SpriteRenderer))]
    public class PlayerMovement : MonoBehaviour
    {
        [Header("Walking")]
        [SerializeField]
        private float walkSpeed = 8f;

        [SerializeField]
        private float acceleration = 50f;

        [SerializeField]
        private float deceleration = 50f;

        [Header("Jumping")]
        [SerializeField]
        private float jumpForce = 12f;

        [SerializeField]
        private int maxAirJumps = 1;

        [SerializeField]
        private float fallMultiplier = 3f;

        [SerializeField]
        private float jumpCutMultiplier = 0.5f;

        [Header("Timers")]
        [SerializeField]
        private float coyoteTime = 0.15f;

        [SerializeField]
        private float jumpBufferTime = 0.15f;

        [Header("Dashing")]
        [SerializeField]
        private float dashSpeed = 25f;

        [SerializeField]
        private float dashDuration = 0.2f;

        [SerializeField]
        private float dashCooldown = 1f;

        [Header("Knockback")]
        [SerializeField]
        private float knockbackDuration = 0.2f;

        [Header("Ground Check (Automatic)")]
        [SerializeField]
        private Vector2 boxSize = new Vector2(0.6f, 0.1f);

        [SerializeField]
        private float groundSkin = 0.05f;

        [SerializeField]
        private LayerMask groundLayer;

        // State Properties
        public bool IsGrounded { get; private set; }
        public bool IsDashing { get; private set; }
        public bool IsKnockedBack { get; private set; }

        // Components
        private Rigidbody2D _rb;
        private Collider2D _col;
        private Animator _anim;
        private SpriteRenderer _sprite;
        private PlayerAudio _audio;
        private PlayerAbilityHandler _abilities; // Added Reference

        // Internal Variables
        private Vector2 _moveInput;
        private int _airJumpsRemaining;
        private float _coyoteTimer;
        private float _jumpBufferTimer;
        private float _lastDashTime;
        private float _knockbackTimer;
        private float _originalGravity;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _col = GetComponent<Collider2D>();
            _anim = GetComponent<Animator>();
            _sprite = GetComponent<SpriteRenderer>();
            _audio = GetComponent<PlayerAudio>();
            _abilities = GetComponent<PlayerAbilityHandler>(); // Initialize Reference
            _originalGravity = _rb.gravityScale;
        }

        public void SetMoveInput(Vector2 input) => _moveInput = input;

        private void Update()
        {
            if (
                GameManager.Instance != null
                && GameManager.Instance.CurrentState == GameState.Pause
            )
                return;

            HandleTimers();
            CheckGround();
            HandleGravity();
            FlipSprite();

            // Animator Sync
            _anim?.SetBool("isJumping", !IsGrounded);
        }

        private void FixedUpdate()
        {
            if (IsDashing || IsKnockedBack)
                return;
            ApplyMovement();
        }

        public void OnPause()
        {
            if (GameManager.Instance != null)
                GameManager.Instance.TogglePause();
        }

        private void CheckGround()
        {
            Vector2 checkPos = new Vector2(
                transform.position.x,
                _col.bounds.min.y - (boxSize.y / 2) + groundSkin
            );
            bool wasGrounded = IsGrounded;
            IsGrounded = Physics2D.OverlapBox(checkPos, boxSize, 0, groundLayer);

            if (IsGrounded)
            {
                _coyoteTimer = coyoteTime;
                _airJumpsRemaining = maxAirJumps;
            }

            if (IsGrounded && !wasGrounded && _jumpBufferTimer > 0)
                PerformJump();
        }

        private void ApplyMovement()
        {
            float targetSpeed = _moveInput.x * walkSpeed;
            float speedDif = targetSpeed - _rb.linearVelocity.x;
            float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? acceleration : deceleration;
            float movement = speedDif * accelRate;

            _rb.AddForce(movement * Vector2.right, ForceMode2D.Force);

            if (IsGrounded && Mathf.Abs(_moveInput.x) > 0.1f)
                _audio?.StartFootsteps();
            else
                _audio?.StopFootsteps();

            _anim?.SetBool("isWalking", Mathf.Abs(_moveInput.x) > 0.1f);
        }

        private void HandleTimers()
        {
            _coyoteTimer -= Time.deltaTime;
            _jumpBufferTimer -= Time.deltaTime;

            if (IsKnockedBack)
            {
                _knockbackTimer -= Time.deltaTime;
                if (_knockbackTimer <= 0)
                    IsKnockedBack = false;
            }
        }

        private void FlipSprite()
        {
            if (IsKnockedBack)
                return;
            if (_moveInput.x > 0)
                _sprite.flipX = false;
            else if (_moveInput.x < 0)
                _sprite.flipX = true;
        }

        public void OnJumpPressed()
        {
            _jumpBufferTimer = jumpBufferTime;
            if (_coyoteTimer > 0 || _airJumpsRemaining > 0)
                PerformJump();
        }

        public void OnJumpReleased()
        {
            if (_rb.linearVelocity.y > 0)
                _rb.linearVelocity = new Vector2(
                    _rb.linearVelocity.x,
                    _rb.linearVelocity.y * jumpCutMultiplier
                );
        }

        private void PerformJump()
        {
            if (!IsGrounded)
                _airJumpsRemaining--;
            _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, jumpForce);
            _jumpBufferTimer = 0;
            _coyoteTimer = 0;
            _audio?.PlayJump();
            // _anim?.SetTrigger("Jump"); // Optional if using Bools
        }

        private void HandleGravity()
        {
            if (IsDashing)
                return;
            _rb.gravityScale =
                (_rb.linearVelocity.y < 0) ? _originalGravity * fallMultiplier : _originalGravity;
        }

        public void OnDashPressed()
        {
            // ABILITY CHECK: Only dash if the Dash ability is unlocked in the handler
            if (_abilities != null && !_abilities.HasAbility(AbilityType.Dash))
            {
                Debug.Log("Dash not yet unlocked!");
                return;
            }

            if (IsDashing || Time.time < _lastDashTime + dashCooldown)
                return;
            StartCoroutine(DashRoutine());
        }

        private IEnumerator DashRoutine()
        {
            IsDashing = true;
            _lastDashTime = Time.time;
            _audio?.PlayDash();
            _audio?.StopFootsteps();

            float dashDir = _sprite.flipX ? -1f : 1f;
            _rb.gravityScale = 0;
            _rb.linearVelocity = new Vector2(dashDir * dashSpeed, 0);

            yield return new WaitForSeconds(dashDuration);

            _rb.gravityScale = _originalGravity;
            IsDashing = false;
        }

        public void ApplyKnockback(Vector2 direction, float force)
        {
            if (IsDashing)
                return;
            IsKnockedBack = true;
            _knockbackTimer = knockbackDuration;
            _rb.linearVelocity = Vector2.zero;
            _rb.AddForce(direction.normalized * force, ForceMode2D.Impulse);
        }

        private void OnDrawGizmos()
        {
            if (_col == null)
                _col = GetComponent<Collider2D>();
            Gizmos.color = Color.green;
            Vector2 checkPos = new Vector2(
                transform.position.x,
                _col.bounds.min.y - (boxSize.y / 2) + groundSkin
            );
            Gizmos.DrawWireCube(checkPos, boxSize);
        }
    }
}
