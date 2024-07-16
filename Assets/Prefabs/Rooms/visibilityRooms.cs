using System.Collections;
using System.Collections.Generic;
using UnityEngine;


enum VisibilityState
{
    Visible,
    NotVisible
}

public class visibilityRooms : MonoBehaviour
{
    private Renderer[] renderers;
    private Light[] lights;
    private EnemyAI[] enemyAIs;

    private VisibilityState visibilityState;
    private Timer timer;

    private void Awake()
    {
        visibilityState = VisibilityState.Visible;
        GetReferences();
    }

    public EnemyAI[] GetEnemyList()
    {
        return enemyAIs;
    }

    public void SetEnemyList(EnemyAI[] enemyList)
    {
        enemyAIs = enemyList;
    }

    private void GetReferences()
    {
        renderers ??= GetComponentsInChildren<Renderer>();
        lights ??= GetComponentsInChildren<Light>();
        enemyAIs ??= GetComponentsInChildren<EnemyAI>();
    }

    public void DeactivateRoomAtStart()
    {
        GetReferences();
        ForceSetRoomVisibility(false);
    }

    private void Start()
    {
        timer = new Timer()
        {
            OnTimerElapsed = () =>
            {
                SetRoomVisibility();
                return null;
            }
        };
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            timer.Stop();
            visibilityState = VisibilityState.Visible;
            SetRoomVisibility();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            visibilityState = VisibilityState.NotVisible;
            if (!timer.IsRunning)
            {
                timer.Start(0.5f);
            }
        }
    }

    public void ForceSetRoomVisibility(bool isVisible)
    {
        visibilityState = isVisible ? VisibilityState.Visible : VisibilityState.NotVisible;
        SetRoomVisibility();
    }

    public void SetRoomVisibility()
    {
        bool isVisible = visibilityState == VisibilityState.Visible;
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
