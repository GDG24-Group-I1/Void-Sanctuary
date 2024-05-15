using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    // Start is called before the first frame update

    private VoidSanctuaryActions voidSanctuaryActions;

    void Start()
    {
        voidSanctuaryActions = new VoidSanctuaryActions();
        voidSanctuaryActions.Enable();
        var exitGameAction = voidSanctuaryActions.FindAction("ExitGameAction");
        exitGameAction.performed += (context) =>
        {
            Application.Quit();
        };
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
