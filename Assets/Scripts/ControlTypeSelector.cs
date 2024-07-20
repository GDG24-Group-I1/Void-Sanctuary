using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControlTypeSelector : MonoBehaviour
{
    [SerializeField] private GameObject xboxControllerView;
    [SerializeField] private GameObject keyboardMouseView;
    [SerializeField] private GameObject psControllerView;


    // FIXME: this solution is not ideal. find a better way to detect input type changes in the main menu without having to port the entire input handler
    void OnActionChange(object obj, InputActionChange change)
    {
        if (change == InputActionChange.ActionPerformed && obj is InputAction action) {
            if (action.name == "Navigate") return;
            if (action.activeControl.device is Keyboard or Mouse)
            {
                SwitchShowedControls(ControlType.Mouse);
            }
            else
            {
                if (action.activeControl.device.displayName.Contains("DualSense") || action.activeControl.device.displayName.Contains("DualShock"))
                {
                    SwitchShowedControls(ControlType.PSController);
                }
                else
                {
                    SwitchShowedControls(ControlType.XboxController);
                }
            }
        }
    }

    ControlType GetControlType(InputDevice device)
    {
        if (device is Keyboard or Mouse)
        {
            return ControlType.Mouse;
        }
        else
        {
            if (device.displayName.Contains("DualSense") || device.displayName.Contains("DualShock"))
            {
                return ControlType.PSController;
            }
            else
            {
                return ControlType.XboxController;
            }
        }
    }

    void SwitchShowedControls(ControlType type)
    {
        xboxControllerView.SetActive(type == ControlType.XboxController || type == ControlType.OtherController);
        keyboardMouseView.SetActive(type == ControlType.Mouse);
        psControllerView.SetActive(type == ControlType.PSController);
    }

    void OnEnable()
    {
        if (!xboxControllerView.activeSelf && !psControllerView.activeSelf && !keyboardMouseView.activeSelf)
        {
            SwitchShowedControls(GetControlType(InputSystem.devices.Last()));
        }
        InputSystem.onActionChange += OnActionChange;
    }

    void OnDisable()
    {
        InputSystem.onActionChange -= OnActionChange;
    }
}
