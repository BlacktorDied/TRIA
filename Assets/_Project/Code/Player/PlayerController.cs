using UnityEngine;
using UnityEngine.InputSystem;

namespace TRIA.Player
{
    [RequireComponent(typeof(PlayerMovement), typeof(PlayerCombat))]
    public class PlayerController : MonoBehaviour
    {
        private PlayerMovement _movement;
        private PlayerCombat _combat;
        private Vector2 _moveInput;

        private void Awake()
        {
            _movement = GetComponent<PlayerMovement>();
            _combat = GetComponent<PlayerCombat>();
        }

        public void OnMove(InputValue value)
        {
            _moveInput = value.Get<Vector2>();
            _movement.SetMoveInput(_moveInput);
            _combat.SetMoveInput(_moveInput); // Needed for Up/Down attacks
        }

        public void OnJump(InputValue value)
        {
            if (value.isPressed)
                _movement.OnJumpPressed();
            else
                _movement.OnJumpReleased();
        }

        public void OnDash(InputValue value)
        {
            if (value.isPressed)
                _movement.OnDashPressed();
        }

        public void OnAttack(InputValue value)
        {
            if (value.isPressed)
                _combat.AttemptAttack();
        }
    }
}
