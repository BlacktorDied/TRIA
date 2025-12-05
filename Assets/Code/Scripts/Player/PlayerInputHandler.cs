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

    #endregion

    #region Unity Methods

    private void Awake()
    {
        input ??= new Controls();
    }

    private void OnEnable()
    {
        input.Enable();

        // Movement
        input.Player.Move.performed += OnMove;
        input.Player.Move.canceled += OnMove;

        // Jump
        input.Player.Jump.performed += OnJump;
        input.Player.Jump.canceled += OnJumpCanceled;
    }

    private void OnDisable()
    {
        // Movement
        input.Player.Move.performed -= OnMove;
        input.Player.Move.canceled -= OnMove;

        // Jump
        input.Player.Jump.performed -= OnJump;
        input.Player.Jump.canceled -= OnJumpCanceled;


        input.Disable();
    }

    #endregion

    #region Input Callbacks

    private void OnMove(InputAction.CallbackContext ctx)
    {
        MoveInput = ctx.ReadValue<Vector2>();
    }

    private void OnJump(InputAction.CallbackContext ctx)
    {
        JumpPressed = true;
        JumpHeld = true;
    }

    private void OnJumpCanceled(InputAction.CallbackContext ctx)
    {
        JumpHeld = false;
    }

    #endregion

    private void LateUpdate()
    {
        JumpPressed = false;
        JumpHeld = input.Player.Jump.IsPressed();
    }
}
