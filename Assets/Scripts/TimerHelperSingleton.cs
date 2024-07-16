using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerHelperSingleton : MonoBehaviour
{
    private static TimerHelperSingleton Instance { get; set; }

    public static TimerHelperSingleton GetInstance() { return Instance; }
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogWarning("Found more than one Data Persistence Manager, destroying the newest one");
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
