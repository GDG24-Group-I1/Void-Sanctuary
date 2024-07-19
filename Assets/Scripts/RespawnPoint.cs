using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnPoint : MonoBehaviour, IDataPersistence
{
    public int respawnPointID;
    private GameData gameData;
    private GameObject checkpointIndicator;
    private Timer timer;
    void Start()
    {
        checkpointIndicator = GameObject.Find("GameUI").transform.Find("CheckpointIndicator").gameObject;
        timer = new Timer(this)
        {
            OnTimerElapsed = () => { checkpointIndicator.SetActive(false); return null; }
        };
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Debug.Log($"Player entered respawn point {respawnPointID}");
            gameData.playerData.lastRespawnPointID = respawnPointID;
            checkpointIndicator.SetActive(true);
            timer.Start(2.5f);
        }
    }
    public void LoadData(GameData data)
    {
        gameData = data;
    }
}
