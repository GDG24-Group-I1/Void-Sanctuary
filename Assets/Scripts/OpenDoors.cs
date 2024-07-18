using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class OpenDoors : MonoBehaviour, IDataPersistence
{
    [SerializeField] private bool unlocked = true;
    [SerializeField] private int requiredInputs = 5;
    [SerializeField] private bool resetOnRespawn = false;
    private Light doorLight;
    private int activeInputs = 0;
    private Animator animator;
    public string doorId;
    private bool originalStatus;
    private AudioSource audioSource;

    public void LoadData(GameData data)
    {
        originalStatus = unlocked;
        if (data.doorStatus.doorsMap.ContainsKey(doorId))
        {
            unlocked = data.doorStatus.doorsMap[doorId];
            changeLight();
        } else
        {
            data.doorStatus.doorsMap.Add(doorId, unlocked);
            changeLight();
        }
    }

    public void SaveData(GameData data)
    {
        data.doorStatus.doorsMap[doorId] = unlocked;
    }

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        doorLight = GetComponentInChildren<Light>();
        animator = GetComponent<Animator>();
        if (doorLight != null)
            if (!unlocked)
                doorLight.color = Color.red;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && unlocked)
        {
            // Attiva l'animazione per aprire la porta
            animator.ResetTrigger("close");
            animator.SetTrigger("open");
            audioSource.Play();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && unlocked)
        {
            // Chiudi la porta quando il giocatore esce dal trigger
            animator.ResetTrigger("open");
            animator.SetTrigger("close");
            audioSource.Play();
        }
    }

    public void OnPlayerRespawn()
    {
        if (resetOnRespawn)
        {
            unlocked = originalStatus; 
            changeLight();
        }
    }

    public void Input(int receivedInput)
    {
        activeInputs += receivedInput;
        //Debug.Log($"door {gameObject.name} now has {activeInputs} inputs active");
        if (activeInputs == requiredInputs)
        {
            Debug.Log($"door {gameObject.name} is now unlocked");
            unlocked = true;
            changeLight();
        }
        else if (activeInputs < 0)
        {
            Debug.Log($"door {gameObject.name} is now locked");
            unlocked = false;
            changeLight();
        }
    }

    public void changeLight() 
    {
        var lightColor = Color.green;
        if (!unlocked)
            lightColor = Color.red;
        if (doorLight != null)
            doorLight.color = lightColor;
    }
}
