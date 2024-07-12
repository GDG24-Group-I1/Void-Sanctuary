using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonSwitch : MonoBehaviour
{
    public SwitchPorta door; //crea un oggetto della classe SwitchPorta
    public Color newEmissionColor = Color.green;

    //void OnTriggerEnter(Collider other)
    public void cambioColore()
    {
        // Controlla se l'oggetto che ha attivato il trigger è il giocatore o un oggetto specifico

        //if (other.gameObject.CompareTag("Projectile") || other.gameObject.CompareTag("Sword"))
        //{
            gameObject.GetComponent<MeshRenderer>().materials[0].color = Color.green;

            // Ottieni il materiale del MeshRenderer
             Material material = gameObject.GetComponent<MeshRenderer>().materials[0];

            // Abilita l'emissione sul materiale
             material.EnableKeyword("_EMISSION");

             // Cambia il colore dell'emissione
            material.SetColor("_EmissionColor", newEmissionColor);
            door.swordSwitchActivated = true;
            door.CheckSwitches();
       // }
    }
}
