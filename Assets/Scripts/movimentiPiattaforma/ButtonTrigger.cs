using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonTrigger : MonoBehaviour
{
    public MovingTrigger platform; // La piattaforma da muovere

    void OnTriggerEnter(Collider other)
    {
        // Controlla se l'oggetto che ha attivato il trigger � il giocatore o un oggetto specifico
        if (other.gameObject.name == "Projectile(Clone)" || other.gameObject.CompareTag("Sword"))
        {
            platform.StartMoving();
        }
    }
}
