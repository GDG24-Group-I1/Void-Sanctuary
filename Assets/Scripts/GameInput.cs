using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class GameInput : MonoBehaviour, IDataPersistence
{
    private VoidSanctuaryActions playerInputActions;

    private InputAction _attackAction;
    private InputAction _fireAction;
    private InputAction _blockAction;
    private InputAction _runAction;
    private InputAction _drawWeaponAction;
    private InputAction _dashAction;
    private InputAction _fakeHitAction;
    public bool IsKeyboardMovement { get; private set; }
    public bool HoldDownToRun { get; private set; } = true;

    private GameObject pauseMenu;


    public void LoadData(GameData data)
    {
        HoldDownToRun = data.savedSettings.holdDownToRun;
    }

    
    public void SaveData(GameData data)
    {
        data.savedSettings.holdDownToRun = HoldDownToRun;
    }

    private void Start()
    {
        pauseMenu = GameObjectExtensions.FindInactive("PauseMenu", "Canvas");
        if (pauseMenu != null)
        {
            pauseMenu.SetActive(false);
        }
    }

    private void ChangePauseState()
    {
        if (Time.timeScale == 0)
            Time.timeScale = 1;
        else
            Time.timeScale = 0;
        if (pauseMenu.activeSelf)
        {
            playerInputActions.Player.Enable();
            playerInputActions.MenuActionMap.Disable();
        }
        else
        {
            playerInputActions.Player.Disable();
            playerInputActions.MenuActionMap.Enable();
        }
        pauseMenu.SetActive(!pauseMenu.activeSelf);
        if (pauseMenu.activeSelf)
        {
            pauseMenu.GetComponentInChildren<UnityEngine.UI.Toggle>().isOn = HoldDownToRun;
        }
    }

    private void Awake()
    {
        playerInputActions = new VoidSanctuaryActions();
        playerInputActions.Enable();
        playerInputActions.MenuActionMap.Disable();
        playerInputActions.MenuActionMap.GoBackButton.performed += (context) =>
        {
            // TODO: implement "go back" mechanism
            // currently there is no nested menu so just unpause the game
            ChangePauseState();
        };
        playerInputActions.ControlsActionMap.ExitGameAction.performed += (context) =>
        {
            ChangePauseState();
        };
        playerInputActions.ControlsActionMap.PauseGameAction.performed += (context) =>
        {
            ChangePauseState();
        };
        playerInputActions.Player.Move.performed += (context) =>
        {
            if (context.control.device is Keyboard or Mouse) IsKeyboardMovement = true;
            else IsKeyboardMovement = false;
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

    public Vector2 GetMousePosition()
    {
        Vector2 mousePosition = playerInputActions.Player.MousePosition.ReadValue<Vector2>();

        return mousePosition;
    }

    public Action<CallbackContext> OnAttack
    {
        set { _attackAction.performed += value; }
    }

    public Action<CallbackContext> OnAim
    {
        set { _fireAction.performed += value; }
    }

    public Action<CallbackContext> OnFire
    {
        set { _fireAction.canceled += value; }
    }

    public Action<CallbackContext> OnBlock
    {
        set { _blockAction.performed += value; }
    }

    public Action<CallbackContext> OnRunStart
    {
        set { _runAction.performed += value; }
    }

    public Action<CallbackContext> OnRunEnd
    {
        set { _runAction.canceled += value; }
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

    public Action<CallbackContext> OnChangeCurrentSelectedControl
    {
        set { playerInputActions.MenuActionMap.ChangeCurrentSelectedControl.performed += value; }
    }

    public Action<CallbackContext> OnCurrentSelectedControlClick
    {
        set { playerInputActions.MenuActionMap.TriggerCurrentButton.performed += value; }
    }

    private void OnDestroy()
    {
        playerInputActions.Dispose();
    }

    public void OnExitGameButtonClicked()
    {
        Application.Quit();
    }

    public void OnHoldDownToRunChanged(bool value)
    {
        HoldDownToRun = value;
    }
}
