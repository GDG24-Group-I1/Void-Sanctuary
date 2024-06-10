using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public enum ComboState
{
    CanCombo,
    PressedEarly,
    NotPressed,
    Pressed
}

public enum AnimationState
{
    None,
    Transition,
    Playing
}

[RequireComponent(typeof(Rigidbody), typeof(GameInput), typeof(FloorCollider))]
public class Player : MonoBehaviour
{
    enum FiringStage { notFiring, aiming, startCharging, charging, firing, knockback }

    private const float firingKnockbackSpeed = 50f;
    private const int maxWallsCollided = 10;
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float runSpeed = 10f;
    [SerializeField] private float groundDrag = 6f;
    [SerializeField] public GameObject projectilePrefab;
    [SerializeField] public Transform cameraDirection;
    [SerializeField] public Transform cameraTransform;
    [SerializeField] public LayerMask wallLayer;
    [SerializeField] private int thresholdFrameGrounded = 1;

    private GameObject WeaponOnBack;
    private GameObject WeaponInHand;
    private LineRenderer aimLaserRenderer;
    private BoxCollider swordCollider;
    private GameInput gameInput;

    private Rigidbody rb;
    private int frameNotGrounded;
    private bool isGrounded;
    private Collider[] previousWallsCollided = Array.Empty<Collider>();
    private RaycastHit[] wallsCollided = new RaycastHit[maxWallsCollided];

    public bool IsWalking { get; private set; }

    public bool IsRunning { get; private set; }

    public bool IsWeaponEquipped { get; private set; }

    public bool IsAttacking { get; private set; }

    public AnimationState IsDashing { get; private set; } = AnimationState.None;

    public AnimationState IsFalling { get; private set; } = AnimationState.None;

    public ComboState CanCombo { get; set; } = ComboState.NotPressed;

    public int AttackNumber { get; set; } = 0;

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
    private Timer firingStageCooldown;
    private Timer deathTimer;
    private Timer dashCooldownTimer;
    private Timer dashTimer;
    private FiringStage firingStage = FiringStage.notFiring;
    private Vector3 startingPosition;
    private Vector3 dashDirection;


    private void ResetPlayer()
    {
        Destroy(gameObject);
    }


    private void Start()
    {
        gameInput = GetComponent<GameInput>();
        movementSpeed = walkSpeed;
        startingPosition = transform.position;
        rb = GetComponent<Rigidbody>();
        swordCollider = GetComponentInChildren<BoxCollider>();

        WeaponOnBack = GameObject.Find("WeaponHolderOnBack");
        WeaponInHand = GameObject.Find("WeaponHolderOnHand");
        WeaponInHand.SetActive(false);


        deathTimer = new Timer(this)
        {
            OnTimerElapsed = () =>
            {
                // Reset player position, for now
                // Later, add death logic here
                if (IsFalling != AnimationState.None)
                {
                    Debug.Log("Player died");


                    ResetPlayer();
                    return null;
                }
                else
                {
                    IsFalling = AnimationState.Transition;
                    return 4.0f;
                }
            }
        };
        var floorCollider = GetComponent<FloorCollider>();
        floorCollider.CollisionEnterCallback = () =>
        {
            deathTimer?.Stop();
            IsFalling = AnimationState.None;
            frameNotGrounded = 0;
            isGrounded = true;
        };
        floorCollider.CollisionExitCallback = () =>
        {
            deathTimer?.Start(1.0f);
            isGrounded = false;
        };
        floorCollider.CollisionStayCallback = () =>
        {
            deathTimer?.Stop();
            IsFalling = AnimationState.None;
            frameNotGrounded = 0;
            isGrounded = true;
        };
        rb.freezeRotation = true;

        //setting up the aim laser
        aimLaserRenderer = GetComponentInChildren<LineRenderer>();
        aimLaserRenderer.positionCount = 2;
        aimLaserRenderer.SetPosition(0, new Vector3(0, .1f, 0));
        aimLaserRenderer.enabled = false;

        gameInput.OnAttack = (context) =>
        {

            if (IsWeaponEquipped)
            {
                OnPlayerAttack?.Invoke();
                IsAttacking = true;
                canMove = false;
                Attack();
                swordCollider.enabled = true;
                // attackCooldownTimer.Start(1.5f);
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
                return null;
            }
        };
        turningCooldownTimer = new Timer(this)
        {
            OnTimerElapsed = () =>
            {
                canTurn = true;
                return null;
            }
        };
        actionCooldownTimer = new Timer(this)
        {
            OnTimerElapsed = () =>
            {
                canAct = true;
                return null;
            }
        };
        fireCooldownTimer = new Timer(this)
        {
            OnTimerElapsed = () =>
            {
                canFire = true;
                return null;
            }
        };
        attackCooldownTimer = new Timer(this)
        {
            OnTimerElapsed = () =>
            {
                IsAttacking = false;
                canAttack = true;
                canMove = true;
                return null;
            }
        };
        firingStageCooldown = new Timer(this)
        {
            OnTimerElapsed = () =>
            {
                firingStage += 1;
                if (firingStage > FiringStage.knockback)
                    firingStage = FiringStage.notFiring;
                return null;
            }
        };
        dashCooldownTimer = new Timer(this)
        {
            OnTimerElapsed = () =>
            {
                canDash = true;
                return null;
            }
        };
        dashTimer = new Timer(this)
        {
            OnTimerElapsed = () =>
            {
                IsDashing = AnimationState.None;
                return null;
            }
        };
    }

