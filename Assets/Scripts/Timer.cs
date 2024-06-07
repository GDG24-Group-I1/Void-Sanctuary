using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer
{
    private readonly MonoBehaviour ownerObject;
    private IEnumerator timerCoroutine;

    public float Duration { get; private set; } = 0f;
    public Func<float?> OnTimerElapsed;
    
    public Timer(MonoBehaviour owner) { ownerObject = owner; }

    public void Start(float durationSeconds)
    {
        Stop();
        if (durationSeconds <= 0f)
        {
            var ret = OnTimerElapsed?.Invoke();
            if (ret != null)
            {
                Start(ret.Value);
            }
        }
        else
        {
            Duration = durationSeconds;
            timerCoroutine = TimerCoroutine();
            ownerObject.StartCoroutine(timerCoroutine);
        }
    }

    public void Stop()
    {
        if (timerCoroutine != null)
        {
            Duration = 0f;
            ownerObject.StopCoroutine(timerCoroutine);
        }
    }

    private void OnElapsed()
    {
        var ret = OnTimerElapsed?.Invoke();
        ownerObject.StopCoroutine(timerCoroutine);
        if (ret != null)
        {
            Start(ret.Value);
        }
    }

    private IEnumerator TimerCoroutine()
    {
        yield return new WaitForSeconds(Duration);
        OnElapsed();
    }
}
