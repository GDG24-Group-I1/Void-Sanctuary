using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum PlayerState
{
    IdleWithWeapon = 4,
    WalkingWithWeapon = 5,
    RunningWithWeapon = 6,
    Attacking = 7,
    FallingTransition = 8,
    Falling = 9,
    Dashing = 10,
    AimingWithWeapon = 11,
}

public class PlayerAnimator : MonoBehaviour
{
    private Player player;
    private Animator animator;
    private const string PLAYER_STATE = "PlayerState";
    private const string ATTACK_NUMBER = "AttackNumber";
    private PlayerState currentState;
    private int previousAttackNumber = 0;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        currentState = PlayerState.IdleWithWeapon;
        player = gameObject.GetComponentInParent<Player>();
    }

    private void Update()
    {
        PlayerState newState = GetPlayerState();

        if (newState != currentState)
        {
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
        bool isAttacking = player.IsAttacking;
        bool isAiming = player.firingStage != FiringStage.notFiring;

        if (player.IsFalling == AnimationState.Transition) return PlayerState.FallingTransition;

        if (player.IsFalling == AnimationState.Playing) return PlayerState.Falling;

        if (isAiming)
        {
            return PlayerState.AimingWithWeapon;
        }
        else if (isAttacking)
        {
            return PlayerState.Attacking;
        }
        else if (isRunning)
        {
            return PlayerState.RunningWithWeapon;
        }
        else if (isWalking)
        {
            return PlayerState.WalkingWithWeapon;
        }
        else
        {
            return PlayerState.IdleWithWeapon;
        }

    }

    public Player GetPlayer()
    {
        return player;
    }

    public void SetDash()
    {
        animator.SetTrigger("Dashing");
    }

    public void SetPickup()
    {
        animator.SetTrigger("Pickup");
    }
}

