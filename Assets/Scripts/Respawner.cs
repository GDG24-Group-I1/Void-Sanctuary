using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Respawner : MonoBehaviour
{

    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject youDiedText;
    private Transform cameraDirection;
    private Transform cameraTransform;
    private GameObject healthBar;
    private GameObject playerObject;
    private CinemachineVirtualCamera virtualCamera;
    private Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        animator = youDiedText.GetComponent<Animator>();
        playerObject = GameObject.FindGameObjectWithTag("Player");
        virtualCamera = GameObject.Find("TopDownCamera").GetComponent<CinemachineVirtualCamera>();
        var player = playerObject.GetComponent<Player>();
        cameraDirection = player.cameraDirection;
        cameraTransform = player.cameraTransform;
        healthBar = player.healthBar;
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
        var player = playerObject.GetComponent<Player>();
        player.cameraDirection = cameraDirection;
        player.cameraTransform = cameraTransform;
        player.healthBar = healthBar;
        virtualCamera.Follow = playerObject.transform;
        virtualCamera.LookAt = playerObject.transform;
    } 
}
