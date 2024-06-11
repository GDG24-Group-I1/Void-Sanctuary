using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GradualBlinkingLight : MonoBehaviour
{
    public Light lightSource; // La luce che vuoi far lampeggiare
    public float minIntensity = 0f; // Intensità minima della luce
    public float maxIntensity = 1f; // Intensità massima della luce
    public float blinkDuration = 1f; // Durata di un ciclo di accensione e spegnimento

    private float timer = 0f;
    private bool increasing = true; // Indica se l'intensità sta aumentando

    void Start()
    {
        if (lightSource == null)
        {
            lightSource = GetComponent<Light>();
        }
        lightSource.intensity = minIntensity; // Inizia con l'intensità minima
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (increasing)
        {
            lightSource.intensity = Mathf.Lerp(minIntensity, maxIntensity, timer / (blinkDuration / 2));

            if (timer >= blinkDuration / 2)
            {
                timer = 0f;
                increasing = false; // Inizia a diminuire l'intensità
            }
        }
        else
        {
            lightSource.intensity = Mathf.Lerp(maxIntensity, minIntensity, timer / (blinkDuration / 2));

            if (timer >= blinkDuration / 2)
            {
                timer = 0f;
                increasing = true; // Inizia ad aumentare l'intensità
            }
        }
    }
}