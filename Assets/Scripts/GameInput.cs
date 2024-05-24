using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInput : MonoBehaviour
{
    private VoidSanctuaryActions playerInputActions;

    private void Awake()
    {
        playerInputActions = new VoidSanctuaryActions();
        playerInputActions.Enable();

        var exitGameAction = playerInputActions.FindAction("ExitGameAction");
        exitGameAction.performed += (context) =>
        {
            Application.Quit();
        };

    }
    public Vector2 GetMovementVectorNormalized()
    {
        Vector2 inputVector = playerInputActions.Player.Move.ReadValue<Vector2>();

        inputVector = inputVector.normalized;

        return inputVector;
    }
}
