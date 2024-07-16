using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnPoint : MonoBehaviour, IDataPersistence
{
    public int respawnPointID;
    private GameData gameData;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Debug.Log($"Player entered respawn point {respawnPointID}");
            gameData.playerData.lastRespawnPointID = respawnPointID;
        }
    }
    public void LoadData(GameData data)
    {
        gameData = data;
    }
}
