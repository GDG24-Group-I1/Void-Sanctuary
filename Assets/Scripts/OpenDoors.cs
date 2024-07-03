using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenDoors : MonoBehaviour
{
    private Animator animator; // Riferimento all'Animator

    void Start()
    {
        animator = GetComponent<Animator>(); // Ottiene il riferimento all'Animator dell'oggetto corrente
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Attiva l'animazione per aprire la porta
            animator.ResetTrigger("close");
            animator.SetTrigger("open");
        }
        
    }
     private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Chiudi la porta quando il giocatore esce dal trigger
            animator.ResetTrigger("open");
            animator.SetTrigger("close");
        }
    }
}
