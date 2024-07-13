using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Diagnostics;

[Serializable]
public struct SavedSettings
{
    public bool holdDownToRun;
    public bool slowDownAttack;
    public bool drawDebugRays;
    public float volume;
}


[Serializable]
public class GameData
{
    public SavedSettings savedSettings;


    public GameData()
    {
        savedSettings = new()
        {
            holdDownToRun = true,
            slowDownAttack = true,
            drawDebugRays = false,
            volume = 1f
        };
    }
}