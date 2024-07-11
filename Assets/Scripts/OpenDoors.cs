using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenDoors : MonoBehaviour
{
    [SerializeField] private bool unlocked = true;
    [SerializeField] private int requiredInputs = 5;
    public Light light;
    private int activeInputs = 0;
    public Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        if (light != null)
            if (!unlocked)
                light.color = Color.red;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && unlocked)
        {
            // Attiva l'animazione per aprire la porta
            animator.SetTrigger("open");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && unlocked)
        {
            // Chiudi la porta quando il giocatore esce dal trigger
            animator.SetTrigger("close");
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
        if (light != null)
            light.color = lightColor;
    }
}
