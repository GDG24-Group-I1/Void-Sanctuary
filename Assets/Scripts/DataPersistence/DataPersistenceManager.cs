using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;
using UnityEngine.Playables;

public class DataAssetLoader
{
    public static T Load<T>(TextAsset asset)
    {
        return JsonUtility.FromJson<T>(asset.text);
    }
}

public class DataPersistenceManager : MonoBehaviour
{
    [Header("File Storage Config")]
    [SerializeField] private string fileName;

#if UNITY_EDITOR
    [Header("Development settings")]
    [SerializeField] private bool forceRespawnPoint;
    [SerializeField] private int respawnPointId;
#endif

    private GameData gameData;
    private List<IDataPersistence> dataPersistenceObjects;
    private FileDataHandler dataHandler;

    private static DataPersistenceManager Instance { get; set; }

    public static DataPersistenceManager GetInstance() { return Instance; }


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
        dataHandler = new(Application.persistentDataPath, fileName);
    }
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }
    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        dataPersistenceObjects = FindAllDataPersistanceObjects();
        LoadGame();
    }

    public void OnSceneUnloaded(Scene scene)
    {
        SaveGame();
    }

    public void RegisterDataPersistenceObject(IDataPersistence obj)
    {
        if (dataPersistenceObjects.Contains(obj))
        {
            return;
        }
        dataPersistenceObjects.Add(obj);
        obj.LoadData(gameData);
    }
    public void UnregisterDataPersistenceObject(IDataPersistence obj)
    {
        obj.SaveData(gameData);
        dataPersistenceObjects.Remove(obj);
    }
    public void NewGame()
    {
        gameData = new();
    }
    public void LoadGame()
    {
        gameData = dataHandler.Load();
        if (gameData == null)
        {
            Debug.Log("No data was found. Initializing data to defaults");
            NewGame();
        }
#if UNITY_EDITOR
        if (forceRespawnPoint)
        {
            gameData.playerData.lastRespawnPointID = respawnPointId;
        }
#endif
        foreach (IDataPersistence dataPersistence in dataPersistenceObjects)
        {
            dataPersistence?.LoadData(gameData);
        }

    }
    public void SaveGame()
    {
        foreach (IDataPersistence dataPersistence in dataPersistenceObjects)
        {
            dataPersistence?.SaveData(gameData);
        }
        dataHandler.Save(gameData);
    }

    public void ResetGame(ResetType resetType)
    {
        gameData.ResetGameData(resetType);
        SaveGame();
        LoadGame();
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            SaveGame();
        }
    }

    private List<IDataPersistence> FindAllDataPersistanceObjects()
    {
        IEnumerable<IDataPersistence> dataPersistanceObjects = FindObjectsOfType<MonoBehaviour>().OfType<IDataPersistence>();
        return new List<IDataPersistence>(dataPersistanceObjects);
    }
}