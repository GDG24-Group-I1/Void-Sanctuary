using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody), typeof(AudioSource))]
public class EnemyAI : MonoBehaviour
{
    private AudioSource audioSource;

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
            if (Type != EnemyType.Boss)
            {
                animator.SetBool("Frozen", value);
            }
        }
    }

    private float stopRange;
    private float attackRange;

    // Patroling
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    // States
    public float sightRange;
    [SerializeField] private float attackingDistanceMelee;
    [SerializeField] private float stoppingDistanceMelee;
    [SerializeField] private float attackingDistanceRanged;
    [SerializeField] private float stoppingDistanceRanged;
    private bool playerInSightRange, playerInAttackRange;

    // Attack
    private bool canAttack = true;
    [SerializeField] private float attackCooldown;
    public Timer attackCooldownTimer;
    public bool animationEnded = false;

    // range attack
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float projectileHeight = 0.8f;
    [SerializeField] private float projectileRange = 15f;
    [SerializeField] private float lingerTimeAfterDeath = 5f;
    [SerializeField] private float distanceSwitchTypeAttack = 10f;

    // melee attack
    [SerializeField] private BoxCollider leftArmCollider;
    [SerializeField] private BoxCollider rightArmCollider;

    // enemy type
    public EnemyType Type;

    // combat
    [SerializeField] private float health = 3;
    public float staggerDuration = 1f;
    public Timer staggerTimer;
    Animator animator;
    private Rigidbody rb;
    private bool isMeleeAttacking;

    //
    Renderer[] renderers;

    // only for boss
    private Slider healthBar;
    private OpenDoors bossDoor;

    [Header("Sounds")]
    [SerializeField] private AudioClip attackSound;
    [SerializeField] private AudioClip deathSound;
    [SerializeField] private AudioClip activationSound;
    [SerializeField] private AudioClip hitSound;

    [Header("Sounds")]
    [SerializeField] private AudioClip bossMeleeAttackSound;
    [SerializeField] private AudioClip bossRangedAttackSound;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        if (Type == EnemyType.Boss)
        {
            healthBar = GameObject.Find("GameUI").transform.Find("BossHealthBar").GetComponent<Slider>();
            healthBar.gameObject.SetActive(false);
        } else
        {
            healthBar = null;
        }
    }

    private void OnEnable()
    {
        if (healthBar != null)
        {
            healthBar.gameObject.SetActive(true);
            healthBar.minValue = 0;
            healthBar.maxValue = health;
            healthBar.value = health;
            healthBar.wholeNumbers = true;
        }
    }

    private void OnDisable()
    {
        if (healthBar != null)
        {
            healthBar.gameObject.SetActive(false);
        }
    }

    private void Start()
    {
        agent.autoRepath = true;
        agent.updatePosition = true;
        agent.updateRotation = false;
        if (Type == EnemyType.Boss)
        {
            bossDoor = GameObject.Find("boss_door").GetComponent<OpenDoors>(); 
        }
        audioSource = GetComponent<AudioSource>();
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
        if (IsFrozen || isStaggered || health <= 0 || player == null) return;
        // Update the detection of player

        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer > distanceSwitchTypeAttack && Type == EnemyType.Boss)
        {
            attackRange = attackingDistanceRanged;
            stopRange = stoppingDistanceRanged;
            isMeleeAttacking = false;
        }
        else if (distanceToPlayer <= distanceSwitchTypeAttack && Type == EnemyType.Boss)
        {
            stopRange = stoppingDistanceMelee;
            attackRange = attackingDistanceMelee;
            isMeleeAttacking = true;
        }
        if (Type != EnemyType.Boss)
        {
            if (Type == EnemyType.Melee)
            {
                stopRange = stoppingDistanceMelee;
                attackRange = attackingDistanceMelee;
            }
            else if (Type == EnemyType.Ranged)
            {
                stopRange = stoppingDistanceRanged;
                attackRange = attackingDistanceRanged;
            }
        }

        if (!playerInSightRange && !playerInAttackRange)
        {
            HandleEnemyRotation(agent.destination - transform.position);
            Patroling();
        }
        else if (playerInSightRange && !playerInAttackRange)
        {
            HandleEnemyRotation(player.position - transform.position);
            ChasePlayer();
        }
        else if (playerInSightRange && playerInAttackRange)
        {
            HandleEnemyRotation(player.position - transform.position);
            AttackPlayer();
        }
    }

    private void Patroling()
    {
        if (Type == EnemyType.Boss)
        {
            animator.SetBool("isWalking", false);
            agent.isStopped = true;
        }

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

    private void HandleEnemyRotation(Vector3 direction)
    {
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
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
        if (Type == EnemyType.Boss)
        {
            animator.SetBool("isWalking", true);
        }

        agent.isStopped = false;
        agent.stoppingDistance = stopRange;

        if (player != null)
        {
            agent.SetDestination(player.position);
        }
    }

    private void AttackPlayer()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        agent.SetDestination(player.position);  // Continuously update the destination to the player's position

        // Check if the enemy is within the effective stop distance and has stopped moving
        if (distanceToPlayer <= stopRange + agent.stoppingDistance && agent.remainingDistance <= agent.stoppingDistance)
        {
            transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z)); // Look at player ignoring y difference

            if (canAttack)
            {
                agent.isStopped = true;
                DetermineAttackType(distanceToPlayer);
                canAttack = false;
                attackCooldownTimer.Start(attackCooldown);
            }
            else
            {
                agent.isStopped = false;
            }
        }
    }
    private void DetermineAttackType(float distanceToPlayer)
    {
        if (Type == EnemyType.Melee)  // Melee enemy
        {
            MeleeAttack();
        }
        else if (Type == EnemyType.Ranged)  // Regular ranged enemy
        {
            RangedAttack();
        }
        else if (Type == EnemyType.Boss)  // Boss
        {
            if (isMeleeAttacking)
            {
                Debug.Log("Switching to melee attack");
                MeleeAttack();
            }
            else
            {
                Debug.Log("Switching to ranged attack");
                BossRangedAttack();
            }
        }
    }

    public void SetSwordSolidity(bool isSolid)
    {
        if (Type == EnemyType.Ranged) return;
        leftArmCollider.enabled = isSolid;
        rightArmCollider.enabled = isSolid;
    }

    public void StaggerFromHit()
    {
        if (healthBar != null && Type == EnemyType.Boss)
        {
            healthBar.value = health;
        }
        agent.isStopped = true;
        rb.MovePosition(transform.position + transform.forward * -1);
        if (health <= 0) return;
        if (Type == EnemyType.Boss)
        {
            bossDoor.Input(-1);
        }
        audioSource.PlayOneShot(hitSound);
        isStaggered = true;
        staggerTimer.Start(staggerDuration);
        InvokeRepeating(nameof(FlashEnemy), 0f, 0.1f);
    }

    public void Unfreeze()
    {
        if (isFrozen)
        {
            var iceCube = GetComponentInChildren<IceCubeScript>();
            if (iceCube != null)
            {
                Destroy(iceCube.gameObject);
                isFrozen = false;
            }
        }
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
        if (Type == EnemyType.Boss)
        {
            audioSource.PlayOneShot(bossMeleeAttackSound);
        }
        else
        {
            audioSource.PlayOneShot(attackSound);
        }
        animator.SetTrigger("Attack");
    }

    private void RangedAttack()
    {
        audioSource.PlayOneShot(attackSound);
        animator.SetTrigger("Attack");
        Vector3 projectilePosition = new(transform.position.x, transform.position.y + projectileHeight, transform.position.z);

        // Central projectile
        ShootProjectile(projectilePosition, (player.position - transform.position).normalized);
    }

    private void BossRangedAttack()
    {
        audioSource.PlayOneShot(bossRangedAttackSound);
        Vector3 projectilePosition = new(transform.position.x, transform.position.y + projectileHeight, transform.position.z);

        // Central projectile
        ShootProjectile(projectilePosition, (player.position - transform.position).normalized);

        // Left projectile with a slight rotation
        ShootProjectile(projectilePosition, Quaternion.Euler(0, -30, 0) * (player.position - transform.position).normalized);

        // Right projectile with a slight rotation
        ShootProjectile(projectilePosition, Quaternion.Euler(0, 30, 0) * (player.position - transform.position).normalized);
    }

    private void ShootProjectile(Vector3 startPosition, Vector3 direction)
    {
        Vector3 endingPosition = startPosition + direction * projectileRange;
        endingPosition.y += projectileHeight;

        GameObject projectile = Instantiate(projectilePrefab, startPosition, Quaternion.LookRotation(direction));
        projectile.transform.Rotate(90, 0, 0);  // Adjust rotation for the projectile's model if necessary

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
        if (Type == EnemyType.Boss && isStaggered) return;
        if (health <= 0) return;
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
            audioSource.PlayOneShot(deathSound);
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
        if (Type == EnemyType.Boss)
        {
            player.GetComponent<Player>().TriggerDialog("BossDefeat");
            transform.parent.GetComponentInChildren<BossLever>().SetEnabled(true);
            bossDoor.Input(int.MaxValue);
        }
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
        if (Type == EnemyType.Boss && isStaggered) return;
        if (health <= 0) return;
        if (collision.gameObject.name == "Projectile(Clone)")
        {
            health -= 2;
            StaggerFromHit();
        }
        if (health <= 0)
        {
            if (healthBar != null && Type == EnemyType.Boss)
            {
                healthBar.gameObject.SetActive(false);
            }
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