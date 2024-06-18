using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlloLuce: MonoBehaviour
{
   
    public SwitchPorta door; //crea un oggetto della classe SwitchPorta
     public Color newEmissionColor = Color.green;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("DraggableCube"))
        {
            gameObject.GetComponent<MeshRenderer>().material.color = Color.green;

            // Ottieni il materiale del MeshRenderer
             Material material = gameObject.GetComponent<MeshRenderer>().material;

            // Abilita l'emissione sul materiale
             material.EnableKeyword("_EMISSION");

             // Cambia il colore dell'emissione
            material.SetColor("_EmissionColor", newEmissionColor);
            
            door.cubeSwitchActivated = true;
            
            door.CheckSwitches();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("DraggableCube"))
        {
            gameObject.GetComponent<MeshRenderer>().material.color = Color.red;
            
            door.cubeSwitchActivated = false;
        }
    }
}
