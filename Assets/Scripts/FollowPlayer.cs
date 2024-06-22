using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Unity.VisualScripting;
using UnityEngine.Assertions;

public class FollowPlayer : MonoBehaviour
{
    public GameObject Player;
    [SerializeField] private float MaxAdjustment;
    [SerializeField] private float yDiffThreshold;

    private bool adjustingCamera;
    public Player PlayerScript { get; set; }

    void Start()
    {
        transform.position = Player.transform.position;
        PlayerScript = Player.GetComponent<Player>();
    }

    void Update()
    {
        if (Player == null)
        {
            return;
        }
        transform.rotation = Player.transform.rotation;
        var playerPosition = Player.transform.position;
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
}
