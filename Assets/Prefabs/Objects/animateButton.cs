using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class animateButton : MonoBehaviour
{
     public Animator animator; // Riferimento all'Animator
     public ButtonSwitch pulsante;

    void Start()
    {
        animator = GetComponent<Animator>(); // Ottiene il riferimento all'Animator dell'oggetto corrente
    }


    void OnTriggerEnter(Collider other)
    {
        
        if (other.gameObject.name == "Projectile(Clone)" || other.gameObject.CompareTag("Sword"))
        {
            pulsante.cambioColore();
             animator.SetTrigger("attiva");
            
        }
    }
}
