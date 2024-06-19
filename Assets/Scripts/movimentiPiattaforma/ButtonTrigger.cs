using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonTrigger : MonoBehaviour
{
    public MovingTrigger platform; // La piattaforma da muovere
    public Color newEmissionColor = Color.green;

    //void OnTriggerEnter(Collider other)
    public void cambioColore()
    {
        
            gameObject.GetComponent<MeshRenderer>().materials[0].color = Color.green;

            // Ottieni il materiale del MeshRenderer
             Material material = gameObject.GetComponent<MeshRenderer>().materials[0];

            // Abilita l'emissione sul materiale
             material.EnableKeyword("_EMISSION");

             // Cambia il colore dell'emissione
            material.SetColor("_EmissionColor", newEmissionColor);

       // }
    }

    void OnTriggerEnter(Collider other)
    {
        // Controlla se l'oggetto che ha attivato il trigger è il giocatore o un oggetto specifico
        if (other.gameObject.name == "Projectile(Clone)" || other.gameObject.CompareTag("Sword"))
        {
            cambioColore();
            platform.StartMoving();
        }
    }
}
