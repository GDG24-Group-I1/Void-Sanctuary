using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

public class Player : MonoBehaviour
{
    enum FiringStage { notFiring, startCharging, charging, firing, knockback }

    [SerializeField] private float movementSpeed = 20f;
    [SerializeField] private float groundDrag = 6f;
    [SerializeField] private GameInput gameInput;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private GameObject swordPrefab;

    private Rigidbody rb;
    private LayerMask groundLayer;
    private bool isGrounded;
    private CapsuleCollider playerCollider;
    private bool isWalking;

    private bool canMove = true;
    private bool canAct = true;
    private bool canFire = true;
    private bool canAttack = true;
    private float movementCooldownTimer = 0;
    private float actionCooldownTimer = 0;
    private float fireCooldownTimer = 0;
    private float attackCooldownTimer = 0;
    private int attackComboCounter = 1;
    private float attackComboTimer = 0;
    private FiringStage firingStage = FiringStage.notFiring;
    private float firingStageCooldown = 0;
    private float firingKnockbackSpeed = 50f;



    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        playerCollider = GetComponentInChildren<CapsuleCollider>();
        if (playerCollider == null)
        {
            Debug.LogError("Player collider not found");
        }
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
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    private void Update()
    {
        Cooldowns();
        FiringSequence();
    }

    private void HandleMovement()
    {
        // Get input for movement
        Vector2 movementVector = gameInput.GetMovementVectorNormalized();
        Vector3 moveDir = new Vector3(movementVector.x, 0f, movementVector.y).normalized;

        // Check if movement is disabled
        if (!canMove)
        {
            moveDir = Vector3.zero;
            if (movementCooldownTimer > 0)
            {
                movementCooldownTimer -= Time.fixedDeltaTime;
                if (movementCooldownTimer <= 0)
                {
                    movementCooldownTimer = 0;
                    canMove = true;
                }
            }
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
        transform.forward = Vector3.Slerp(transform.forward, moveDir, Time.deltaTime * rotationSpeed);

    }

    private void Cooldowns()
    {
        if (movementCooldownTimer > 0)
        {
            movementCooldownTimer -= Time.deltaTime;
            if (movementCooldownTimer <= 0)
            {
                movementCooldownTimer = 0;
                canMove = true;
            }
        }

        if (actionCooldownTimer > 0)
        {
            actionCooldownTimer -= Time.deltaTime;
            if (actionCooldownTimer <= 0)
            {
                actionCooldownTimer = 0;
                canAct = true;
            }
        }

        if (attackCooldownTimer > 0)
        {
            attackCooldownTimer -= Time.deltaTime;
            if (attackCooldownTimer <= 0)
            {
                attackCooldownTimer = 0;
                canAttack = true;
            }
        }

        if (fireCooldownTimer > 0)
        {
            fireCooldownTimer -= Time.deltaTime;
            if (fireCooldownTimer <= 0)
            {
                fireCooldownTimer = 0;
                canFire = true;
            }
        }

        if (attackComboTimer > 0)
        {
            attackComboTimer -= Time.deltaTime;
            if (attackComboTimer <= 0)
            {
                attackComboTimer = 0;
                attackComboCounter = 1;
            }
        }

        if (firingStageCooldown > 0)
        {
            firingStageCooldown -= Time.deltaTime;
            if (firingStageCooldown <= 0)
            {
                firingStageCooldown = 0;
                firingStage += 1;
                if (firingStage > FiringStage.knockback)
                    firingStage = FiringStage.notFiring;
            }
        }
    }

    private void Attack()
    {
        if (!canAttack || !canAct)
            return;

        // spawn sword at right offset and angle from player
        float swordRange = 5f;

        Vector3 playerFacing = transform.forward;

        Quaternion swordRotation = Quaternion.Euler(90f, transform.rotation.eulerAngles.y, 180f);
        Vector3 swordPosition = new Vector3(transform.position.x + swordRange * playerFacing.x, transform.position.y + 1.5f, transform.position.z + swordRange * playerFacing.z); ;

        GameObject sword = Instantiate(swordPrefab, swordPosition, swordRotation);

        var swordScript = sword.GetComponent<SwordScript>();
        swordScript.pivot = transform.position;
        swordScript.combo = attackComboCounter;

        Debug.Log($"Combo {attackComboCounter}");

        if (attackComboCounter < 3)
        {
            attackComboCounter += 1;
            canAttack = false;
            attackCooldownTimer = 0.4f;
            attackComboTimer = 1.0f;
        }
        else
        {
            attackComboCounter = 1;
            canAttack = false;
            attackCooldownTimer = 0.8f;
            attackComboTimer = 0.0f;
        }

        canAct = false;
        actionCooldownTimer = 0.4f;

        canMove = false;
        movementCooldownTimer = 0.5f;
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
                movementCooldownTimer = 1.8f;
                canAct = false;
                actionCooldownTimer = 1.8f;
                firingStage = FiringStage.charging;
                firingStageCooldown = 1.0f;
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
                fireCooldownTimer = 3.0f;

                canAct = false;
                actionCooldownTimer = 1.0f;

                canMove = false;
                movementCooldownTimer = 0.4f;

                firingStageCooldown = 0.1f;
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
