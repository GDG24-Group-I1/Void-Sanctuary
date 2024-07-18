using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[Flags]
public enum TimerFlags
{
    Scaled = 0b001,
    Realtime = 0b010,
    AlwaysInvoke = 0b100
}

public class Timer
{
    private readonly MonoBehaviour ownerObject;
    private readonly MonoBehaviour referenceObject;
    private IEnumerator timerCoroutine;

    public float Duration { get; private set; } = 0f;
    public Func<float?> OnTimerElapsed;

    private readonly TimerFlags timerType;

    public bool IsRunning => timerCoroutine != null;
    
    public Timer(MonoBehaviour referenceObject, TimerFlags type = TimerFlags.Scaled) {
        this.referenceObject = referenceObject;
        ownerObject = TimerHelperSingleton.GetInstance();
        Assert.IsFalse(type.HasFlag(TimerFlags.Scaled) && type.HasFlag(TimerFlags.Realtime), "Timer type cannot be both scaled and realtime");
        timerType = type; 
    }

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
            timerCoroutine = null;
        }
    }

    private void OnElapsed()
    {
        Assert.IsNotNull(ownerObject, "Timer owner object is null");
        float? ret = null;
        if (referenceObject != null || timerType.HasFlag(TimerFlags.AlwaysInvoke))
        {
            ret = OnTimerElapsed?.Invoke();
        }
        ownerObject.StopCoroutine(timerCoroutine);
        timerCoroutine = null;
        if (ret != null)
        {
            Start(ret.Value);
        }
    }

    private IEnumerator TimerCoroutine()
    {
        if (timerType.HasFlag(TimerFlags.Scaled))
            yield return new WaitForSeconds(Duration);
        else
            yield return new WaitForSecondsRealtime(Duration);
        OnElapsed();
    }
}
