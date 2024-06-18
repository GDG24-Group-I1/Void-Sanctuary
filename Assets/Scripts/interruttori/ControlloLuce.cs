using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlloLuce : MonoBehaviour
{
    public Light targetLight; // La luce che verrà attivata/disattivata

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("DraggableCube"))
        {
            // Accendi la luce quando il cubo entra nel trigger
            targetLight.enabled = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("DraggableCube"))
        {
            // Spegni la luce quando il cubo esce dal trigger
            targetLight.enabled = false;
        }
    }
}
