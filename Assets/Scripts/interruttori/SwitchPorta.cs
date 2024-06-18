using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SwitchPorta : MonoBehaviour
{
    public Light targetLight; // La luce che cambierà colore
    public Color newColor; // Il nuovo colore per la luce

    public bool cubeSwitchActivated = false;
    public bool swordSwitchActivated = false;
    public Animator animator; // Riferimento all'Animator

    void Start()
    {
        animator = GetComponent<Animator>(); // Ottiene il riferimento all'Animator dell'oggetto corrente
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Verifica se entrambi gli interruttori sono attivati
            if (cubeSwitchActivated && swordSwitchActivated)
            {
                // Attiva l'animazione per aprire la porta
                animator.SetTrigger("open");
            }
        }
        
    }
     private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Chiudi la porta quando il giocatore esce dal trigger
            animator.SetTrigger("close");
        }
    }

    public void CheckSwitches()
    {
        if (cubeSwitchActivated && swordSwitchActivated)
        {
            // Cambia il colore della luce
            if (targetLight != null)
            {
                targetLight.color = newColor;
            }
        }
    }
}
