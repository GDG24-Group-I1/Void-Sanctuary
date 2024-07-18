using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(AudioSource))]
public class SwitchPorta : MonoBehaviour, IDataPersistence
{
    public Light targetLight; // La luce che cambierà colore
    public Color newColor; // Il nuovo colore per la luce

    public bool cubeSwitchActivated = false;
    public bool swordSwitchActivated = false;
    private Animator animator; // Riferimento all'Animator

    public string doorId;

    private AudioSource audioSource;
    public void LoadData(GameData data)
    {
        if (data.doorStatus.doorsMap.ContainsKey(doorId))
        {
            var res = data.doorStatus.doorsMap[doorId];
            cubeSwitchActivated = res;
            swordSwitchActivated = res;
            CheckSwitches();
        }
        else
        {
            data.doorStatus.doorsMap.Add(doorId, cubeSwitchActivated && swordSwitchActivated);
            CheckSwitches();
        }
    }

    public void SaveData(GameData data)
    {
        data.doorStatus.doorsMap[doorId] = cubeSwitchActivated && swordSwitchActivated;
    }

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
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
                animator.ResetTrigger("close");
                animator.SetTrigger("open");
                audioSource.Play();
            }
        }
        
    }
     private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Chiudi la porta quando il giocatore esce dal trigger
            animator.ResetTrigger("open");
            animator.SetTrigger("close");
            audioSource.Play();
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
