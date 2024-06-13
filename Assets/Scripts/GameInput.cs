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
    private InputAction _runAction;
    private InputAction _drawWeaponAction;
    private InputAction _dashAction;
    private InputAction _fakeHitAction;


    private void Awake()
    {
        playerInputActions = new VoidSanctuaryActions();
        playerInputActions.Enable();

        playerInputActions.ControlsActionMap.ExitGameAction.performed += (context) =>
        {
            Application.Quit();
        };

        _attackAction = playerInputActions.Player.Attack;

        _fireAction = playerInputActions.Player.Fire;

        _blockAction = playerInputActions.Player.Block;

        _runAction = playerInputActions.Player.Run;
           
        _drawWeaponAction = playerInputActions.Player.DrawWeapon;
        _dashAction = playerInputActions.Player.Dash;
        _fakeHitAction = playerInputActions.Player.FakeHit;

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

    public Action<CallbackContext> OnRun
    {
        set { _runAction.performed += value; }
    }

    public Action<CallbackContext> OnDrawWeapon
    {
        set { _drawWeaponAction.performed += value; }
    }
    public Action<CallbackContext> OnDash
    {
        set { _dashAction.performed += value; }
    }
    public Action<CallbackContext> OnFakeHit
    {
        set { _fakeHitAction.performed += value; }
    }

    private void OnDestroy()
    {
        playerInputActions.Dispose();
    }
}
