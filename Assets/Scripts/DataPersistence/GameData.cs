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
    public float volume;
    public bool drawDashIndicator;

    public static SavedSettings DefaultSettings => new()
    {
        holdDownToRun = true,
        drawDashIndicator = false,
        volume = 1f
    };
}

[Serializable]
public struct SavedPlayerData
{
    public int lastRespawnPointID;
    public List<string> obtainedPowerups;

    public static SavedPlayerData DefaultPlayerData => new()
    {
        lastRespawnPointID = 0,
        obtainedPowerups = new List<string>()
        {
            "GunTransparent"
        }
    };
}

[Serializable]
public struct DoorStatus
{
    public Dictionary<string, bool> doorsMap;

    public static DoorStatus DefaultDoorStatus => new()
    {
        doorsMap = new Dictionary<string, bool>()
    };
}

[Flags]
public enum ResetType
{
    ResetSettings = 0b01,
    ResetSaveData = 0b10,
    ResetAll = ResetSettings | ResetSaveData
}

[Serializable]
public class GameData
{
    public SavedSettings savedSettings;
    public SavedPlayerData playerData;
    public DoorStatus doorStatus;

    public GameData()
    {
        savedSettings = SavedSettings.DefaultSettings;
        playerData = SavedPlayerData.DefaultPlayerData;
        doorStatus = DoorStatus.DefaultDoorStatus;
    }

    public void ResetGameData(ResetType resetType)
    {
        if (resetType.HasFlag(ResetType.ResetSettings))
        {
            savedSettings = SavedSettings.DefaultSettings;
        }
        if (resetType.HasFlag(ResetType.ResetSaveData))
        {
            playerData = SavedPlayerData.DefaultPlayerData;
            doorStatus = DoorStatus.DefaultDoorStatus;
        }
    }
}