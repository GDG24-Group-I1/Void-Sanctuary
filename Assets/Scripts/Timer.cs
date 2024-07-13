using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public enum TimerType
{
    Scaled,
    Realtime
}

public class Timer
{
    private readonly MonoBehaviour ownerObject;
    private IEnumerator timerCoroutine;

    public float Duration { get; private set; } = 0f;
    public Func<float?> OnTimerElapsed;

    private readonly TimerType timerType;
    
    public Timer(MonoBehaviour owner, TimerType type = TimerType.Scaled) { ownerObject = owner; timerType = type; }

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
            Assert.IsNotNull(ownerObject, "Timer owner object is null");
            ownerObject.StartCoroutine(timerCoroutine);
        }
    }

    public void Stop()
    {
        if (timerCoroutine != null)
        {
            Duration = 0f;
            Assert.IsNotNull(ownerObject, "Timer owner object is null");
            ownerObject.StopCoroutine(timerCoroutine);
        }
    }

    private void OnElapsed()
    {
        Assert.IsNotNull(ownerObject, "Timer owner object is null");
        var ret = OnTimerElapsed?.Invoke();
        ownerObject.StopCoroutine(timerCoroutine);
        if (ret != null)
        {
            Start(ret.Value);
        }
    }

    private IEnumerator TimerCoroutine()
    {
        if (timerType == TimerType.Scaled)
            yield return new WaitForSeconds(Duration);
        else
            yield return new WaitForSecondsRealtime(Duration);
        OnElapsed();
    }
}
