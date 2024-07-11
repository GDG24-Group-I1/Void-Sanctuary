using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public enum EnemyType
{
    Melee,
    Ranged
}

struct EnemyData
{
    public GameObject instance;
    public EnemyType type;
    public Vector3 position;
}

public class Respawner : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject enemySwordPrefab;
    [SerializeField] private GameObject enemyRangedPrefab;
    private GameObject youDiedText;
    private Transform cameraTransform;
    private GameObject healthBar;
    private GameObject loaderBorder;
    private GameObject dashLoaderBorder;
    private GameObject playerObject;
    private Image uiWeaponImage;
    private readonly List<Sprite> powerupsEquipped = new();
    private Timer respawnTimer;
    private EnemyData[] enemies;

    // camera stuff
    [SerializeField] private float MaxAdjustment;
    [SerializeField] private float yDiffThreshold;

    private bool adjustingCamera;

    // Start is called before the first frame update
    void Start()
    {
        respawnTimer = new Timer(this)
        {
            OnTimerElapsed = () =>
            {
                RespawnPlayerCallback();
                return null;
            }
        };
        playerObject = GameObject.FindGameObjectWithTag("Player");
        var player = playerObject.GetComponent<Player>();
        youDiedText = player.youDiedText;
        cameraTransform = player.cameraTransform;
        healthBar = player.healthBar;
        loaderBorder = player.loaderBorder;
        dashLoaderBorder = player.dashLoaderBorder;
        uiWeaponImage = player.uiWeaponImage;
        enemies = GameObject.FindGameObjectsWithTag("EnemyObj").Select(x => new EnemyData { 
            instance = x,
            type = x.GetComponent<EnemyAI>().Type,
            position = x.transform.position
        }).ToArray();
    }

    private void Update()
    {
        FollowCameraToPlayer();
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
        playerObject = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
        var player = playerObject.GetComponent<Player>();
        player.youDiedText = youDiedText;
        player.cameraTransform = cameraTransform;
        player.healthBar = healthBar;
        player.loaderBorder = loaderBorder;
        player.dashLoaderBorder = dashLoaderBorder;
        player.uiWeaponImage = uiWeaponImage;
        player.SetPowerupsOnRespawn(powerupsEquipped);
        for (int i = 0; i < enemies.Length; i++)
        {
            ref var enemyData = ref enemies[i];
            if (enemyData.instance != null)
            {
                Destroy(enemyData.instance);
            }
            var enemy = enemyData.type == EnemyType.Melee ? enemySwordPrefab : enemyRangedPrefab;
            enemyData.instance = Instantiate(enemy, enemyData.position, Quaternion.identity);
            enemyData.instance.GetComponent<EnemyAI>().player = playerObject.transform;

        }
    }

    public void ClearPowerups()
    {
        powerupsEquipped.Clear();
    }

    public void AddPowerup(Sprite sprite)
    {
        powerupsEquipped.Add(sprite);
    }
}
