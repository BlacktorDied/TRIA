using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    private Controls input;  // your generated input class

    public Vector2 MoveInput { get; private set; }
    public bool JumpPressed { get; private set; }
    public bool JumpHeld { get; private set; }

    private void Awake()
    {
        input = new Controls();
    }

    private void OnEnable()
    {
        input.Enable();

        // Movement
        input.Player.Move.performed += OnMove;
        input.Player.Move.canceled += OnMove;

        // Jump
        input.Player.Jump.performed += ctx => JumpPressed = true;
        input.Player.Jump.canceled += ctx => JumpHeld = false;


    }

    private void OnDisable()
    {
        // Movement
        input.Player.Move.performed -= OnMove;
        input.Player.Move.canceled -= OnMove;

        // Jump
        input.Player.Jump.performed -= OnJump;


        input.Disable();
    }

    private void OnMove(InputAction.CallbackContext ctx)
    {
        MoveInput = ctx.ReadValue<Vector2>();
    }

    private void OnJump(InputAction.CallbackContext ctx)
    {
        JumpPressed = true;
    }



    private void LateUpdate()
    {
        JumpPressed = false;
        JumpHeld = input.Player.Jump.IsPressed();
    }
}
