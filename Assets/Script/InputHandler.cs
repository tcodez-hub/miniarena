using System;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    private InputSystem_Actions inputActions;

    public Vector2 PlayerMovementInput_1 { get; private set; }
    public Vector2 PlayerMovementInput_2 { get; private set; }

    public Action OnAttackPressed_1;
    public Action OnAttackPressed_2;



    private void Awake()
    {
        inputActions = new InputSystem_Actions();
        inputActions.Enable();
    }

    private void OnEnable()
    {
        inputActions.Player_1.Enable();
        inputActions.Player_2.Enable();

        inputActions.Player_1.Move.performed += ctx => PlayerMovementInput_1 = ctx.ReadValue<Vector2>();
        inputActions.Player_1.Move.canceled += ctx => PlayerMovementInput_1 = Vector2.zero;
        inputActions.Player_1.Attack.performed += ctx => OnAttackPressed_1?.Invoke();
        inputActions.Player_2.Move.performed += ctx => PlayerMovementInput_2 = ctx.ReadValue<Vector2>();
        inputActions.Player_2.Move.canceled += ctx => PlayerMovementInput_2 = Vector2.zero;
        inputActions.Player_2.Attack.performed += ctx => OnAttackPressed_2?.Invoke();

    }
    private void OnDisable()
    {
        inputActions.Player_1.Move.performed -= ctx => PlayerMovementInput_1 = ctx.ReadValue<Vector2>();
        inputActions.Player_1.Move.canceled -= ctx => PlayerMovementInput_1 = Vector2.zero;
        inputActions.Player_1.Attack.performed -= ctx => OnAttackPressed_1?.Invoke();
        inputActions.Player_2.Move.performed -= ctx => PlayerMovementInput_2 = ctx.ReadValue<Vector2>();
        inputActions.Player_2.Move.canceled -= ctx => PlayerMovementInput_2 = Vector2.zero;
        inputActions.Player_2.Attack.performed -= ctx => OnAttackPressed_2?.Invoke();

        inputActions.Player_1.Disable();
        inputActions.Player_2.Disable();
    }
    private void OnDestroy()
    {
        inputActions.Disable();
    }
}
