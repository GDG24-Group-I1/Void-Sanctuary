using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Rigidbody))]
public class EnemyAI : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform player;
    public LayerMask whatIsGround, whatIsPlayer;

    private bool isStaggered;
    private bool isFrozen;
    public bool IsFrozen
    {
        get => isFrozen; set
        {
            agent.isStopped = value;
            isFrozen = value;
            animator.SetBool("Frozen", value);
        }
    }

    public float stopDistance;

    // Patroling
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    // States
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;

    // Attack
    private bool canAttack = true;
    private float attackCooldown = 2f;
    public Timer attackCooldownTimer;
    public bool animationEnded = false;
    public bool isMelee;

    // range attack
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float projectileHeight = 0.8f;
    [SerializeField] private float projectileRange = 15f;
    [SerializeField] private float lingerTimeAfterDeath = 5f;

    // melee attack
    [SerializeField] private BoxCollider leftArmCollider;
    [SerializeField] private BoxCollider rightArmCollider;

    // enemy type
    public EnemyType Type;

    // combat
    public float health = 3;
    public float staggerDuration = 1f;
    public Timer staggerTimer;
    Animator animator;
    private Rigidbody rb;

    //
    Renderer[] renderers;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        if (Type == EnemyType.Melee)
        {
            leftArmCollider.enabled = false;
            rightArmCollider.enabled = false;
        }
        renderers = GetComponentsInChildren<Renderer>();
        animator = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody>();
        IsFrozen = false;
        attackCooldownTimer = new Timer(this)
        {
            OnTimerElapsed = () =>
            {
                Debug.Log($"canAttack");
                canAttack = true;
                return null;
            }
        };
        staggerTimer = new Timer(this)
        {
            OnTimerElapsed = () =>
            {
                CancelInvoke(nameof(FlashEnemy));
                foreach (var renderer in renderers)
                {
                    renderer.enabled = true;
                }
                isStaggered = false;
                agent.isStopped = false;
                return null;
            }
        };
    }

    private void Update()
    {
        if (IsFrozen || isStaggered || health <= 0) return;
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
        if (player != null)
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

                if (canAttack)
                {
                    if (isMelee)
                    {
                        MeleeAttack();
                    }
                    else
                    {
                        animator.SetTrigger("Attack");
                        RangedAttack();
                    }
                    canAttack = false;
                    attackCooldownTimer.Start(attackCooldown);
                }
            } else // agent is already close to the player, but still moving, make it stop
            {
                // TODO: figure out if this is ok.
                agent.SetDestination(transform.position);
            }
        }
    }

    public void SetSwordSolidity(bool isSolid)
    {
        if (Type != EnemyType.Melee) return;
        leftArmCollider.enabled = isSolid;
        rightArmCollider.enabled = isSolid;
    }

    public void StaggerFromHit()
    {
        agent.isStopped = true;
        rb.MovePosition(transform.position + transform.forward * -1);
        if (health <= 0) return;
        isStaggered = true;
        staggerTimer.Start(staggerDuration);
        InvokeRepeating(nameof(FlashEnemy), 0f, 0.1f);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, sightRange);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    private void MeleeAttack()
    {
        animator.SetTrigger("Attack");
    }

    private void RangedAttack()
    {
        //Debug.Log("Ranged attacking player");

        Vector3 projectilePosition = new(transform.position.x, transform.position.y + projectileHeight, transform.position.z);

        Vector3 directionToPlayer = (player.position - transform.position).normalized;

        Vector3 endingPosition = transform.position + directionToPlayer * projectileRange;
        endingPosition.y += projectileHeight;

        //Debug.Log($"Starting position: {startingPosition}, Ending position: {endingPosition}");

        // Instantiate the projectile
        GameObject projectile = Instantiate(projectilePrefab, projectilePosition, Quaternion.LookRotation(directionToPlayer));
        projectile.transform.Rotate(90, 0, 0);

        if (projectile.TryGetComponent<ProjectileScript>(out var projectileScript))
        {
            projectileScript.SetTarget(endingPosition);
        }
        else
        {
            Debug.LogError("ProjectileScript is missing on the projectile prefab!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isStaggered || health <= 0) return;
        if (other.gameObject.CompareTag("Sword"))
        {
            health -= 1;
            StaggerFromHit();
        }

        if (health <= 0)
        {
            canAttack = false;
            agent.angularSpeed = 0;
            agent.isStopped = true;
            animator.SetTrigger("Death");
            StartCoroutine(EnemyFlashDeath());
            Destroy(gameObject, lingerTimeAfterDeath);
        }
    }


    private void FlashEnemy()
    {
        foreach (var renderer in renderers)
        {
            renderer.enabled = !renderer.enabled;
        }
    }

    private IEnumerator EnemyFlashDeath()
    {
        const float waitBeforeFlashing = 2f;
        float realLingerTimeAfterDeath = lingerTimeAfterDeath - waitBeforeFlashing;
        int flashCount = 0;
        int currentInterval = 0;
        float[] flashIntervals = { 0.3f, 0.2f, 0.1f };
        float intervalDuration = realLingerTimeAfterDeath / flashIntervals.Length;
        int[] intervalsCount = flashIntervals.Select(x => (int)(intervalDuration / x)).ToArray();
        yield return new WaitForSeconds(waitBeforeFlashing);
        while (currentInterval < flashIntervals.Length)
        {
            yield return new WaitForSeconds(flashIntervals[currentInterval]);
            FlashEnemy();
            flashCount++;
            if (flashCount >= intervalsCount[currentInterval])
            {
                currentInterval++;
                flashCount = 0;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isStaggered || health <= 0) return;
        if (collision.gameObject.name == "Projectile(Clone)")
        {
            health -= 2;
            StaggerFromHit();
        }
        if (health <= 0)
        {
            canAttack = false;
            agent.angularSpeed = 0;
            agent.isStopped = true;
            animator.SetTrigger("Death");
            StartCoroutine(EnemyFlashDeath());
            Destroy(gameObject, lingerTimeAfterDeath);
        }
    }

    public EnemyAI GetEnemy()
    {
        return this;
    }

    public void OnDestroy()
    {
        StopAllCoroutines();
    }
}