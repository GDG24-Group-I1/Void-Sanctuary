using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogTrigger : MonoBehaviour
{
    [SerializeField]
    private string[] DialogueIds;
    [SerializeField]
    private bool SingleShot;

    private int currentDialogIndex = 0;

    private void OnTriggerEnter(Collider other)
    {
        var didAllDialogs = currentDialogIndex >= DialogueIds.Length;
        if (didAllDialogs && !SingleShot)
        {
            currentDialogIndex = 0;
            didAllDialogs = false;
        }
        if (other.CompareTag("Player") && !didAllDialogs)
        {
            other.GetComponent<Player>().TriggerDialog(DialogueIds[currentDialogIndex]);
            currentDialogIndex++;
        }
    }
}
