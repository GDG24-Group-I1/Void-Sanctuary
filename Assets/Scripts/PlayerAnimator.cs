using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    [SerializeField] private Player player;

    private Animator animator;
    private const string IS_WALKING = "IsWalking";
    private const string IS_RUNNING = "IsRunning";
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        bool isWalking = player.IsWalking;
        animator.SetBool(IS_WALKING, isWalking);

        bool isRunning = player.IsRunning;
        animator.SetBool(IS_RUNNING, isRunning);

        //Debug.Log("IsWalking: " + isWalking + " IsRunning: " + isRunning);
    }
    
}
