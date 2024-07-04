using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public enum EnemyState
{
    Aggro,
    Searching,
    Resting,
    Patrolling, // this is here if we want to add a patrol state, for now this state is never used
    Returning
}

public class Enemy : MonoBehaviour
{
    [SerializeField] private EnemyState defaultState = EnemyState.Resting; // resting or patrolling

    private EnemyState enemyState = EnemyState.Resting;
    private Vector3 startingPosition;
    private Vector3 playerPosition;
    private Vector3 targetPosition;

    private float maxHealth;
    private float currentHealth;
    private float attackDamage;
    private float attackCooldown = 5f;

    private float searchDuration = 10f;
    private float searchMoveDuration = 2f;
    private float movementSpeed = 10;
    private float attackRange = 5f; // can attack if this close to the player
    private float engagementRange = 3f; // will attempt to stay within this range of the player, must be smaller than attackRange
    private float aggroRange;
    private float searchRange = 15;
    private bool isSearching = false;
    private bool canAttack = true;

    private Timer attackCooldownTimer;
    private Timer searchTimer; // how long to search for the player after loosing them
    private Timer searchMoveTimer; //how long before moving to a new spot during a search

    // Start is called before the first frame update
    void Start()
    {
        startingPosition = transform.position;
        attackCooldownTimer = new Timer(this)
        {
            OnTimerElapsed = () =>
            {
                canAttack = true;
                return null;
            }
        };
        searchTimer = new Timer(this)
        {
            OnTimerElapsed = () =>
            {
                if (enemyState == EnemyState.Searching)
                {
                    targetPosition = startingPosition;
                    enemyState = EnemyState.Returning;
                }
                return null;
            }
        };
        searchMoveTimer = new Timer(this)
        {
            OnTimerElapsed = () =>
            {
                if (enemyState == EnemyState.Searching)
                {
                    isSearching = false;
                    var circlePoint = UnityEngine.Random.insideUnitCircle;
                    targetPosition = new Vector3(circlePoint.x * searchRange, transform.position.y, circlePoint.y * searchRange);
                    Debug.Log($"new search point: {targetPosition}");
                }
                return null;
            }
        };
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(targetPosition, Vector3.up * 5, UnityEngine.Color.blue);

        switch (enemyState)
        {
            case EnemyState.Aggro:
                if (Vector3.Distance(transform.position, playerPosition) <= attackRange)
                {
                    if (canAttack)
                        Attack();
                }

                if (Vector3.Distance(transform.position, playerPosition) <= engagementRange)
                {

                }
                else if (Vector3.Distance(targetPosition, playerPosition) <= engagementRange)
                {
                    TravelToPosition(targetPosition);
                }
                else
                {
                    ChooseTargetPosition();
                }
                break;

            case EnemyState.Searching:
                Debug.Log($"searching {transform.position == targetPosition} and {!isSearching}");
                if (transform.position == targetPosition && !isSearching)
                {
                    isSearching = true;
                    searchMoveTimer.Start(searchMoveDuration);
                    Debug.Log("starting searchMoveTimer");
                }
                else
                    TravelToPosition(targetPosition);
                break;

            case EnemyState.Resting:
                break;

            case EnemyState.Patrolling:
                break;

            case EnemyState.Returning:
                if (transform.position == startingPosition)
                    enemyState = defaultState;
                else
                    TravelToPosition(startingPosition);
                break;
        }
        // if (take damage) set state to searching and target position to
    }

    private void OnTriggerEnter(Collider other)
    {
        if (enemyState == EnemyState.Aggro)
            return;
        if (other.gameObject.CompareTag("Player"))
        {
            enemyState = EnemyState.Aggro;

            canAttack = false;
            attackCooldownTimer.Start(attackCooldown * 2);
        }
        // OPTION: enemy should detect player projectile and move to where the projectile entered its detection range to investigate
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            FindPlayerPosition();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (enemyState != EnemyState.Aggro)
            return;
        if (other.gameObject.CompareTag("Player"))
        {
            //player lost, go look for them
            enemyState = EnemyState.Searching;
            searchTimer.Start(searchDuration);
            targetPosition = playerPosition;
        }
    }

    void FindPlayerPosition()
    {
        var player = GameObject.FindGameObjectsWithTag("Player");
        if (player.Length > 0)
            playerPosition = player[0].transform.position;
    }

    void TravelToPosition(Vector3 moveTo)
    {
        //here we handle pathfinding
        transform.position = Vector3.MoveTowards(transform.position, moveTo, movementSpeed * Time.deltaTime);
    }

    void Attack()
    {
        Debug.Log("attacking player");

        canAttack = false;
        attackCooldownTimer.Start(attackCooldown);
    }

    void ChooseTargetPosition()
    {
        if (enemyState == EnemyState.Aggro)
        {
            var targetAngle = Math.Atan2((float)(transform.position.x - playerPosition.x), (float)(transform.position.z - playerPosition.z));
            var targetX = Mathf.Sin((float)targetAngle);
            var targetZ = Mathf.Cos((float)targetAngle);
            targetPosition = new Vector3(playerPosition.x + targetX * engagementRange, playerPosition.y, playerPosition.z + targetZ * engagementRange);
        }
        else if (enemyState == EnemyState.Searching)
        {

        }
        else if (enemyState == EnemyState.Returning)
        {
            targetPosition = startingPosition;
        }
        else if (enemyState == EnemyState.Patrolling)
        {

        }
    }
}
