using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static UnityEngine.InputSystem.InputAction;

public class GameInput : MonoBehaviour
{
    private VoidSanctuaryActions playerInputActions;
    public bool IsKeyboardMovement { get; private set; }
    private bool isPaused = false;

    public Observable<ControlType> CurrentControl { get; } = ControlType.Mouse;

    private GameObject pauseMenu;
    private GameObject dialogBox;

    public bool PauseCooldown { get; set; }

    private void Start()
    {
        isPaused = false;
        pauseMenu = GameObject.Find("PauseMenu");
        dialogBox = GameObject.Find("DialogBox");
        if (pauseMenu != null)
        {
            pauseMenu.SetActive(false);
        }
        PauseCooldown = false;
    }

    private void ChangePauseState()
    {
        if (PauseCooldown)
        {
            return;
        }
        if (Time.timeScale == 0)
            Time.timeScale = 1;
        else
            Time.timeScale = 0;
        if (isPaused)
        {
            playerInputActions.Player.Enable();
            playerInputActions.MenuActionMap.Disable();
        }
        else
        {
            playerInputActions.Player.Disable();
            playerInputActions.MenuActionMap.Enable();
        }
        isPaused = !isPaused;
        var dialogHandler = dialogBox.GetComponent<DialogHandler>();
        if (isPaused)
        {
            dialogHandler.DismissDialogForced();
            pauseMenu.SetActive(true);
        } else
        {
            dialogHandler.RestoreUpdateMode();
        }
        pauseMenu.GetComponentInChildren<MenuButtonSelector>().TogglePauseMenu(isPaused, CurrentControl.value);
        PauseCooldown = true;
    }

    private void Awake()
    {
        IsKeyboardMovement = true;
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
            if (context.control.device is Keyboard or Mouse) { 
                IsKeyboardMovement = true; 
                CurrentControl.Value = ControlType.Mouse;
            }
            else { 
                IsKeyboardMovement = false; 
                if (context.control.device.displayName.Contains("DualSense"))
                {
                    CurrentControl.Value = ControlType.PSController;
                } else
                {
                    CurrentControl.Value = ControlType.XboxController;
                }
            }

        };

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

    public void RegisterPlayer(VoidSanctuaryActions.IPlayerActions player)
    {
        playerInputActions.Player.SetCallbacks(player);
    }

    public void UnregisterPlayer(VoidSanctuaryActions.IPlayerActions player)
    {
        playerInputActions.Player.RemoveCallbacks(player);
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
}
