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
    Attacking = 7,
    FallingTransition = 8,
    Falling = 9,
    Dashing = 10,
}

public class PlayerAnimator : MonoBehaviour
{
    [SerializeField] private Player player;

    private Animator animator;
    private const string PLAYER_STATE = "PlayerState";
    private const string ATTACK_NUMBER = "AttackNumber";
    private PlayerState currentState;
    private PlayerState previousState;
    private int previousAttackNumber = 0;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        currentState = PlayerState.Idle;
        previousState = PlayerState.Idle;
    }

    private void Update()
    {
        PlayerState newState = GetPlayerState();
        if (newState != currentState)
        {
            previousState = currentState;
            currentState = newState;
            animator.SetInteger(PLAYER_STATE, (int)currentState);
        }
        if (previousAttackNumber != player.AttackNumber)
        {
            previousAttackNumber = player.AttackNumber;
            animator.SetInteger(ATTACK_NUMBER, player.AttackNumber);
        }
    }

    private PlayerState GetPlayerState()
    {
        bool isWalking = player.IsWalking;
        bool isRunning = player.IsRunning;
        bool isWeaponEquipped = player.IsWeaponEquipped;
        bool isAttacking = player.IsAttacking;

        if (player.IsFalling == AnimationState.Transition) return PlayerState.FallingTransition;

        if (player.IsFalling == AnimationState.Playing) return PlayerState.Falling;

        if (player.IsDashing) return PlayerState.Dashing;

        if (isWeaponEquipped)
        {
            animator.SetBool("WeaponEquipped", true);
            if(isAttacking)
            {
                return PlayerState.Attacking;
            }
            else if(isRunning)
            {
                return PlayerState.RunningWithWeapon;
            }
            else if(isWalking)
            {
                return PlayerState.WalkingWithWeapon;
            }
            else
            {
                return PlayerState.IdleWithWeapon;
            }
        }
        else
        {
            animator.SetBool("WeaponEquipped", false);
            if(isRunning)
            {
                return PlayerState.Running;
            }
            else if(isWalking)
            {
                return PlayerState.Walking;
            }
            else
            {
                return PlayerState.Idle;
            }
        }
        
    }

    public Player GetPlayer()
    {
        return player;
    }
}

