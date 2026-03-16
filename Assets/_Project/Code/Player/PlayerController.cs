using UnityEngine;
using UnityEngine.InputSystem;

namespace TRIA.Player
{
    // Updated requirement: We now use PlayerAttack instead of PlayerCombat
    [RequireComponent(typeof(PlayerMovement), typeof(PlayerAttack))]
    public class PlayerController : MonoBehaviour
    {
        private PlayerMovement _movement;
        private PlayerAttack _attack;
        private Vector2 _moveInput;

        private void Awake()
        {
            _movement = GetComponent<PlayerMovement>();
            _attack = GetComponent<PlayerAttack>();
        }

        // Called by PlayerInput (Send Messages / Broadcast Messages)
        public void OnMove(InputValue value)
        {
            _moveInput = value.Get<Vector2>();

            // Tell the movement script where we want to go
            _movement.SetMoveInput(_moveInput);

            // Note: We no longer need to send input to the Attack script
            // because we removed top/bottom attacks!
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
            // If the button is pressed, trigger the attack logic
            if (value.isPressed)
            {
                _attack.OnAttack();
            }
        }
    }
}
