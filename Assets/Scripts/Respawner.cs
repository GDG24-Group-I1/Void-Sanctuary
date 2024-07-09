using UnityEngine;
using UnityEngine.UI;

public class Respawner : MonoBehaviour
{
    [SerializeField] private GameObject playerFollower;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject youDiedText;
    private Transform cameraTransform;
    private GameObject healthBar;
    private GameObject loaderBorder;
    private GameObject dashLoaderBorder;
    private GameObject playerObject;
    private Image uiWeaponImage;
    private Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        animator = youDiedText.GetComponent<Animator>();
        playerObject = GameObject.FindGameObjectWithTag("Player");
        var player = playerObject.GetComponent<Player>();
        cameraTransform = player.cameraTransform;
        healthBar = player.healthBar;
        loaderBorder = player.loaderBorder;
        dashLoaderBorder = player.dashLoaderBorder;
        uiWeaponImage = player.uiWeaponImage;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerObject == null)
        {
            youDiedText.SetActive(true);
            animator.SetTrigger("PlayerDied");
        }
    }

    public void RespawnPlayer()
    {
        youDiedText.SetActive(false);
        playerObject = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
        var followPlayer = playerFollower.GetComponent<FollowPlayer>();
        var player = playerObject.GetComponent<Player>();
        followPlayer.Player = playerObject;
        followPlayer.PlayerScript = player;
        playerFollower.transform.position = playerObject.transform.position;
        player.cameraTransform = cameraTransform;
        player.healthBar = healthBar;
        player.loaderBorder = loaderBorder;
        player.dashLoaderBorder = dashLoaderBorder;
        player.uiWeaponImage = uiWeaponImage;

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
}
