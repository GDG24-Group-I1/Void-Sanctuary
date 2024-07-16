using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Text.RegularExpressions;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEngine.EventSystems.EventTrigger;

public enum EnemyState
{
    Aggro,
    Searching,
    Returning,
    Resting
}
public enum SearchState
{
    notSearching,
    calculatingNewPosition,
    Moving,
    Waiting
}

public class SwordEnemyBehavior : MonoBehaviour
{
    [SerializeField] private EnemyState enemyState = EnemyState.Resting;
    private SearchState searchState = SearchState.notSearching;
    private Vector3 startingPosition;
    private Vector3 playerPosition;
    private Vector3 targetPosition;

    private float maxHealth;
    private float currentHealth;
    private float attackDamage;

    [SerializeField] private float movementSpeed = 9f;
    [SerializeField] private float attackRange = 4.5f; // can attack if this close to the player
    [SerializeField] private float engagementRange = 3.5f; // will attempt to stay within this range of the player, must be smaller than attackRange
    private float angleAdjustment = 0;
    private float rangeAdjustment = 0;
    private float aggroRange;
    private float searchRange = 15f;
    private float minDistance = 0.1f; // used to calculate when a distance is 0 without the enemy breakdancing on the spot
    private bool canAttack = true;

    // timers and durations
    private Timer attackCooldownTimer;
    private float attackCooldown = 5f;
    private Timer searchTimer; // how long to search for the player after loosing them
    private float searchDuration = 10f;
    private Timer searchMoveTimer; //how long before moving to a new spot during a search
    private float searchMoveDuration = 2f;

    // movement and physics
    private Rigidbody rb;
    private bool isGrounded;
    private int frameNotGrounded;
    private int thresholdFrameGrounded = 1;
    private float groundDrag = 6f;

