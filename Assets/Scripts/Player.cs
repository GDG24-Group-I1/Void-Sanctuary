using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Windows;

public class Player : MonoBehaviour
{
    enum FiringStage { notFiring, aiming, startCharging, charging, firing, knockback }

    PlayerAnimator playerAnimator;

    private const float firingKnockbackSpeed = 50f;
    private const int maxWallsCollided = 10;
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float runSpeed = 10f;
    [SerializeField] private float groundDrag = 6f;
    [SerializeField] private GameInput gameInput;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private GameObject swordPrefab;
    [SerializeField] private Transform camera_direction;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private LayerMask wallLayer;

    private GameObject WeaponOnBack;
    private GameObject WeaponInHand;
    private LineRenderer aimLaserRenderer;

    private Rigidbody rb;
    private LayerMask groundLayer;
    private bool isGrounded;
    private CapsuleCollider playerCollider;
    private Collider[] previousWallsCollided = Array.Empty<Collider>();
    private RaycastHit[] wallsCollided = new RaycastHit[maxWallsCollided];

    public bool IsWalking { get; private set; }

    public bool IsRunning { get; private set; }

    public bool IsWeaponEquipped { get; private set; }

    public bool IsAttacking { get; private set; }

    private float movementSpeed;
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
    private Timer deathTimer;
    private Timer dashCooldownTimer;
    private int attackComboCounter = 1;
    private FiringStage firingStage = FiringStage.notFiring;
    private Vector3 startingPosition;


    private void Start()
    {
        movementSpeed = walkSpeed;
        startingPosition = transform.position;
        rb = GetComponent<Rigidbody>();

        WeaponOnBack = GameObject.Find("WeaponHolderOnBack");
        WeaponInHand = GameObject.Find("WeaponHolderOnHand");
        WeaponInHand.SetActive(false);
        

        deathTimer = new Timer(this)
        {
            OnTimerElapsed = () =>
            {
                // Reset player position, for now
                // Later, add death logic here
                Debug.Log("Player died");
                transform.position = startingPosition;
                rb.velocity = Vector3.zero;
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
            deathTimer?.Start(5.0f);
            isGrounded = false;
        };
        floorCollider.CollisionStayCallback = () =>
        {
            deathTimer?.Stop();
            isGrounded = true;
        };
        rb.freezeRotation = true;

        //setting up the aim laser
        aimLaserRenderer = GetComponentInChildren<LineRenderer>();
        aimLaserRenderer.positionCount = 2;
        aimLaserRenderer.SetPosition(0, new Vector3(0, 0, 0));
        aimLaserRenderer.enabled = false;

        playerCollider = GetComponentInChildren<CapsuleCollider>();
        Debug.Assert(playerCollider != null, "Player collider not found");
        gameInput.OnAttack = (context) =>
        {
            
            if (IsWeaponEquipped)
            {
                IsAttacking = true;
                canMove = false;
                if (IsAttacking)
                {
                    Attack();
                }
                attackCooldownTimer.Start(1.5f);
            }
            else
            {
                IsAttacking = false;
            }
                
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
        gameInput.OnRun = (context) =>
        {

            //Debug.Log($"IsWalking {IsWalking}, IsRunning {IsRunning} context {context.performed}");

            if (IsWalking)
            {
                IsRunning = !IsRunning;
                if (IsRunning)
                {
                    movementSpeed = runSpeed;
                }
                else
                {
                    movementSpeed = walkSpeed;
                }
            }

        };
        gameInput.OnDrawWeapon = (context) =>
        {
            IsWeaponEquipped = !IsWeaponEquipped;

            canMove = false;
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
                IsAttacking = false;
                canAttack = true;
                canMove = true;
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
        CheckIfPlayerIsHidden();
    }

    private static void SetRendererOpacity(GameObject obj, float alpha)
    {
        var renderer = obj.GetComponent<Renderer>();
        if (renderer == null)
            return;
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
                SetRendererOpacity(hit.collider.gameObject, 0.0f);
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

        if (IsWalking == false)
        {
            movementSpeed = walkSpeed;
            IsRunning = false;
        }

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
            //attackCooldownTimer.Start(0.4f);
            attackComboTimer.Start(1.0f);
        }
        else
        {
            attackComboCounter = 1;
            canAttack = false;
            //attackCooldownTimer.Start(0.8f);
            attackComboTimer.Start(0.0f);
        }

        

        canAct = false;
        actionCooldownTimer.Start(0.4f);

        canMove = false;
        // movementCooldownTimer.Start(0.5f);
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
                aimLaserRenderer.SetPosition(0, new Vector3(1, 0, 0));
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

    public void SwordAnimationEnded()
    {
        canMove = true;
    }

    public void SwordDrawn()
    {
        WeaponOnBack.SetActive(!IsWeaponEquipped);
        WeaponInHand.SetActive(IsWeaponEquipped);
    }
}
