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
}


[Serializable]
public class GameData
{
    public SavedSettings savedSettings;


    public GameData()
    {
        savedSettings = new()
        {
            holdDownToRun = true
        };
    }
}