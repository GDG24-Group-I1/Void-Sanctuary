using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;


public class SwitchPorta : MonoBehaviour, IDataPersistence
{
    public Light targetLight; // La luce che cambierà colore
    public Color newColor; // Il nuovo colore per la luce

    public bool cubeSwitchActivated = false;
    public bool swordSwitchActivated = false;
    private Animator animator; // Riferimento all'Animator

    public string doorId;

    private AudioSource audioSource;

    [SerializeField] private AudioClip doorOpen;
    [SerializeField] private AudioClip puzzleSolve;

    public void LoadData(GameData data)
    {
        if (data.doorStatus.doorsMap.ContainsKey(doorId))
        {
            var res = data.doorStatus.doorsMap[doorId];
            cubeSwitchActivated = res;
            swordSwitchActivated = res;
            CheckSwitchesNoSound();
        }
        else
        {
            data.doorStatus.doorsMap.Add(doorId, cubeSwitchActivated && swordSwitchActivated);
            CheckSwitchesNoSound();
        }
    }

    public void SaveData(GameData data)
    {
        data.doorStatus.doorsMap[doorId] = cubeSwitchActivated && swordSwitchActivated;
    }

    private void PlayDoorSound()
    {
        if (!audioSource.isPlaying)
        {
            audioSource.PlayOneShot(doorOpen);
        }
    }

    void Start()
    {
        Assert.IsNotNull(puzzleSolve, "Sound for puzzle solve is not set in the inspector");
        Assert.IsNotNull(doorOpen, "Sound for door open is not set in the inspector");
        audioSource = GameObject.FindWithTag("AudioSource").GetComponent<AudioSource>();
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
                PlayDoorSound();
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
            PlayDoorSound();
        }
    }

    private void CheckSwitchesNoSound()
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

    public void CheckSwitches()
    {
        if (cubeSwitchActivated && swordSwitchActivated)
        {
            // Cambia il colore della luce
            if (targetLight != null)
            {
                targetLight.color = newColor;
            }
            audioSource.PlayOneShot(puzzleSolve);
        }
    }

#if UNITY_EDITOR
    public void SetSounds(AudioClip doorOpen, AudioClip puzzleSolve)
    {
        if (this.doorOpen == null)
            this.doorOpen = doorOpen;
        if (this.puzzleSolve == null)
            this.puzzleSolve = puzzleSolve;
    }
#endif
}
