using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    private List<(EnemyAI, Vector3)> dynamicEnemyAis;

    private VisibilityState visibilityState;
    private Timer timer;

    private AudioSource audioSource;

    [SerializeField] AudioClip roomMusic;

    private void Awake()
    {
        visibilityState = VisibilityState.Visible;
        audioSource = GameObject.FindWithTag("AudioSource").GetComponent<AudioSource>();
        GetReferences();
    }

    public EnemyAI[] GetEnemyList()
    {
        return enemyAIs;
    }

    public List<(EnemyAI, Vector3)> GetDynamicEnemyList()
    {
        return dynamicEnemyAis;
    }

    public void SetEnemyList(EnemyAI[] enemyList)
    {
        enemyAIs = enemyList;
    }

    public void AddEnemy(EnemyAI enemyAi)
    {
        dynamicEnemyAis.Add((enemyAi, enemyAi.transform.position));
    }

    private void GetReferences()
    {
        renderers ??= GetComponentsInChildren<Renderer>();
        lights ??= GetComponentsInChildren<Light>();
        enemyAIs ??= GetComponentsInChildren<EnemyAI>();
        dynamicEnemyAis ??= new();
    }

    public void DeactivateRoomAtStart()
    {
        GetReferences();
        ForceSetRoomVisibility(false);
    }

    private void Start()
    {
        timer = new Timer(this)
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
            if (roomMusic != null)
            {
                var needsChangeMusic = audioSource.clip == null || audioSource.clip.name != roomMusic.name;
                if (needsChangeMusic)
                {
                    audioSource.Stop();
                    audioSource.clip = roomMusic;
                    audioSource.loop = true;
                    audioSource.Play();
                }
            }
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
        foreach (var enemyAI in dynamicEnemyAis)
        {
            if (enemyAI.Item1 != null)
            {
                enemyAI.Item1.gameObject.SetActive(isVisible);
                if (isVisible)
                {
                    enemyAI.Item1.transform.position = enemyAI.Item2;
                }
            }
        }
    }
}
