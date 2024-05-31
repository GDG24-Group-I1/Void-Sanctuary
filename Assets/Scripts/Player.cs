using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Windows;

public class Player : MonoBehaviour
{
    enum FiringStage { notFiring, startCharging, charging, firing, knockback }

    private const float firingKnockbackSpeed = 50f;
    private const int maxWallsCollided = 10;
    [SerializeField] private float movementSpeed = 20f;
    [SerializeField] private float groundDrag = 6f;
    [SerializeField] private GameInput gameInput;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private GameObject swordPrefab;
    [SerializeField] private Transform camera_direction;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private LayerMask wallLayer;

    private Rigidbody rb;
    private LayerMask groundLayer;
    private bool isGrounded;
    private CapsuleCollider playerCollider;
    private Collider[] previousWallsCollided = Array.Empty<Collider>();
    private RaycastHit[] wallsCollided = new RaycastHit[maxWallsCollided];

    public bool IsWalking { get; private set; }

    private bool canMove = true;
    private bool canAct = true;
    private bool canFire = true;
    private bool canAttack = true;
    private Timer movementCooldownTimer;
    private Timer actionCooldownTimer;
    private Timer fireCooldownTimer;
    private Timer attackCooldownTimer;
    private Timer attackComboTimer;
    private Timer firingStageCooldown;
    private Timer deathTimer;
    private int attackComboCounter = 1;
    private FiringStage firingStage = FiringStage.notFiring;
    private Vector3 startingPosition;


