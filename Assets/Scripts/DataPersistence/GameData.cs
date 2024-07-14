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
    public float volume;
}

[Serializable]
public struct SavedPlayerData
{
    public int lastRespawnPointID;
    public List<string> obtainedPowerups;
}

[Serializable]
public struct DoorStatus
{
    public Dictionary<string, bool> doorsMap;
}

[Serializable]
public class GameData
{
    public SavedSettings savedSettings;
    public SavedPlayerData playerData;
    public DoorStatus doorStatus;

    public GameData()
    {
        savedSettings = new()
        {
            holdDownToRun = true,
            slowDownAttack = true,
            volume = 1f
        };
        playerData = new()
        {
            lastRespawnPointID = 0,
            obtainedPowerups = new List<string>()
            {
                "GunTransparent"
            }
        };
        doorStatus = new() { doorsMap = new Dictionary<string, bool>() };
    }
}