using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum PlayerState
{
    Idle = 0,
    Walking = 1,
    Running = 2,
    DrawingWeapon = 3,
    IdleWithWeapon = 4,
    WalkingWithWeapon = 5,
    RunningWithWeapon = 6,
}

public class PlayerAnimator : MonoBehaviour
{
    [SerializeField] private Player player;

    private Animator animator;
    private const string IS_WALKING = "IsWalking";
    private const string IS_RUNNING = "IsRunning";
    private const string IS_WEAPON_EQUP = "IsWeaponEquipped";
    private const string PLAYER_STATE = "PlayerState";
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {

        animator.SetInteger(PLAYER_STATE, GetPlayerState());

    }

    public Player GetPlayer()
    {
        return player;
    }

    private int GetPlayerState()
    {
        bool isWalking = player.IsWalking;

        bool isRunning = player.IsRunning;

        bool isWeaponEquip = player.IsWeaponEquipped;

        if (isWalking)
        {
            if (isWeaponEquip)
            {
                return (int)PlayerState.WalkingWithWeapon;
            }
            else
            {
                return (int)PlayerState.Walking;
            }
        }
        else if (isRunning)
        {
            if (isWeaponEquip)
            {
                return (int)PlayerState.RunningWithWeapon;
            }
            else
            {
                return (int)PlayerState.Running;
            }
        } else 
        {
            if (isWeaponEquip)
            {
                return (int)PlayerState.IdleWithWeapon;
            }
            else
            {
                return (int)PlayerState.Idle;
            }
        }
    }
}