    private void Start()
    {
        startingPosition = transform.position;
        rb = GetComponent<Rigidbody>();
        deathTimer = new Timer(this)
        {
            OnTimerElapsed = () =>
            {
                // Reset player position, for now
                // Later, add death logic here
                Debug.Log("Player died");
                transform.position = startingPosition;
            }
        };
        var floorCollider = GetComponent<FloorCollider>();
        floorCollider.CollisionEnterCallback = () =>
        {
            deathTimer?.Stop();
            isGrounded = true;
        };
        floorCollider.CollisionExitCallback = () =>
        {
            Debug.Log("Start death timer");
            deathTimer?.Start(5.0f);
            isGrounded = false;
        };
        rb.freezeRotation = true;
        gameInput.OnAttack = (context) =>
        {
            Attack();
        };
        gameInput.OnFire = (context) =>
        {
            Fire();
        };
        gameInput.OnBlock = (context) =>
        {
            Block();
        };
        movementCooldownTimer = new Timer(this)
        {
            OnTimerElapsed = () =>
            {
                canMove = true;
            }
        };
        actionCooldownTimer = new Timer(this)
        {
            OnTimerElapsed = () =>
            {
                canAct = true;
            }
        };
        fireCooldownTimer = new Timer(this)
        {
            OnTimerElapsed = () =>
            {
                canFire = true;
            }
        };
        attackCooldownTimer = new Timer(this)
        {
            OnTimerElapsed = () =>
            {
                canAttack = true;
            }
        };
        firingStageCooldown = new Timer(this)
        {
            OnTimerElapsed = () =>
            {
                firingStage += 1;
                if (firingStage > FiringStage.knockback)
                    firingStage = FiringStage.notFiring;
            }
        };
        attackComboTimer = new Timer(this)
        {
            OnTimerElapsed = () =>
            {
                attackComboCounter = 1;
            }
        };
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    private void Update()
    {
        FiringSequence();
        CheckIfPlayerIsHidden();
    }

    private static void SetRendererOpacity(GameObject obj, float alpha)
    {
        var renderer = obj.GetComponent<Renderer>();
        var color = renderer.material.GetColor("_Color");
        renderer.material.SetColor("_Color", new Color(color.r, color.g, color.b, alpha));
    }

    private IEnumerable<Collider> FilterLostWalls(int hits)
    {
        return previousWallsCollided.Where(x => !wallsCollided.Take(hits).Select(x => x.collider).Contains(x));
    }

    private void UpdateCollidedWalls(int hits)
    {
        previousWallsCollided =  wallsCollided.Take(hits).Select(x => x.collider).Where(x => x != null).ToArray();
    }

    private void CheckIfPlayerIsHidden()
    {
        Vector3 cameraPosition = cameraTransform.position;
        Vector3 directionToPlayer = new Vector3(transform.position.x, 1, transform.position.z) - cameraPosition;
        float distanceToPlayer = Vector3.Distance(cameraPosition, transform.position);

        // Debug.DrawRay(cameraPosition, directionToPlayer, Color.red);

        var hits = Physics.RaycastNonAlloc(cameraPosition, directionToPlayer, wallsCollided, distanceToPlayer, wallLayer);
        for (int i = 0; i < hits; ++i)
        {
            var hit = wallsCollided[i];
            var wasAlreadyCollidedPreviousFrame = previousWallsCollided.Contains(hit.collider);
            if (!wasAlreadyCollidedPreviousFrame)
            {
                SetRendererOpacity(hit.collider.gameObject, 0.5f);
            }
        }
        foreach (var oldWall in FilterLostWalls(hits))
        {
            // Reset the color of the wall
            SetRendererOpacity(oldWall.gameObject, 1.0f);
        }
        UpdateCollidedWalls(hits);
    }

    private void HandleMovement()
    {
        // Get input for movement
        Vector2 movementVector = gameInput.GetMovementVectorNormalized();
        Vector3 moveDir = new Vector3(movementVector.x, 0, movementVector.y);
        moveDir = camera_direction.forward * moveDir.z + camera_direction.right * moveDir.x;
        moveDir.y = 0;

        // Check if movement is disabled
        if (!canMove)
        {
            moveDir = Vector3.zero;
        }


        // Handle ground detection

        // Handle movement
        Vector3 targetVelocity = moveDir * movementSpeed;
        Vector3 velocity = rb.velocity;
        Vector3 velocityChange = targetVelocity - velocity;

        // Clamp velocity change to prevent abrupt changes
        velocityChange.y = 0;

        rb.AddForce(velocityChange, ForceMode.VelocityChange);

        // Apply drag if grounded
        rb.drag = isGrounded ? groundDrag : 0;

        // Check if the player is walking
        IsWalking = moveDir != Vector3.zero;

        // Smoothly rotate player towards movement direction

        float rotationSpeed = 20f;
        transform.forward = Vector3.Slerp(transform.forward, moveDir, Time.deltaTime * rotationSpeed);

    }

    private void Attack()
    {
        if (!canAttack || !canAct)
            return;

        // spawn sword at right offset and angle from player
        float swordRange = 5f;

        Vector3 playerFacing = transform.forward;

        Quaternion swordRotation = Quaternion.Euler(90f, transform.rotation.eulerAngles.y, 180f);
        Vector3 swordPosition = new(transform.position.x + swordRange * playerFacing.x, transform.position.y - 1.0f, transform.position.z + swordRange * playerFacing.z); ;

        GameObject sword = Instantiate(swordPrefab, swordPosition, swordRotation);

        var swordScript = sword.GetComponent<SwordScript>();
        swordScript.pivot = transform.position;
        swordScript.combo = attackComboCounter;

        Debug.Log($"Combo {attackComboCounter}");

        if (attackComboCounter < 3)
        {
            attackComboCounter += 1;
            canAttack = false;
            attackCooldownTimer.Start(0.4f);
            attackComboTimer.Start(1.0f);
        }
        else
        {
            attackComboCounter = 1;
            canAttack = false;
            attackCooldownTimer.Start(0.8f);
            attackComboTimer.Start(0.0f);
        }

        canAct = false;
        actionCooldownTimer.Start(0.4f);

        canMove = false;
        movementCooldownTimer.Start(0.5f);
    }

    private void Fire()
    {

        if (!canFire || !canAct)
            return;

        firingStage = FiringStage.startCharging;
    }

    private void FiringSequence()
    {
        Vector3 playerFacing = transform.forward;
        switch (firingStage)
        {
            //stop movement while charging projectile
            case FiringStage.startCharging:
                canMove = false;
                movementCooldownTimer.Start(1.8f);
                canAct = false;
                actionCooldownTimer.Start(1.8f);
                firingStage = FiringStage.charging;
                firingStageCooldown.Start(1.0f);
                break;
            // charging
            case FiringStage.charging:
                break;
            //fire projectile
            case FiringStage.firing:
                var projectileSpawnDistance = 3f;
                Vector3 projectilePosition = new Vector3(
                    transform.position.x + projectileSpawnDistance * playerFacing.x,
                    transform.position.y + 1.5f,
                    transform.position.z + projectileSpawnDistance * playerFacing.z);

                GameObject projectile = Instantiate(projectilePrefab, transform.position, transform.rotation);
                var projectileScript = projectile.GetComponent<ProjectileScript>();
                projectileScript.facing = transform.forward;

                canFire = false;
                fireCooldownTimer.Start(3.0f);

                canAct = false;
                actionCooldownTimer.Start(1.0f);

                canMove = false;
                movementCooldownTimer.Start(0.4f);

                firingStageCooldown.Start(0.1f);
                firingStage = FiringStage.knockback;
                break;
            //knockback
            case FiringStage.knockback:
                float knockback = firingKnockbackSpeed * Time.deltaTime;
                transform.position += new Vector3(knockback * -playerFacing.x, 0.0f, knockback * -playerFacing.z);
                break;
            default:
                break;
        }
    }

    private void Block()
    {

    }
}
