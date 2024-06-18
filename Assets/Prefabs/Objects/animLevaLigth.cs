using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class animLevaLigth : MonoBehaviour
{

    //void OnTriggerEnter(Collider other)
    public void cambioColore()
    {
            gameObject.GetComponent<MeshRenderer>().materials[0].color = Color.green;

            // Ottieni il materiale del MeshRenderer
             Material material = gameObject.GetComponent<MeshRenderer>().materials[0];

            // Abilita l'emissione sul materiale
             material.EnableKeyword("_EMISSION");

             // Cambia il colore dell'emissione
            material.SetColor("_EmissionColor",Color.green);
            
       
    }
}
