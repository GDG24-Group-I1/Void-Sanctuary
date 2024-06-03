using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class GameInput : MonoBehaviour
{
    private VoidSanctuaryActions playerInputActions;

    private InputAction _attackAction;
    private InputAction _fireAction;
    private InputAction _blockAction;
    private InputAction _dashAction;


    private void Awake()
    {
        playerInputActions = new VoidSanctuaryActions();
        playerInputActions.Enable();

        var exitGameAction = playerInputActions.FindAction("ExitGameAction");
        exitGameAction.performed += (context) =>
        {
            Application.Quit();
        };

        _attackAction = playerInputActions.FindAction("Attack");

        _fireAction = playerInputActions.FindAction("Fire");

        _blockAction = playerInputActions.FindAction("Block");

        _dashAction = playerInputActions.FindAction("Dash");

    }
    public Vector2 GetMovementVectorNormalized()
    {
        Vector2 inputVector = playerInputActions.Player.Move.ReadValue<Vector2>();

        inputVector = inputVector.normalized;

        return inputVector;
    }

    public Action<CallbackContext> OnAttack
    {
        set { _attackAction.performed += value; }
    }

    public Action<CallbackContext> OnAim
    {
        set { _fireAction.started += value; }
    }

    public Action<CallbackContext> OnFire
    {
        set { _fireAction.canceled += value; }
    }

    public Action<CallbackContext> OnBlock
    {
        set { _blockAction.performed += value; }
    }

    public Action<CallbackContext> OnDash
    {
        set { _dashAction.performed += value; }
    }
}
