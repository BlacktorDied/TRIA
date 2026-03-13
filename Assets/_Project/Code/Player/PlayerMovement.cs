using System.Collections;
using UnityEngine;

namespace TRIA.Player
{
    [RequireComponent(typeof(Rigidbody2D), typeof(Collider2D), typeof(PlayerAudio))]
    public class PlayerMovement : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField]
        private float walkSpeed = 8f;

        [Header("Jump Settings")]
        [SerializeField]
        private float jumpForce = 9f;

        [SerializeField]
        private int maxAirJumps = 0;

        [SerializeField]
        private float jumpCutMultiplier = 0.5f;

        [SerializeField]
        private float fallMultiplier = 2f;

        [Header("Jump Timers")]
        [SerializeField]
        private float coyoteTime = 0.1f;

        [SerializeField]
        private float jumpBufferTime = 0.1f;

        [Header("Dash Settings")]
        [SerializeField]
        private float dashSpeed = 20f;

        [SerializeField]
        private float dashTime = 0.15f;

        [SerializeField]
        private float dashCooldown = 0.6f;

        [Header("Ground Check")]
        [SerializeField]
        private Vector2 boxSize = new Vector2(0.8f, 0.05f);

        [SerializeField]
        private float castDistance = 0.1f;

        [SerializeField]
        private LayerMask groundLayer;

        private Rigidbody2D _rb;
        private Collider2D _col;
        private PlayerAudio _playerAudio;
        private Animator _anim;

        private Vector2 _moveInput;
        public bool IsGrounded { get; private set; }
        private bool _isDashing;

        private int _jumpsRemaining;
        private float _coyoteTimer;
        private float _jumpBufferTimer;
        private float _lastDashTime;
        private float _normalGravity;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _col = GetComponent<Collider2D>();
            _playerAudio = GetComponent<PlayerAudio>();
            _anim = GetComponent<Animator>();

            _normalGravity = _rb.gravityScale;
        }

        public void SetMoveInput(Vector2 input) => _moveInput = input;

        private void Update()
        {
            if (_isDashing)
                return;

            CheckGrounded();
            HandleTimers();
            HandleJump();
        }

        private void FixedUpdate()
        {
            if (_isDashing)
                return;

            ApplyMovement();
            ApplyVariableJump();
        }

        // Add this to PlayerMovement.cs
        public void OnPause()
        {
            // When you press ESC, the Player Input sends a message here.
            // We tell the GameManager to toggle the pause state.
            if (TRIA.Core.GameManager.Instance != null)
            {
                TRIA.Core.GameManager.Instance.TogglePause();
            }
        }

        #region Locomotion & Physics

        private void CheckGrounded()
        {
            Vector2 origin = new Vector2(_col.bounds.center.x, _col.bounds.min.y);
            IsGrounded = Physics2D.BoxCast(
                origin,
                boxSize,
                0f,
                Vector2.down,
                castDistance,
                groundLayer
            );

            if (IsGrounded)
                _jumpsRemaining = maxAirJumps;
        }

        private void ApplyMovement()
        {
            float x = _moveInput.x;
            _rb.linearVelocity = new Vector2(x * walkSpeed, _rb.linearVelocity.y);

            _anim?.SetBool("isWalking", Mathf.Abs(x) > 0.1f && IsGrounded);

            if (x > 0f)
                transform.localScale = new Vector3(1f, 0.875f, 1f);
            else if (x < 0f)
                transform.localScale = new Vector3(-1f, 0.875f, 1f);

            if (Mathf.Abs(x) > 0.1f && IsGrounded)
                _playerAudio?.StartFootsteps();
            else
                _playerAudio?.StopFootsteps();
        }

        #endregion

        #region Jumping

        public void OnJumpPressed() => _jumpBufferTimer = jumpBufferTime;

        public void OnJumpReleased()
        {
            if (_rb.linearVelocity.y > 0)
                _rb.linearVelocity = new Vector2(
                    _rb.linearVelocity.x,
                    _rb.linearVelocity.y * jumpCutMultiplier
                );
        }

        private void HandleTimers()
        {
            _coyoteTimer = IsGrounded ? coyoteTime : _coyoteTimer - Time.deltaTime;
            _jumpBufferTimer -= Time.deltaTime;
        }

        private void HandleJump()
        {
            if (_jumpBufferTimer <= 0f)
                return;

            if (IsGrounded || _coyoteTimer > 0f)
            {
                PerformJump();
            }
            else if (_jumpsRemaining > 0)
            {
                PerformJump();
                _jumpsRemaining--;
            }
        }

        private void PerformJump()
        {
            _jumpBufferTimer = 0f;
            _coyoteTimer = 0f;
            _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, jumpForce);
            _playerAudio?.PlayJump();
        }

        private void ApplyVariableJump()
        {
            if (_rb.linearVelocity.y < 0)
                _rb.linearVelocity +=
                    Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
        }

        #endregion

        #region Dashing

        public void OnDashPressed()
        {
            if (!_isDashing && Time.time >= _lastDashTime + dashCooldown)
                StartCoroutine(DashRoutine());
        }

        private IEnumerator DashRoutine()
        {
            _isDashing = true;
            _lastDashTime = Time.time;
            _playerAudio?.PlayDash();
            _playerAudio?.StopFootsteps();
            _anim?.SetBool("isWalking", false);

            float xDir = _moveInput.x;
            if (Mathf.Abs(xDir) < 0.01f)
                xDir = Mathf.Sign(transform.localScale.x);

            _rb.gravityScale = 0f;
            _rb.linearVelocity = new Vector2(xDir * dashSpeed, 0f);

            yield return new WaitForSeconds(dashTime);

            _rb.gravityScale = _normalGravity;
            _isDashing = false;
        }

        #endregion
    }
}
