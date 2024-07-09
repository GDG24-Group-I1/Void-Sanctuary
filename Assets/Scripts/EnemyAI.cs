using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform player;
    public LayerMask whatIsGround, whatIsPlayer;

    public float stopDistance;

    // Patroling
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    // States
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;

    // Attack
    public float timeBetweenAttacks;
    bool alreadyAttacked;
    private Vector3 playerPosition;

    public bool isMelee;

    // range attack
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float projectileHeight = 0.8f;
    [SerializeField] private float projectileRange = 15f;

    private void Awake()
    {
        player = GameObject.Find("Player").transform;
        playerPosition = player.transform.position;
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        // Update the detection of player
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if (!playerInSightRange && !playerInAttackRange)
        {
            Patroling();
        }
        else if (playerInSightRange && !playerInAttackRange)
        {
            ChasePlayer();
        }
        else if (playerInSightRange && playerInAttackRange)
        {
            AttackPlayer();
        }
    }

    private void Patroling()
    {
        if (!walkPointSet)
        {
            SearchWalkPoint();
        }

        if (walkPointSet)
        {
            agent.stoppingDistance = 0.0f;
            agent.SetDestination(walkPoint);
            Vector3 distanceToWalkPoint = transform.position - walkPoint;

            // Consider using a slightly larger range to account for inaccuracies
            if (distanceToWalkPoint.magnitude < 2.0f)
            {
                walkPointSet = false;
            }
        }
    }

    private void SearchWalkPoint()
    {
        // Calculate random point in range
        float randomZ = UnityEngine.Random.Range(-walkPointRange, walkPointRange);
        float randomX = UnityEngine.Random.Range(-walkPointRange, walkPointRange);
        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        // Check if point is on ground
        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
        {
            walkPointSet = true;
        }
    }

    private void ChasePlayer()
    {
        agent.stoppingDistance = stopDistance;
        agent.SetDestination(player.position);
    }

    private void AttackPlayer()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        agent.SetDestination(player.position);

        // Check if the enemy is close enough to stop and is actually stopped
        if (distanceToPlayer <= stopDistance + agent.stoppingDistance && !agent.pathPending)
        {
            // Ensure the agent has actually stopped moving
            if (agent.remainingDistance <= agent.stoppingDistance && !agent.hasPath || agent.velocity.sqrMagnitude == 0f)
            {
                Vector3 lookDirection = new Vector3(player.position.x, transform.position.y, player.position.z);
                transform.LookAt(lookDirection);

                if (!alreadyAttacked)
                {
                    if (isMelee)
                    {
                        Debug.Log("Melee attack");
                    }
                    else
                    {
                        // Perform ranged attack
                        RangedAttack();
                    }
                    alreadyAttacked = true;
                    Invoke(nameof(ResetAttack), timeBetweenAttacks);
                }
            }
        }
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
        Debug.Log("Resetting attack!");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, sightRange);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    private void RangedAttack()
    {
        Debug.Log("Ranged attacking player");

        Vector3 startingPosition = new Vector3(transform.position.x, projectileHeight, transform.position.z);

        Vector3 directionToPlayer = (player.position - transform.position).normalized;

        Vector3 endingPosition = transform.position + directionToPlayer * projectileRange;
        endingPosition.y = projectileHeight;

        Debug.Log($"Starting position: {startingPosition}, Ending position: {endingPosition}");

        Vector3 projectilePosition = startingPosition;

        // Instantiate the projectile
        GameObject projectile = Instantiate(projectilePrefab, projectilePosition, Quaternion.LookRotation(directionToPlayer));
        projectile.transform.Rotate(90, 0, 0);

        ProjectileScript projectileScript = projectile.GetComponent<ProjectileScript>();
        if (projectileScript != null)
        {
            projectileScript.endingPosition = endingPosition;
        }
        else
        {
            Debug.LogError("ProjectileScript is missing on the projectile prefab!");
        }
    }
}