    private void FixedUpdate()
    {
        if (!isGrounded)
        {
            frameNotGrounded++;
        }
        HandleMovement();
        Dashing();
    }

    private void Update()
    {
        FiringSequence();
        CheckIfPlayerIsHidden();
    }

    private static void SetRendererOpacity(GameObject obj, float alpha)
    {
        if (!obj.TryGetComponent<Renderer>(out var renderer))
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
        previousWallsCollided = wallsCollided.Take(hits).Select(x => x.collider).Where(x => x != null).ToArray();
    }

    private void CheckIfPlayerIsHidden()
    {
        Vector3 cameraPosition = cameraTransform.position;
        var playerCentrum = new Vector3(transform.position.x, 1, transform.position.z);
        Vector3 directionToPlayer = playerCentrum - cameraPosition;
        float distanceToPlayer = Vector3.Distance(cameraPosition, playerCentrum);

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
        moveDir = cameraDirection.forward * moveDir.z + cameraDirection.right * moveDir.x;
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

        if (frameNotGrounded > thresholdFrameGrounded)
        {
            velocityChange.y = -9.8f;
        }

        rb.AddForce(velocityChange, ForceMode.VelocityChange);

        // Apply drag if grounded
        rb.drag = frameNotGrounded == 0 ? groundDrag : 0.1f;

        // Check if the player is walking
        IsWalking = moveDir != Vector3.zero;

        if (IsWalking == false)
        {
            movementSpeed = walkSpeed;
            IsRunning = false;
        }

        // Smoothly rotate player towards movement direction
        if (canMove || firingStage == FiringStage.aiming)
        {
            float rotationSpeed = 20f;
            transform.forward = Vector3.Slerp(transform.forward, rotateDir, Time.deltaTime * rotationSpeed);
        }
    }

    private void Attack()
    {
        if (!canAttack || !canAct)
            return;

        canAct = false;
        actionCooldownTimer.Start(0.4f);
        canMove = false;
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
        if (!canDash || !canMove)
            return;

        //temporary, I'll cleanup when it works
        var dashCooldown = 3f;
        var dashDuration = 0.2f;
        var dashSpeed = 30f;

        // Get input for movement
        dashDirection = transform.forward * dashSpeed;

        canDash = false;
        dashCooldownTimer.Start(dashCooldown);
        IsDashing = AnimationState.Transition;
        dashTimer.Start(dashDuration);
        canMove = false;
        movementCooldownTimer.Start(dashDuration);

        /*
        var dashSpeed = 25f;
        // Get input for movement
        Vector2 movementVector = gameInput.GetMovementVectorNormalized();
        Vector3 moveDir = new Vector3(movementVector.x, 0, movementVector.y);
        moveDir = dashSpeed * moveDir.z * cameraDirection.forward + dashSpeed * moveDir.x * cameraDirection.right;
        moveDir.y = 0;

        // Handle movement
        Vector3 targetVelocity = moveDir * movementSpeed;
        Vector3 velocity = rb.velocity;
        Vector3 velocityChange = targetVelocity - velocity;

        // Clamp velocity change to prevent abrupt changes
        velocityChange.y = 0;

        rb.AddForce(velocityChange, ForceMode.VelocityChange);

        // Apply drag if grounded
        rb.drag = frameNotGrounded == 0 ? groundDrag : 0;

        canDash = false;
        IsDashing = true;
        dashCooldownTimer.Start(1.2f);
        */
    }

    private void FiringSequence()
    {
        Vector3 playerFacing = transform.forward;
        switch (firingStage)
        {
            //stop movement while aiming projectile
            case FiringStage.aiming:
                aimLaserRenderer.SetPosition(1, new Vector3(0, .1f, 100));
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
                var projectileSpawnDistance = 2f;
                var startingPosition = aimLaserRenderer.GetPosition(0);
                var endingPosition = aimLaserRenderer.GetPosition(1);
                Vector3 projectilePosition = aimLaserRenderer.transform.TransformPoint(startingPosition) + (transform.forward * projectileSpawnDistance);

                GameObject projectile = Instantiate(projectilePrefab, projectilePosition, transform.rotation);
                var projectileScript = projectile.GetComponent<ProjectileScript>();
                projectileScript.endingPosition = aimLaserRenderer.transform.TransformPoint(endingPosition);

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

    public void AttackAnimationEnded()
    {
        if (CanCombo == ComboState.Pressed)
        {
            IsAttacking = true;
        }
        else
        {
            AttackNumber = 0;
            IsAttacking = false;
            swordCollider.enabled = false;
            canAttack = true;
            canMove = true;
        }
        CanCombo = ComboState.NotPressed;

    }

    public void StartFalling()
    {
        IsFalling = AnimationState.Playing;
    }

    public void StartDashing()
    {
        IsDashing = AnimationState.Playing;
    }

    public Action OnPlayerAttack;

    private void Dashing()
    {
        if (IsDashing != AnimationState.None)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, 1f))
                return;

            // Handle movement
            Vector3 targetVelocity = dashDirection * movementSpeed;
            Vector3 velocity = rb.velocity;
            Vector3 velocityChange = targetVelocity - velocity;

            // Clamp velocity change to prevent abrupt changes
            velocityChange.y = 0;

            rb.AddForce(velocityChange, ForceMode.VelocityChange);

            // Apply drag if grounded
            rb.drag = frameNotGrounded == 0 ? groundDrag : 0;
        }
    }
}
