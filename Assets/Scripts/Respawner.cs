using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum EnemyType
{
    Melee,
    Ranged,
    Boss
}

struct EnemyData
{
    public GameObject instance;
    public Transform parent;
    public EnemyType type;
    public Vector3 position;
}

struct RoomData
{
    public GameObject instance;
    public RespawnPoint respawnPoint;
    public visibilityRooms visibilityRooms;
    public EnemyData[] enemyList;
}

public class Respawner : MonoBehaviour, IDataPersistence
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject enemySwordPrefab;
    [SerializeField] private GameObject enemyRangedPrefab;
    [SerializeField] private GameObject enemyBossPrefab;
    private GameObject playerObject;
    [SerializeField] private Sprite[] availablePowerups;
    private Timer respawnTimer;

    // camera stuff
    [SerializeField] private float MaxAdjustment;
    [SerializeField] private float yDiffThreshold;


    [SerializeField] private Transform cameraTransform;
    [SerializeField] private GameObject healthBar;
    [SerializeField] private GameObject loaderBorder;
    [SerializeField] private GameObject dashLoaderBorder;
    [SerializeField] private Image uiWeaponImage;
    [SerializeField] private GameObject youDiedText;

    private bool adjustingCamera;
    private RespawnPoint[] respawnPoints;
    private RoomData[] rooms;
    private GameData gameData;

    void Awake()
    {
        var roomObjects = SceneManager.GetActiveScene().GetRootGameObjects().SelectMany(x => x.GetComponentsInChildren<visibilityRooms>()).ToArray();
        foreach (var item in roomObjects)
        {
            item.DeactivateRoomAtStart();
        }
        rooms = roomObjects.Select(x => new RoomData
        {
            instance = x.gameObject,
            respawnPoint = x.GetComponentInChildren<RespawnPoint>(),
            visibilityRooms = x,
            enemyList = x.GetEnemyList().Select(y => new EnemyData
            {
                instance = y.gameObject,
                type = y.Type,
                position = y.transform.position,
                parent = y.transform.parent
            }).ToArray()
        }).ToArray();
    }

    // Start is called before the first frame update
    void Start()
    {
        Assert.IsNotNull(cameraTransform, "CAMERA TRANSFORM IS NOT SET IN THE PLAYER RESPAWNER OBJECT IN THE SCENE, PUT THE TopDownCamera IN THE CameraTrasform SLOT ON THIS GAMEOBJECT");
        Assert.IsNotNull(healthBar, "HEALTH BAR IS NOT SET IN THE PLAYER RESPAWNER OBJECT IN THE SCENE, PUT THE Canvas->HealthBar OBJECT IN THE Health Bar SLOT ON THIS GAME OBJECT");
        Assert.IsNotNull(loaderBorder, "LOADER BORDER IS NOT SET IN PLAYER RESPAWNER OBJECT IN THE SCENE, PUT THE Canvas->Loader->LoaderBorder IN THE Loader Border SLOT ON THIS GAME OBJECT");
        Assert.IsNotNull(dashLoaderBorder, "DASH LOADER BORDER IS NOT SET IN PLAYER RESPAWNER OBJECT IN THE SCENE, PUT THE Canvas->DashLoader->DashLoaderBorder IN THE Dash Loader Border SLOT ON THIS GAME OBJECT");
        Assert.IsNotNull(uiWeaponImage, "UI WEAPON IMAGE IS NOT SET IN PLAYER RESPAWNER OBJECT IN THE SCENE, PUT THE Canvas->UiWeaponImage IN THE UI Weapon Image SLOT ON THIS GAME OBJECT");
        Assert.IsNotNull(youDiedText, "YOU DIED TEXT IS NOT SET IN PLAYER RESPAWNER OBJECT IN THE SCENE, PUT THE Canvas->YouDiedText IN THE You Died Text SLOT ON THIS GAME OBJECT");
        respawnTimer = new Timer(this)
        {
            OnTimerElapsed = () =>
            {
                RespawnPlayerCallback();
                return null;
            }
        };
        respawnPoints = GameObject.FindGameObjectsWithTag("RespawnPoint").Select(x => x.GetComponent<RespawnPoint>()).ToArray();
        var powerups = GameObject.FindGameObjectsWithTag("Powerup");
        foreach (var powerup in powerups)
        {
            if (powerup.TryGetComponent<PowerUpHolder>(out var powerupHolder))
            {
                if (gameData.playerData.obtainedPowerups.Contains(powerupHolder.Powerup.name))
                {
                    Debug.Log($"Destroying powerup {powerup.name} because it was already obtained");
                    Destroy(powerup);
                }
            }
        }
        RespawnPlayerCallback();
    }

    private void Update()
    {
        FollowCameraToPlayer();
    }

    public void LoadData(GameData data)
    {
        gameData = data;
    }

    private void FollowCameraToPlayer()
    {
        if (playerObject == null)
        {
            return;
        }
        transform.rotation = playerObject.transform.rotation;
        var playerPosition = playerObject.transform.position;
        var yDiff = Mathf.Abs(playerPosition.y - transform.position.y);
        if (!adjustingCamera && yDiff > yDiffThreshold)
        {
            // if we were folling closely and the player made a sudden Y change, we switch to adjusting mode
            adjustingCamera = true;
            DebugExt.LogCamera("Following in adjusting mode");
        }
        else if (adjustingCamera && Mathf.Approximately((float)Math.Round(yDiff, 1), 0))
        {
            adjustingCamera = false;
            DebugExt.LogCamera("Following in close mode");
        }
        var newYPos = adjustingCamera ? Mathf.Lerp(transform.position.y, playerPosition.y, MaxAdjustment * Time.deltaTime) : transform.position.y;
        transform.position = new Vector3(playerPosition.x, newYPos, playerPosition.z);
    }

    public void RespawnPlayer()
    {
        youDiedText.SetActive(false);
        respawnTimer.Start(2f);
    }

    private void RespawnPlayerCallback()
    {
        foreach (var room in rooms)
        {
            room.visibilityRooms.ForceSetRoomVisibility(false);
        }
        var respawnPoint = respawnPoints.FirstOrDefault(x => x.respawnPointID == gameData.playerData.lastRespawnPointID);
        Debug.Log($"Respawning player at respawn point {gameData.playerData.lastRespawnPointID}");
        playerObject = Instantiate(playerPrefab, respawnPoint == null ? Vector3.zero : respawnPoint.transform.position, Quaternion.identity);
        var player = playerObject.GetComponent<Player>();
        player.YouDiedText = youDiedText;
        player.CameraTransform = cameraTransform;
        player.HealthBar = healthBar;
        player.LoaderBorder = loaderBorder;
        player.DashLoaderBorder = dashLoaderBorder;
        player.UiWeaponImage = uiWeaponImage;
        player.SetPowerupsOnRespawn(availablePowerups.Where(x => gameData.playerData.obtainedPowerups.Contains(x.name)));

        var staticEnemies = rooms.SelectMany(x => x.enemyList).Where(x => x.instance != null).Select(x => x.instance).ToArray();
        var activeEnemies = GameObject.FindGameObjectsWithTag("EnemyObj").Where(x => !staticEnemies.Contains(x));
        foreach (var enemy in activeEnemies)
        {
            enemy.GetComponent<EnemyAI>().player = player.transform;
        }
        foreach (var room in rooms)
        {
            var needsActive = room.respawnPoint != null && room.respawnPoint.respawnPointID == respawnPoint.respawnPointID;
            var newEnemyList = new EnemyAI[room.enemyList.Length];
            for (int i = 0; i < room.enemyList.Length; i++)
            {
                newEnemyList[i] = InstantiateEnemy(ref room.enemyList[i], needsActive);
            }
            room.visibilityRooms.SetEnemyList(newEnemyList);
            var doors = room.instance.GetComponentsInChildren<OpenDoors>();
            foreach (var door in doors)
            {
                door.OnPlayerRespawn();
            }
            foreach (var (enemy, ogPosition) in room.visibilityRooms.GetDynamicEnemyList())
            {
                if (enemy != null)
                {
                    enemy.Unfreeze();
                    enemy.transform.position = ogPosition;
                    enemy.player = playerObject.transform;
                }
            }
        }
    }

    private EnemyAI InstantiateEnemy(ref EnemyData enemyData, bool active)
    {
        if (enemyData.instance != null)
        {
            Destroy(enemyData.instance);
        }
        var enemy = enemyData.type switch
        {
            EnemyType.Melee => enemySwordPrefab,
            EnemyType.Ranged => enemyRangedPrefab,
            EnemyType.Boss => enemyBossPrefab,
            _ => throw new ArgumentOutOfRangeException()
        };

        enemyData.instance = Instantiate(enemy, enemyData.position, Quaternion.identity);
        if (enemyData.parent != null)
        {
            enemyData.instance.transform.SetParent(enemyData.parent, true);
        }
        var enemyAi = enemyData.instance.GetComponent<EnemyAI>();
        enemyAi.player = playerObject.transform;
        enemyData.instance.SetActive(active);
        return enemyAi;
    }
}
