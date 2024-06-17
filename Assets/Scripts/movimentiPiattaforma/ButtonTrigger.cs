using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonTrigger : MonoBehaviour
{
    public MovingTrigger platform; // La piattaforma da muovere

    void OnTriggerEnter(Collider other)
    {
        // Controlla se l'oggetto che ha attivato il trigger è il giocatore o un oggetto specifico
        if (other.CompareTag("Player"))
        {
            platform.StartMoving();
        }
    }
}
