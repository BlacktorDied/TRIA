using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    #region Variables

    private Controls input;

    public Vector2 MoveInput { get; private set; }
    public bool JumpPressed { get; private set; }
    public bool JumpHeld { get; private set; }
    public bool DashPressed { get; private set; }
    public bool AttackPressed { get; private set; }

    #endregion

    #region Unity Methods

    private void Awake() => input ??= new Controls();

    private void OnEnable()
    {
        input.Enable();
        input.Player.Move.performed += OnMove;
        input.Player.Move.canceled += OnMove;
        input.Player.Jump.started += OnJumpStarted;
        input.Player.Jump.canceled += OnJumpCanceled;
        input.Player.Dash.started += OnDashStarted;
        input.Player.Attack.started += OnAttackStarted;
    }

    private void OnDisable()
    {
        input.Player.Move.performed -= OnMove;
        input.Player.Move.canceled -= OnMove;
        input.Player.Jump.started -= OnJumpStarted;
        input.Player.Jump.canceled -= OnJumpCanceled;
        input.Player.Dash.started -= OnDashStarted;
        input.Player.Attack.started -= OnAttackStarted;
        input.Disable();
    }

    #endregion

    #region Input Callbacks

    private void OnMove(InputAction.CallbackContext ctx) => MoveInput = ctx.ReadValue<Vector2>();
    private void OnJumpStarted(InputAction.CallbackContext ctx) { JumpPressed = true; JumpHeld = true; }
    private void OnJumpCanceled(InputAction.CallbackContext ctx) => JumpHeld = false;
    private void OnDashStarted(UnityEngine.InputSystem.InputAction.CallbackContext ctx) => DashPressed = true;
    private void OnAttackStarted(InputAction.CallbackContext ctx) => AttackPressed = true;

    #endregion

    private void LateUpdate()
    {
        JumpPressed = false;
        JumpHeld = input.Player.Jump.IsPressed();
        AttackPressed = false;
        DashPressed = false;
    }

}
