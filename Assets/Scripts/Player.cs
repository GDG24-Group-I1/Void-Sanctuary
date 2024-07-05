using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;
using UnityEngine.UI;

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

public enum FiringStage
{
    notFiring,
    aiming,
    startCharging,
    charging,
    firing,
    knockback
}

[RequireComponent(typeof(Rigidbody), typeof(FloorCollider))]
public class Player : MonoBehaviour, VoidSanctuaryActions.IPlayerActions
{
    private const float firingKnockbackSpeed = 0f;
    private const int maxWallsCollided = 10;
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float runSpeed = 10f;
    [SerializeField] private float groundDrag = 6f;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private GameObject trailPrefab;
    [SerializeField] private LayerMask wallLayerMask;
    [SerializeField] private LayerMask groundLayerMask;
    [SerializeField] private int thresholdFrameGrounded = 1;
    [SerializeField] private float dashDistance = 5f;
    [SerializeField] private float stopDistance = 1f;
    [SerializeField] private Material glowMaterial;
    [SerializeField] private Material swordBaseMaterial;
    [SerializeField] private Material swordBackBaseMaterial;
    [SerializeField] private float slowDownFactor = 0.25f;

    // these need to be public because they are set by the respawner script since they can't be set in the prefab
    public Transform cameraTransform;
    public GameObject healthBar;
    public GameObject loaderBorder;

    private bool IsSwordGlowing = false;
    private GameObject WeaponOnBack;
    private GameObject WeaponInHand;
    private LineRenderer aimLaserRenderer;
    private GameObject sword;
    private GameObject swordBack;
    private GameObject dialogBox;
    private BoxCollider swordCollider;
    private GameInput gameInput;
    private Slider healthSlider;

    private Rigidbody rb;
    private int frameNotGrounded;
    private bool isGrounded;
    private Collider[] previousWallsCollided = Array.Empty<Collider>();
    private readonly RaycastHit[] wallsCollided = new RaycastHit[maxWallsCollided];

    private float fixedDeltaTime;
    public bool IsWalking { get; private set; }

    public bool IsRunning { get; private set; }

    public bool IsWeaponEquipped { get; private set; }

    public bool IsAttacking { get; private set; }

    public bool DashClicked { get; private set; }

    public AnimationState IsFalling { get; private set; } = AnimationState.None;

    public ComboState CanCombo { get; set; } = ComboState.NotPressed;

    public int AttackNumber { get; set; } = 0;

    public FiringStage firingStage { get; private set; } = FiringStage.notFiring;

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
    private Timer firingStageCooldown;
    private Timer deathTimer;
    private Timer dashCooldownTimer;


    private void ResetPlayer()
    {
        Destroy(gameObject);
    }


    private void Awake()
    {
        fixedDeltaTime = Time.fixedDeltaTime;
    }


