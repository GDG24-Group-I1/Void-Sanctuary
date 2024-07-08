using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static UnityEngine.InputSystem.InputAction;

public class GameInput : MonoBehaviour, IDataPersistence
{
    private VoidSanctuaryActions playerInputActions;
    public bool IsKeyboardMovement { get; private set; }
    public bool HoldDownToRun { get; private set; } = true;
    public bool SlowDownAttack { get; private set; } = true;
    public bool DrawDebugRays { get; private set; } = false;

    public Observable<ControlType> CurrentControl { get; } = ControlType.Mouse;

    private GameObject pauseMenu;
    private GameObject dialogBox;
    private Image psControllerImage;
    private Image xboxControllerImage;


    public void LoadData(GameData data)
    {
        HoldDownToRun = data.savedSettings.holdDownToRun;
        SlowDownAttack = data.savedSettings.slowDownAttack;
        DrawDebugRays = data.savedSettings.drawDebugRays;
    }

    
    public void SaveData(GameData data)
    {
        data.savedSettings.holdDownToRun = HoldDownToRun;
        data.savedSettings.slowDownAttack = SlowDownAttack;
        data.savedSettings.drawDebugRays = DrawDebugRays;
    }

    private void Start()
    {
        pauseMenu = GameObjectExtensions.FindInactive("PauseMenu", "GameUI");
        dialogBox = GameObjectExtensions.FindInactive("DialogBox", "GameUI");
        if (pauseMenu != null)
        {
            pauseMenu.SetActive(false);
            var images = pauseMenu.GetComponentsInChildren<Image>(true);
            psControllerImage = images.First(x => x.name == "PSController");
            xboxControllerImage = images.First(x => x.name == "XboxController");
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
        dialogBox.SetActive(!dialogBox.activeSelf);
        if (pauseMenu.activeSelf)
        {
            pauseMenu.GetComponentInChildren<Toggle>().isOn = HoldDownToRun;
            switch (CurrentControl.value)
            {
                case ControlType.Mouse:
                    psControllerImage.color = psControllerImage.color.CopyWithAlpha(0.3f);
                    xboxControllerImage.color = xboxControllerImage.color.CopyWithAlpha(0.3f);
                    break;
                case ControlType.PSController:
                    psControllerImage.color = psControllerImage.color.CopyWithAlpha(1f);
                    xboxControllerImage.color = xboxControllerImage.color.CopyWithAlpha(0.3f);
                    break;
                case ControlType.OtherController:
                case ControlType.XboxController:
                    psControllerImage.color = psControllerImage.color.CopyWithAlpha(0.3f);
                    xboxControllerImage.color = xboxControllerImage.color.CopyWithAlpha(1f);
                    break;
            }
        }
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

    public void OnExitGameButtonClicked()
    {
        Application.Quit();
    }

    public void OnHoldDownToRunChanged(bool value)
    {
        HoldDownToRun = value;
    }

    public void OnSlowDownAttackChanged(bool value)
    {
        SlowDownAttack = value;
    }

    public void OnDrawDebugRaysChanged(bool value)
    {
        DrawDebugRays = value;
    }
}
