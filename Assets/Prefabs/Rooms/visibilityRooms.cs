using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class visibilityRooms : MonoBehaviour {
    private Renderer[] renderers;
    private Light[] lights;
    private EnemyAI[] enemyAIs;
    private bool isVisible;

    private void Awake()
    {
        renderers = GetComponentsInChildren<Renderer>();
        lights = GetComponentsInChildren<Light>();
        enemyAIs = GetComponentsInChildren<EnemyAI>();
    }

    public void DeactivateRoomAtStart()
    {
        SetRoomVisibility(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SetRoomVisibility(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
           SetRoomVisibility(false);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && !isVisible)
        {
            SetRoomVisibility(true);
        }
    }

    public void SetRoomVisibility(bool isVisible)
    {
        this.isVisible = isVisible;
        foreach (Renderer renderer in renderers)
        {
            if (renderer != null)
            {
                renderer.enabled = isVisible;
            }
        }
        foreach (Light light in lights)
        {
            if (light != null)
            {
                light.enabled = isVisible;
            }
        }
        foreach (EnemyAI enemyAI in enemyAIs)
        {
            if (enemyAI != null)
            {
                enemyAI.gameObject.SetActive(isVisible);
            }
        }
    }
}