    private void Start()
    {
        Assert.IsNotNull(cameraTransform, "CAMERA TRANSFORM IS NOT SET IN THE PLAYER OBJECT IN THE SCENE, PUT THE TopDownCamera IN THE CameraTrasform SLOT ON THIS GAMEOBJECT");
        Assert.IsNotNull(healthBar, "HEALTH BAR IS NOT SET IN THE PLAYER OBJECT IN THE SCENE, PUT THE Canvas->HealthBar OBJECT IN THE Health Bar SLOT ON THIS GAME OBJECT");
        Assert.IsNotNull(loaderBorder, "LOADER BORDER IS NOT SET IN PLAYER OBJECT IN THE SCENE, PUT THE Canvas->Loader->LoaderBorder IN THE Loader Border SLOT ON THIS GAME OBJECT");
        gameInput = GameObject.FindWithTag("InputHandler").GetComponent<GameInput>();
        gameInput.RegisterPlayer(this);
        movementSpeed = walkSpeed;
        rb = GetComponent<Rigidbody>();
        healthSlider = healthBar.GetComponent<Slider>();
        healthSlider.value = healthSlider.maxValue;
        healthSlider.onValueChanged.AddListener((value) =>
        {
            if (value == healthSlider.minValue)
            {
                ResetPlayer();
            }
        });

        var swords = GameObject.FindGameObjectsWithTag("Sword");
        Debug.Assert(swords.Length == 1, "There should be exactly one sword in the scene");
        sword = swords[0];
        swordBack = GameObject.FindGameObjectWithTag("SwordInternal");
        swordCollider = sword.GetComponent<BoxCollider>();
        WeaponOnBack = GameObject.Find("WeaponHolderOnBack");
        WeaponInHand = GameObject.Find("WeaponHolderOnHand");
        WeaponInHand.SetActive(false);
        dialogBox = GameObject.Find("DialogBox");


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


    void OnGUI()
    {
        float fps = 1.0f / Time.deltaTime;
        GUI.Label(new Rect(0, 0, 100, 100), "FPS: " + ((int)fps).ToString());
    }

    private void Update()
    {
        DrawDebugRays();
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

        var hits = Physics.RaycastNonAlloc(cameraPosition, directionToPlayer, wallsCollided, distanceToPlayer, wallLayerMask);
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
        Vector3 moveDir = new(movementVector.x, 0, movementVector.y);
        Vector3 rotateDir;
        moveDir = cameraTransform.forward * moveDir.z + cameraTransform.right * moveDir.x;
        moveDir.Normalize();
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

        if (frameNotGrounded == thresholdFrameGrounded)
        {
            velocityChange.y = -9.8f;
        }

        rb.AddForce(velocityChange, ForceMode.VelocityChange);

        // Apply drag if grounded
        rb.drag = frameNotGrounded < thresholdFrameGrounded ? groundDrag : 0.1f;

        // Check if the player is walking
        IsWalking = moveDir != Vector3.zero;

        if (IsWalking == false)
        {
            movementSpeed = walkSpeed;
            IsRunning = false;
        }

        // Smoothly rotate player towards movement direction
        if (canMove || (!gameInput.IsKeyboardMovement && firingStage == FiringStage.aiming))
        {
            float rotationSpeed = 15f;
            transform.forward = Vector3.Slerp(transform.forward, rotateDir, rotationSpeed * Time.deltaTime);
        }
        else if (firingStage == FiringStage.aiming)
        {
            Vector3 mousePosition = gameInput.GetMousePosition();
            Ray ray = Camera.main.ScreenPointToRay(mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                transform.LookAt(new Vector3(hit.point.x, transform.position.y, hit.point.z));
            }
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
        if (!canFire || !canAct || !IsWeaponEquipped)
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

        //FIX THIS 
        var dashCooldown = 1f;

        canDash = false;
        dashCooldownTimer.Start(dashCooldown);
        DashClicked = true;
    }

    private void DrawDebugRays()
    {
        if (gameInput.DrawDebugRays)
        {
            #region Debug rays for dashing
            Debug.DrawRay(transform.position + Vector3.up, transform.forward * dashDistance, Color.red);
            Debug.DrawRay(transform.position + Vector3.up + transform.forward * dashDistance, Vector3.down * 5, Color.red);
            #endregion
        }
    }

    private void FiringSequence()
    {
        Vector3 playerFacing = transform.forward;
        switch (firingStage)
        {
            //stop movement while aiming projectile
            case FiringStage.aiming:
                aimLaserRenderer.SetPosition(1, new Vector3(0.05f, .5f, -100));
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
                var startingPosition = aimLaserRenderer.GetPosition(0);
                var endingPosition = aimLaserRenderer.GetPosition(1);
                Vector3 projectilePosition = aimLaserRenderer.transform.TransformPoint(startingPosition) + transform.forward;
                var rotation = transform.rotation;
                GameObject projectile = Instantiate(projectilePrefab, projectilePosition, rotation * Quaternion.Euler(90, 0, 0));
                var projectileScript = projectile.GetComponent<ProjectileScript>();
                projectileScript.endingPosition = aimLaserRenderer.transform.TransformPoint(endingPosition);

                loaderBorder.GetComponent<CircularProgressBar>().StartProgressBar(3.0f);

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
                rb.AddForce(75 * knockback * -playerFacing, ForceMode.VelocityChange);
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
        if (IsWeaponEquipped)
        {
            FindAndSetupLaser();
        }
    }

    public void FindAndSetupLaser()
    {
        //setting up the aim laser
        aimLaserRenderer = GetComponentInChildren<LineRenderer>();
        aimLaserRenderer.positionCount = 2;
        aimLaserRenderer.SetPosition(0, new Vector3(0.05f, 0.5f, -0.3f));
        aimLaserRenderer.enabled = false;
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

    public void StopSwordGlowing()
    {
        if (IsSwordGlowing)
        {
            DebugExt.LogCombo($"Can't combo anymore at time {Time.time} for {AttackNumber}");
            sword.GetComponent<SkinnedMeshRenderer>().SwitchMaterial(glowMaterial, swordBaseMaterial);
            swordBack.GetComponent<SkinnedMeshRenderer>().SwitchMaterial(glowMaterial, swordBackBaseMaterial);
            IsSwordGlowing = false;
            if (gameInput.SlowDownAttack)
            {
                Time.timeScale = 1.0f;
                Time.fixedDeltaTime = fixedDeltaTime;
            }
        }
    }

    public void StartFalling()
    {
        IsFalling = AnimationState.Playing;
    }

    public Action OnPlayerAttack;

    private void Dashing()
    {
        if (DashClicked)
        {
            DashClicked = false;
            var notPlayerLayer = ~LayerMask.GetMask("playerLayer");
            var hasHit = Physics.Raycast(transform.position + Vector3.up, transform.forward, out RaycastHit hit, dashDistance, notPlayerLayer);
            var groundLayer = LayerMask.NameToLayer("groundLayer");
            Vector3 newPosition;
            if (hasHit)
            {
                if (hit.collider.gameObject.layer == groundLayer)
                {
                    // if we hit the ground, move to the hit point
                    newPosition = hit.point;
                }
                else
                {
                    // if we hit something else (e.g. wall or enemy), adjust the position so we don't go through it
                    var realDashDistance = hit.distance - stopDistance;
                    newPosition = transform.position + transform.forward * realDashDistance;
                }
            }
            else
            {
                var destination = transform.position + (Vector3.up * 2) + transform.forward * dashDistance;
                // we haven't hit anything, check if the ground is the same Y level as the player
                var groundHit = Physics.Raycast(destination, Vector3.down, out RaycastHit groundHeightHit, Mathf.Infinity, groundLayerMask);
                if (groundHit)
                {
                    // if we hit the ground, move to the hit point
                    newPosition = groundHeightHit.point;
                }
                else
                {
                    // if we didn't hit the ground, move to the destinationawwww
                    newPosition = transform.position + transform.forward * dashDistance;
                }
            }

            rb.isKinematic = true;
            var Xoffsets = new float[] { -.5f, .5f };
            for (int i = 1; i < 4; i++)
            {
                var startingPosition = transform.position + (0.5f * i * Vector3.up);
                var endingPosition = newPosition + (0.5f * i * Vector3.up);
                for (int j = 0; j < Xoffsets.Length; j++)
                {
                    var trail = Instantiate(trailPrefab, startingPosition + Xoffsets[j] * Vector3.right, transform.rotation);
                    var trailScript = trail.GetComponent<TrailScript>();
                    trailScript.EndingPosition = endingPosition + Xoffsets[j] * Vector3.right;
                    trailScript.PlayerTransform = transform;
                }
            }
            transform.position = newPosition;
        }
        else
        {
            rb.isKinematic = false;
        }
    }

    public void SetCanCombo()
    {
        CanCombo = ComboState.CanCombo;
        sword.GetComponent<SkinnedMeshRenderer>().SwitchMaterial(swordBaseMaterial, glowMaterial);
        swordBack.GetComponent<SkinnedMeshRenderer>().SwitchMaterial(swordBackBaseMaterial, glowMaterial);
        if (gameInput.SlowDownAttack)
        {
            Time.timeScale = slowDownFactor;
            Time.fixedDeltaTime = fixedDeltaTime * Time.timeScale;
        }
        IsSwordGlowing = true;
        DebugExt.LogCombo($"Can combo at time {Time.time} for {AttackNumber}");
    }

    public void OnDestroy()
    {
        StopAllCoroutines();
        gameInput.UnregisterPlayer(this);
    }

    #region Input actions

    public void OnMove(InputAction.CallbackContext context) { }

    public void OnMousePosition(InputAction.CallbackContext context) { }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (IsWeaponEquipped)
            {
                OnPlayerAttack?.Invoke();
                IsAttacking = true;
                canMove = false;
                Attack();
                swordCollider.enabled = true;
            }
            else
            {
                IsAttacking = false;
            }
        }
    }
    public void OnFire(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Aim();
        }
        else if (context.canceled)
        {
            Fire();
        }
    }
    public void OnBlock(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Block();
        }
    }
    public void OnRun(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (gameInput.HoldDownToRun)
            {
                IsRunning = true;
                movementSpeed = runSpeed;
            }
            else
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
            }
        }
        else if (context.canceled)
        {
            if (gameInput.HoldDownToRun)
            {
                IsRunning = false;
                movementSpeed = walkSpeed;
            }
        }
    }
    public void OnDrawWeapon(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            IsWeaponEquipped = !IsWeaponEquipped;
            canMove = false;
        }
    }
    public void OnDash(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Dash();
        }
    }
    public void OnFakeHit(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (healthSlider.value == healthSlider.minValue)
            {
                healthSlider.value = healthSlider.maxValue;
            }
            else
            {
                healthSlider.value--;
            }
        }
    }

    public void OnDismissDialog(InputAction.CallbackContext context)
    {
        var handler = dialogBox.GetComponent<DialogHandler>();
        if (!gameInput.IsKeyboardMovement && handler.IsInDialog && handler.IsDialogDismissable)
        {
            handler.DismissDialog();
        }
    }

    #endregion
    static int dialogCounter = 0;
    public void OnTestDialog(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            var handler = dialogBox.GetComponent<DialogHandler>();
            if (dialogCounter == 0)
            {
                handler.SetDialog("This is a test dialog", 1.0f);
            }
            else if (dialogCounter == 1)
            {
                handler.SetDialog("This is a bit of a longer dialog\nEven with a newline!!!", 4.0f);
            }
            else
            {
                handler.SetDialog("I will be repeated forever each time F4 is pressed", 1.5f);
            }
            dialogCounter++;
        }
    }
}
