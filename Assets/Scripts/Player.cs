using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

public class Player : MonoBehaviour
{
    enum FiringStage { notFiring, aiming, startCharging, charging, firing, knockback }

    private const float firingKnockbackSpeed = 50f;
    [SerializeField] private float movementSpeed = 20f;
    [SerializeField] private float groundDrag = 6f;
    [SerializeField] private GameInput gameInput;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private GameObject swordPrefab;
    [SerializeField] private Transform camera_direction;
    private LineRenderer aimLaserRenderer;

    private Rigidbody rb;
    private LayerMask groundLayer;
    private bool isGrounded;
    private CapsuleCollider playerCollider;
    private bool isWalking;

    private bool canMove = true;
    private bool canTurn = true;
    private bool canAct = true;
    private bool canFire = true;
    private bool canAttack = true;
    private bool canDash = true;
    private Timer movementCooldownTimer;
    private Timer turningCooldownTimer;
    private Timer actionCooldownTimer;
    private Timer fireCooldownTimer;
    private Timer attackCooldownTimer;
    private Timer attackComboTimer;
    private Timer firingStageCooldown;
    private Timer dashCooldownTimer;
    private int attackComboCounter = 1;
    private FiringStage firingStage = FiringStage.notFiring;



    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        //setting up the aim laser
        aimLaserRenderer = GetComponent<LineRenderer>();
        aimLaserRenderer.positionCount = 2;
        aimLaserRenderer.SetPosition(0, new Vector3(0, 0, 100));
        aimLaserRenderer.enabled = false;

        playerCollider = GetComponentInChildren<CapsuleCollider>();
        Debug.Assert(playerCollider != null, "Player collider not found");
        gameInput.OnAttack = (context) =>
        {
            Attack();
        };
        gameInput.OnFire = (context) =>
        {
            Fire();
        };
        gameInput.OnAim = (context) =>
        {
            Aim();
        };
        gameInput.OnBlock = (context) =>
        {
            Block();
        };
        gameInput.OnDash = (context) =>
        {
            Dash();
        };
        movementCooldownTimer = new Timer(this)
        {
            OnTimerElapsed = () =>
            {
                canMove = true;
            }
        };
        turningCooldownTimer = new Timer(this)
        {
            OnTimerElapsed = () =>
            {
                canTurn = true;
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
        dashCooldownTimer = new Timer(this)
        { 
            OnTimerElapsed = () => 
            { 
                canDash = true; 
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
    }

    private void HandleMovement()
    {
        // Get input for movement
        Vector2 movementVector = gameInput.GetMovementVectorNormalized();
        Vector3 moveDir = new Vector3(movementVector.x, 0, movementVector.y);
        Vector3 rotateDir;
        moveDir = camera_direction.forward * moveDir.z + camera_direction.right * moveDir.x;
        moveDir.y = 0;
        rotateDir = moveDir;

        // Check if movement is disabled
        if (!canMove)
        {
            moveDir = Vector3.zero;
        }

        // Check if rotation is disabled
        if (!canTurn)
        {
            rotateDir = Vector3.zero;
        }

        // Handle ground detection
        isGrounded = Physics.Raycast(transform.position, Vector3.down, playerCollider.height / 2 + 0.2f, groundLayer);

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
        isWalking = moveDir != Vector3.zero;

        // Smoothly rotate player towards movement direction
        float rotationSpeed = 20f;
        transform.forward = Vector3.Slerp(transform.forward, rotateDir, Time.deltaTime * rotationSpeed);
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

        //Debug.Log($"Combo {attackComboCounter}");

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

    private void Aim()
    {
        if (!canFire || !canAct)
            return;
        aimLaserRenderer.enabled = true;
        firingStage = FiringStage.aiming;
    }

    private void Fire()
    {
        if (firingStage != FiringStage.aiming)
            return;
        aimLaserRenderer.enabled = false;
        firingStage = FiringStage.startCharging;
    }

    private void Dash()
    {
        if(!canDash || !canMove)
            return;

        var dashSpeed = 50f;
        // Get input for movement
        Vector2 movementVector = gameInput.GetMovementVectorNormalized();
        Vector3 moveDir = new Vector3(movementVector.x, 0, movementVector.y);
        moveDir = camera_direction.forward * moveDir.z * dashSpeed + camera_direction.right * moveDir.x * dashSpeed;
        moveDir.y = 0;

        // Handle ground detection
        isGrounded = Physics.Raycast(transform.position, Vector3.down, playerCollider.height / 2 + 0.2f, groundLayer);

        // Handle movement
        Vector3 targetVelocity = moveDir * movementSpeed;
        Vector3 velocity = rb.velocity;
        Vector3 velocityChange = targetVelocity - velocity;

        // Clamp velocity change to prevent abrupt changes
        velocityChange.y = 0;

        rb.AddForce(velocityChange, ForceMode.VelocityChange);

        // Apply drag if grounded
        rb.drag = isGrounded ? groundDrag : 0;

        canDash = false;
        dashCooldownTimer.Start(1.2f);
    }

    private void FiringSequence()
    {
        Vector3 playerFacing = transform.forward;
        switch (firingStage)
        {
            //stop movement while aiming projectile
            case FiringStage.aiming:
                canMove = false;
                break;
            //hold still while charging projectile
            case FiringStage.startCharging:
                canMove = false;
                movementCooldownTimer.Start(0.3f);
                turningCooldownTimer.Start(0.3f);
                canAct = false;
                actionCooldownTimer.Start(1.0f);
                firingStage = FiringStage.charging;
                firingStageCooldown.Start(0.3f);
                break;
            // charging
            case FiringStage.charging:
                break;
            //fire projectile
            case FiringStage.firing:
                var projectileSpawnDistance = 5f;
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

                canTurn = false;
                turningCooldownTimer.Start(0.4f);

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

    public bool IsWalking()
    {
        return isWalking;
    }
}
