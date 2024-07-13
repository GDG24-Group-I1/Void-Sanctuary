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
    }
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        dataPersistenceObjects = FindAllDataPersistanceObjects();
        LoadGame();
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

    public void ResetGame()
    {
        SaveGame();
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