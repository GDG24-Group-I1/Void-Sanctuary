using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningController : MonoBehaviour
{
    public ParticleSystem lightningParticleSystem; // Riferimento al sistema di particelle del fulmine
    public float minInterval = 2f; // Intervallo minimo tra i fulmini
    public float maxInterval = 5f; // Intervallo massimo tra i fulmini
    private float nextLightningTime = 0f;

    void Start()
    {
        ScheduleNextLightning();
    }

    void Update()
    {
        if (Time.time >= nextLightningTime)
        {
            TriggerLightning();
            ScheduleNextLightning();
        }
    }

    void TriggerLightning()
    {
        lightningParticleSystem.Play();
    }

    void ScheduleNextLightning()
    {
        nextLightningTime = Time.time + Random.Range(minInterval, maxInterval);
    }
}
