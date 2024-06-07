using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Respawner : MonoBehaviour
{

    [SerializeField] private GameObject playerPrefab;
    private GameInput gameInput;
    private Transform cameraDirection;
    private Transform cameraTransform;
    private GameObject playerObject;
    private CinemachineVirtualCamera virtualCamera;
    // Start is called before the first frame update
    void Start()
    {
        playerObject = GameObject.FindGameObjectWithTag("Player");
        virtualCamera = GameObject.Find("TopDownCamera").GetComponent<CinemachineVirtualCamera>();
        var player = playerObject.GetComponent<Player>();
        gameInput = player.gameInput;
        cameraDirection = player.cameraDirection;
        cameraTransform = player.cameraTransform;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerObject == null)
        {
            playerObject = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
            var player = playerObject.GetComponent<Player>();
            player.gameInput = gameInput;
            player.cameraDirection = cameraDirection;
            player.cameraTransform = cameraTransform;
            virtualCamera.Follow = playerObject.transform;
            virtualCamera.LookAt = playerObject.transform;
        }
    }
}
