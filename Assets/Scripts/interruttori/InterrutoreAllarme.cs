using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InterrutoreAllarme : MonoBehaviour
{
    public Light targetLight; // La luce che verrà attivata/disattivata
    public Animator animator; // Riferimento all'Animator
    public animLevaLigth luce;
    void Start()
    {
        animator = GetComponent<Animator>(); // Ottiene il riferimento all'Animator dell'oggetto corrente
    }


    void OnTriggerEnter(Collider other)
    {
        
        if (other.gameObject.CompareTag("Projectile") || other.gameObject.CompareTag("Sword"))
        {
    
           // Trova tutte le luci con il tag "Alarm"
            GameObject[] alarmLights = GameObject.FindGameObjectsWithTag("Allarm");

            luce.cambioColore();

             animator.SetTrigger("attivo");
            // Spegni tutte le luci con il tag "Alarm"
            foreach (GameObject lightObject in alarmLights)
            {
                Light light = lightObject.GetComponent<Light>();
                if (light != null)
                {
                    light.enabled = false;
                }
            }
        }
    }
}


