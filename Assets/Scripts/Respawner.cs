using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Respawner : MonoBehaviour
{
    [SerializeField] private GameObject playerFollower;
    [SerializeField] private GameObject playerPrefab;
    private GameObject youDiedText;
    private Transform cameraTransform;
    private GameObject healthBar;
    private GameObject loaderBorder;
    private GameObject dashLoaderBorder;
    private GameObject playerObject;
    private Image uiWeaponImage;
    private Animator animator;
    private List<Sprite> powerupsEquipped = new();
    private Timer respawnTimer;
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
        animator = youDiedText.GetComponent<Animator>();
        cameraTransform = player.cameraTransform;
        healthBar = player.healthBar;
        loaderBorder = player.loaderBorder;
        dashLoaderBorder = player.dashLoaderBorder;
        uiWeaponImage = player.uiWeaponImage;
    }

    public void RespawnPlayer()
    {
        youDiedText.SetActive(false);
        respawnTimer.Start(2f);
    } 

    private void RespawnPlayerCallback()
    {
        playerObject = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
        var followPlayer = playerFollower.GetComponent<FollowPlayer>();
        var player = playerObject.GetComponent<Player>();
        followPlayer.Player = playerObject;
        followPlayer.PlayerScript = player;
        playerFollower.transform.position = playerObject.transform.position;
        player.youDiedText = youDiedText;
        player.cameraTransform = cameraTransform;
        player.healthBar = healthBar;
        player.loaderBorder = loaderBorder;
        player.dashLoaderBorder = dashLoaderBorder;
        player.uiWeaponImage = uiWeaponImage;
        player.SetPowerupsOnRespawn(powerupsEquipped);
        var enemyList = GameObject.FindGameObjectsWithTag("EnemyObj");
        foreach (var enemy in enemyList)
        {
            if (!enemy.TryGetComponent<EnemyAI>(out var enemyAI))
            {
                Debug.LogError("EnemyAI script not found on enemy object");
            }
            enemyAI.player = playerObject.transform;
        }
    }

    public void AddPowerup(Sprite sprite)
    {
        powerupsEquipped.Add(sprite);
    }
}