    // Start is called before the first frame update
    void Start()
    {
        targetPosition = transform.position;
        startingPosition = transform.position;
        rb = GetComponent<Rigidbody>();
        aggroRange = GetComponentInChildren<SphereCollider>().radius;

        attackCooldownTimer = new Timer()
        {
            OnTimerElapsed = () =>
            {
                canAttack = true;
                return null;
            }
        };
        searchTimer = new Timer()
        {
            OnTimerElapsed = () =>
            {
                if (enemyState == EnemyState.Searching)
                {
                    searchState = SearchState.notSearching;
                    enemyState = EnemyState.Returning;
                    targetPosition = startingPosition;
                }
                return null;
            }
        };
        searchMoveTimer = new Timer()
        {
            OnTimerElapsed = () =>
            {
                if (enemyState == EnemyState.Searching)
                    searchState = SearchState.calculatingNewPosition;
                else
                    searchState = SearchState.notSearching;
                return null;
            }
        };
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log($"state: {enemyState} - {searchState}");
        Debug.DrawRay(targetPosition, Vector3.up * 5, UnityEngine.Color.blue);
        switch (enemyState)
        {
            case EnemyState.Aggro:
                if (Vector3.Distance(transform.position, playerPosition) <= attackRange)
                    if (canAttack)
                        Attack();
                break;

            case EnemyState.Searching:
                var yAdjustment = transform.position.y - targetPosition.y;
                if (searchState == SearchState.Moving && Vector3.Distance(transform.position, targetPosition) < (minDistance + yAdjustment))
                {
                    searchState = SearchState.Waiting;
                    searchMoveTimer.Start(searchMoveDuration);
                }
                break;

            case EnemyState.Returning:
                break;

            case EnemyState.Resting:
                break;
        }
        // if (take damage) set state to searching and target position to
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerPosition = other.gameObject.transform.position;
            //if the view is obstructed, player is not detected
            if (!Physics.Linecast(transform.position, playerPosition))
            {
                enemyState = EnemyState.Aggro;
                canAttack = false;
                attackCooldownTimer.Start(attackCooldown);
            }
        }
        // OPTION: enemy should detect player projectile and move to where the projectile entered its detection range to investigate
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerPosition = other.gameObject.transform.position;
            if (!CheckWall(transform.position, playerPosition))
            {
                enemyState = EnemyState.Aggro;
            }
            else
            {
                if (enemyState == EnemyState.Aggro)
                {
                    enemyState = EnemyState.Searching;
                    searchState = SearchState.Moving;
                    searchTimer.Start(searchDuration);
                }
            }
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
            searchState = SearchState.Moving;
            searchTimer.Start(searchDuration);
        }
    }

    void Attack()
    {
        Debug.Log("attacking player");

        canAttack = false;
        attackCooldownTimer.Start(attackCooldown);
    }

    void HandleMovement()
    {
        if (searchState == SearchState.Waiting || enemyState == EnemyState.Resting)
            return;

        ChooseTargetPosition();


        // Get input for movement
        Vector3 moveDir;
        Vector3 rotateDir;
        var yAdjustment = transform.position.y - targetPosition.y;
        if (Vector3.Distance(targetPosition, transform.position) > (minDistance + yAdjustment))
            moveDir = (targetPosition - transform.position);
        else
        {
            moveDir = Vector3.zero;
        }
        moveDir.Normalize();
        moveDir.y = 0;
        rotateDir = moveDir;

        if (enemyState == EnemyState.Aggro && moveDir.x <= 0.1 && moveDir.z <= 0.1)
        {
            //look at player
            rotateDir = (playerPosition - transform.position);
            rotateDir.y = 0;
            rotateDir.Normalize();
        }

        // Handle ground detection

        // Handle movement
        Vector3 targetVelocity = moveDir * movementSpeed;

        Vector3 velocity = rb.velocity;
        Vector3 velocityChange = targetVelocity - velocity;

        // Clamp velocity change to prevent abrupt changes
        velocityChange.y = 0;

        if (frameNotGrounded == thresholdFrameGrounded)
        {
            velocityChange.y = -9.8f;
        }

        rb.AddForce(velocityChange, ForceMode.VelocityChange);

        // Apply drag if grounded
        rb.drag = frameNotGrounded < thresholdFrameGrounded ? groundDrag : 0.1f;


        // Smoothly rotate player towards movement direction
        float rotationSpeed = 15f;
        transform.forward = Vector3.Slerp(transform.forward, rotateDir, rotationSpeed * Time.deltaTime);
        
    }

    void ChooseTargetPosition()
    {
        float targetX;
        float targetZ;
        double targetAngle;
        Vector3 calculatedTargetPosition = transform.position;
        switch (enemyState)
        {
            case EnemyState.Aggro:
                if (Vector3.Distance(transform.position, playerPosition) > engagementRange)
                {
                    targetAngle = Math.Atan2((float)(transform.position.x - playerPosition.x), (float)(transform.position.z - playerPosition.z));
                    targetAngle += angleAdjustment;
                    targetX = Mathf.Sin((float)targetAngle) * (engagementRange + rangeAdjustment) + playerPosition.x;
                    targetZ = Mathf.Cos((float)targetAngle) * (engagementRange + rangeAdjustment) + playerPosition.z;
                    calculatedTargetPosition = new Vector3(targetX, playerPosition.y, targetZ);
                }
                break;

            case EnemyState.Searching:
                if (searchState == SearchState.Moving)
                    return;
                var circlePoint = UnityEngine.Random.insideUnitCircle;
                targetX = targetPosition.x + circlePoint.x * UnityEngine.Random.Range(searchRange / 2, searchRange);
                targetZ = targetPosition.z + circlePoint.y * UnityEngine.Random.Range(searchRange / 2, searchRange);
                calculatedTargetPosition = new Vector3(targetX, transform.position.y, targetZ);
                Debug.Log($"calculatedTargetPosition: {calculatedTargetPosition}");
                searchState = SearchState.Moving;
                break;

            case EnemyState.Returning:
                calculatedTargetPosition = startingPosition;
                break;
        }

        if (!CheckWall(transform.position, calculatedTargetPosition))
        {
            var hasHit = Physics.Raycast(calculatedTargetPosition, Vector3.down, out RaycastHit _, 5f);
            if (!hasHit)
            {
                Debug.Log($"invalid targetPosition");
                rangeAdjustment += UnityEngine.Random.Range(0, engagementRange);
                angleAdjustment += UnityEngine.Random.Range(-45, 45);
            }
            else
            {
                rangeAdjustment = 0;
                angleAdjustment = 0;
                targetPosition = calculatedTargetPosition;
            }
        }
    }

    bool CheckWall(Vector3 origin, Vector3 target)
    {
        var mask = LayerMask.GetMask("wallLayer");
        var line = Physics.Linecast(origin, target, mask);
        return line;
    }
}
