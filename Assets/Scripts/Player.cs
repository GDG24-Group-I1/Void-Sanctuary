using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.PostProcessing;
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

enum CurrentLoopingSound
{
    None,
    Walk,
    Run,
    MagneticRay
}

[RequireComponent(typeof(Rigidbody), typeof(FloorCollider), typeof(AudioSource))]
public class Player : MonoBehaviour, VoidSanctuaryActions.IPlayerActions, IDataPersistence
{
    private const float firingKnockbackSpeed = 0f;
    private const int maxWallsCollided = 10;
    private const int maxContactPoints = 10;

    private const string GunSpriteName = "GunTransparent";
    private const string IceSpriteName = "IceTransparent";
    private const string MagnetSpriteName = "MagnetTransparent";

    [Header("Fixed values set in the prefab\nDo not need to be reset by the Respawner")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float runSpeed = 10f;
    [SerializeField] private float groundDrag = 6f;
    [SerializeField] private GameObject damageProjectilePrefab;
    [SerializeField] private GameObject iceProjectilePrefab;
    [SerializeField] private GameObject trailPrefab;
    [SerializeField] private LayerMask wallLayerMask;
    [SerializeField] private LayerMask groundLayerMask;
    [SerializeField] private int thresholdFrameGrounded = 1;
    [SerializeField] private float dashDistance = 5f;
    [SerializeField] private float stopDistance = 1f;
    [SerializeField] private Material glowMaterial;
    [SerializeField] private Material swordBaseMaterial;
    [SerializeField] private Material swordBackBaseMaterial;
    [SerializeField] private List<Sprite> weaponSprites;
    [SerializeField] private Material[] weaponMaterials;
    [SerializeField] private GameObject dashIndicatorPrefab;

    [Header("Sounds")]
    [SerializeField] private AudioClip ouchSound;
    [SerializeField] private AudioClip deathSound;
    [SerializeField] private AudioClip dashSound;
    [SerializeField] private AudioClip attackSound;
    [SerializeField] private AudioClip shootSound;
    [SerializeField] private AudioClip pickupSound;
    [SerializeField] private AudioClip switchPowerup;
    [SerializeField] private AudioClip magnetRay;
    [SerializeField] private AudioClip walk;
    [SerializeField] private AudioClip run;

    private CurrentLoopingSound currentLoopingSound;

    public Transform CameraTransform { get; set; }
    public GameObject HealthBar { get; set; }
    public GameObject LoaderBorder { get; set; }
    public GameObject DashLoaderBorder { get; set; }
    public Image UiWeaponImage { get; set; }
    public GameObject YouDiedText { get; set; }

    private GameObject[] movableObjects;
    private GameObject[] visibleMovableObjects;
    private int currentMovableObject;
    private Rigidbody currentMovableObjectRigidBody;
    private CubeGravity currentMovableObjectGravity;

    private bool IsSwordGlowing = false;
    private LineRenderer aimLaserRenderer;
    private GameObject sword;
    private GameObject swordBack;
    private GameObject swordPartWithLine;
    private GameObject dialogBox;
    private BoxCollider swordCollider;
    private GameInput gameInput;
    private Slider healthSlider;
    private PostProcessEffectSettings outlineEffect;
    private Animator youDiedTextAnimator;

    private Rigidbody rb;
    private AudioSource audioSource;
    private int frameNotGrounded;
    private bool isGrounded = true;
    private bool hasGroundUnderneath = true;
    private Collider[] previousWallsCollided = Array.Empty<Collider>();
    private readonly RaycastHit[] wallsCollided = new RaycastHit[maxWallsCollided];
    private ContactPoint[] contactPoints = new ContactPoint[maxContactPoints];
    private float lowestGroundContactPoint = 0;

    private int weaponIndex = 0;

    public bool IsWalking { get; private set; }

    public bool IsDashing { get; private set; }

    public bool IsRunning { get; private set; }

    public bool IsAttacking { get; private set; }

    public AnimationState IsFalling { get; private set; } = AnimationState.None;

    public ComboState CanCombo { get; set; } = ComboState.NotPressed;

    public int AttackNumber { get; set; } = 0;

    public FiringStage firingStage { get; private set; } = FiringStage.notFiring;

    private PowerUpHolder _touchedPowerup;
    public PowerUpHolder TouchedPowerup
    {
        get => _touchedPowerup; set
        {
            if (isPickingUpItem) return; // if the player is already picking up an item, don't change it.
            _touchedPowerup = value;
        }
    }

    private float movementSpeed;
    private bool canMove = true;
    private bool canTurn = true;
    private bool canAct = true;
    private bool canFire = true;
    private bool canAttack = true;
    private bool canDash = true;
    private bool executeDash = false;
    private bool isPickingUpItem = false;
    private bool isStaggered = false;
    private bool isDead = false;
    private bool finalLeverActivated = false;
    private Timer movementCooldownTimer;
    private Timer turningCooldownTimer;
    private Timer actionCooldownTimer;
    private Timer fireCooldownTimer;
    private Timer firingStageCooldown;
    private Timer dashCooldownTimer;
    private Timer staggerTimer;
    private PlayerAnimator animator;
    private Renderer[] renderers;
    private Vector3 calculatedDashPosition;
    private GameObject dashIndicator;

    private GameData gameData;
    public void LoadData(GameData data)
    {
        gameData = data;
    }

    private void ResetPlayer()
    {
        Debug.Log("Player died");
        audioSource.PlayOneShot(deathSound);
        isDead = true;
        rb.isKinematic = true;
        canMove = false;
        canTurn = false;
        canAct = false;
        canFire = false;
        canAttack = false;
        canDash = false;
        finalLeverActivated = false;
        Animator animator = GetComponentInChildren<Animator>();
        animator.SetTrigger("Death");
        YouDiedText.SetActive(true);
        youDiedTextAnimator.SetTrigger("PlayerDied");
        Destroy(gameObject, 3.0f);
    }

    private void Start()
    {
        finalLeverActivated = false;
        DataPersistenceManager.GetInstance().RegisterDataPersistenceObject(this);
        youDiedTextAnimator = YouDiedText.GetComponent<Animator>();
        movableObjects = GameObject.FindGameObjectsWithTag("MovableObject");
        visibleMovableObjects = Array.Empty<GameObject>();
        ResetMovableObject();
        outlineEffect = Camera.main.GetComponent<PostProcessVolume>().profile.settings.First(x => x.name.StartsWith("OutlineEffect"));
        outlineEffect.enabled.value = false;
        UiWeaponImage.sprite = weaponSprites[weaponIndex];
        gameInput = GameObject.FindWithTag("InputHandler").GetComponent<GameInput>();
        gameInput.RegisterPlayer(this);
        movementSpeed = walkSpeed;
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        healthSlider = HealthBar.GetComponent<Slider>();
        healthSlider.value = healthSlider.maxValue;
        healthSlider.onValueChanged.AddListener((value) =>
        {
            if (value == healthSlider.minValue)
            {
                ResetPlayer();
            }
        });
        GetComponent<FloorCollider>().CollisionStayCallback = (collision) =>
        {
            if (collision.contactCount > contactPoints.Length)
            {
                contactPoints = new ContactPoint[collision.contactCount];
            }
            var pointCount = collision.GetContacts(contactPoints);
            var yPoint = contactPoints.Take(pointCount).Select(x => transform.InverseTransformPoint(x.point).y).Min();
            lowestGroundContactPoint = yPoint;
        };
        animator = GetComponentInChildren<PlayerAnimator>();
        var swords = GameObject.FindGameObjectsWithTag("Sword");
        Debug.Assert(swords.Length == 1, "There should be exactly one sword in the scene");
        sword = swords[0];
        swordBack = GameObject.FindGameObjectWithTag("SwordInternal");
        swordPartWithLine = sword.transform.parent.Find("Cube").gameObject;
        swordCollider = sword.GetComponent<BoxCollider>();
        dialogBox = GameObject.Find("DialogBox");
        renderers = GetComponentsInChildren<Renderer>().Where(x => x is not LineRenderer).ToArray();
        rb.freezeRotation = true;
        movementCooldownTimer = new Timer(this)
        {
            OnTimerElapsed = () =>
            {
                canMove = !isDead;
                return null;
            }
        };
        turningCooldownTimer = new Timer(this)
        {
            OnTimerElapsed = () =>
            {
                canTurn = !isDead;
                return null;
            }
        };
        actionCooldownTimer = new Timer(this)
        {
            OnTimerElapsed = () =>
            {
                canAct = !isDead;
                return null;
            }
        };
        fireCooldownTimer = new Timer(this)
        {
            OnTimerElapsed = () =>
            {
                canFire = !isDead;
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
                canDash = !isDead;
                return null;
            }
        };
        staggerTimer = new Timer(this)
        {
            OnTimerElapsed = () =>
            {
                isStaggered = false;
                CancelInvoke(nameof(FlashPlayer));
                foreach (var renderer in renderers)
                {
                    renderer.enabled = true;
                }
                return null;
            }
        };
        FindAndSetupLaser();
    }

    private void CheckGround()
    {
        var hit = Physics.Raycast(transform.position.ShiftBy(y: 0.1f), Vector3.down, out RaycastHit hitInfo, float.PositiveInfinity, groundLayerMask);
        hasGroundUnderneath = hit;
        var distance = Vector3.Distance(transform.position, hitInfo.point);
        var isGroundedNow = hit && distance < 0.5f;
        isGrounded = isGroundedNow;
        if (!isGrounded)
        {
            frameNotGrounded++;
            rb.drag = 0;
            if (frameNotGrounded == thresholdFrameGrounded)
            {
                IsFalling = AnimationState.Transition;
            }
        }
        else
        {
            rb.drag = groundDrag;
            IsFalling = AnimationState.None;
            frameNotGrounded = 0;
        }
    }

    private void FixedUpdate()
    {
        if (isDead || finalLeverActivated) return;
        CheckGround();
        HandleLoopingSounds();
        HandleMovement();
        StartDashing();
        CalculateDashPosition();
        ExecuteDash();
    }

    private void Update()
    {
        DrawDashIndicator();
        FiringSequence();
        // CheckIfPlayerIsHidden();
    }

    public void StopLoopingSounds()
    {
        audioSource.volume = 1f;
        audioSource.Stop();
        audioSource.loop = false;
        audioSource.clip = null;
        currentLoopingSound = CurrentLoopingSound.None;
    }

    private void HandleLoopingSounds()
    {
        if (firingStage == FiringStage.aiming && currentMovableObject != -1)
        {
            // using magnetic ray sound
            if (currentLoopingSound != CurrentLoopingSound.MagneticRay)
            {
                audioSource.volume = 0.3f;
                audioSource.clip = magnetRay;
                audioSource.loop = true;
                audioSource.Play();
                currentLoopingSound = CurrentLoopingSound.MagneticRay;
            }
        } else if (IsRunning)
        {
            if (currentLoopingSound != CurrentLoopingSound.Run)
            {
                audioSource.volume = 0.3f;
                audioSource.clip = run;
                audioSource.loop = true;
                audioSource.Play();
                currentLoopingSound = CurrentLoopingSound.Run;
            }
        } else if (IsWalking)
        {
            if (currentLoopingSound != CurrentLoopingSound.Walk)
            {
                audioSource.volume = 0.3f;
                audioSource.clip = walk;
                audioSource.loop = true;
                audioSource.Play();
                currentLoopingSound = CurrentLoopingSound.Walk;
            }
        } else
        {
            if (currentLoopingSound != CurrentLoopingSound.None)
            {
                audioSource.volume = 1f;
                audioSource.Stop();
                audioSource.loop = false;
                audioSource.clip = null;
                currentLoopingSound = CurrentLoopingSound.None;
            }
        }
    }

    private void CalculateDashPosition()
    {
        var notPlayerLayer = ~LayerMask.GetMask("playerLayer", "enemyLayer");
        var hasHit = Physics.Raycast(transform.position + Vector3.up, transform.forward, out RaycastHit hit, dashDistance, notPlayerLayer, QueryTriggerInteraction.Ignore);
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
                // if we didn't hit the ground, move to the destination
                newPosition = transform.position + transform.forward * dashDistance;
            }
        }
        calculatedDashPosition = newPosition;
    }

    private void DrawDashIndicator()
    {
        if (!canDash || !canMove || !gameData.savedSettings.drawDashIndicator)
        {
            if (dashIndicator != null && dashIndicator.activeSelf)
            {
                dashIndicator.SetActive(false);
            }
            return;
        }
        if (dashIndicator == null)
        {
            dashIndicator = Instantiate(dashIndicatorPrefab, calculatedDashPosition, Quaternion.identity);
        } else
        {
            dashIndicator.SetActive(true);
            dashIndicator.transform.position = calculatedDashPosition;
        }

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
        Vector3 cameraPosition = CameraTransform.position;
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
        if (isDead) return;
        // Get input for movement
        Vector2 movementVector = gameInput.GetMovementVectorNormalized();
        Vector3 moveDir = new(movementVector.x, 0, movementVector.y);
        Vector3 rotateDir;
        moveDir = CameraTransform.forward * moveDir.z + CameraTransform.right * moveDir.x;
        moveDir.Normalize();
        moveDir.y = 0;
        rotateDir = moveDir;

        Vector3 objMoveDir = moveDir;

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

        // Handle movement
        Vector3 targetVelocity = moveDir * movementSpeed;

        Vector3 velocity = rb.velocity;
        Vector3 velocityChange = targetVelocity - velocity;

        // Clamp velocity change to prevent abrupt changes
        velocityChange.y = 0;

        // Handle ground detection
        if (!isGrounded && hasGroundUnderneath)
        {
            velocityChange.y = -9.8f;
        }

        if (lowestGroundContactPoint > 0.5f && !hasGroundUnderneath)
        {
            velocityChange = Vector3.zero;
            lowestGroundContactPoint = 0;
        }

        rb.AddForce(velocityChange, ForceMode.VelocityChange);

        // Check if the player is walking
        IsWalking = moveDir != Vector3.zero;

        if (IsWalking == false)
        {
            movementSpeed = walkSpeed;
            IsRunning = false;
        }

        if (currentMovableObject != -1)
        {
            if (currentMovableObjectRigidBody != null)
            {
                Vector3 objTargetVelocity = objMoveDir * movementSpeed;
                Vector3 objVelocityChange = objTargetVelocity - currentMovableObjectRigidBody.velocity;
                objVelocityChange.y = 0;
                currentMovableObjectRigidBody.AddForce(objVelocityChange, ForceMode.VelocityChange);
            }
            else
            {
                Debug.LogError("Movable object does not have a rigidbody!");
            }
        }

        // Smoothly rotate player towards movement direction
        if (canMove || (!gameInput.IsKeyboardMovement && firingStage == FiringStage.aiming))
        {
            float rotationSpeed = 15f;
            transform.forward = Vector3.Slerp(transform.forward, rotateDir, rotationSpeed * Time.deltaTime);
        }
        else if (firingStage == FiringStage.aiming)
        {
            var mousePosition = gameInput.GetMousePosition();
            var distance = Vector3.Distance(transform.position, CameraTransform.position);
            var worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, distance));
            transform.LookAt(worldPosition.CopyWith(y: transform.position.y));
        }
        if (isPickingUpItem)
        {
            transform.LookAt(TouchedPowerup.transform.position.CopyWith(y: transform.position.y));
        }
        if (currentMovableObject != -1)
        {
            transform.LookAt(visibleMovableObjects[currentMovableObject].transform.position.CopyWith(y: transform.position.y));
        }
    }

    private void Attack()
    {
        if (!canAttack || !canAct)
            return;

        audioSource.PlayOneShot(attackSound);
        canAct = false;
        actionCooldownTimer.Start(0.4f);
        canMove = false;
    }

    private readonly Plane[] frustumPlanes = new Plane[6];
    private bool FilterVisibleMovableObjects(GameObject obj)
    {
        if (!obj.activeSelf) return false;
        if (obj.TryGetComponent<Collider>(out var collider))
        {
            GeometryUtility.CalculateFrustumPlanes(Camera.main, frustumPlanes);
            if (!GeometryUtility.TestPlanesAABB(frustumPlanes, collider.bounds))
            {
                return false;
            }
        }
        else
        {
            Debug.LogWarning($"Movable object {obj.name} does not have a collider");
        }
        var hit = Physics.Linecast(sword.transform.position, obj.transform.position, LayerMask.GetMask("groundLayer", "wallLayer"));
        return !hit;
    }

    private void Aim()
    {
        if (!canFire || !canAct)
            return;
        aimLaserRenderer.enabled = true;
        firingStage = FiringStage.aiming;

        if (weaponSprites[weaponIndex].name == MagnetSpriteName)
        {
            visibleMovableObjects = movableObjects.Where(x => FilterVisibleMovableObjects(x)).OrderBy(x => Vector3.Distance(x.transform.position, transform.position)).ToArray();
            if (visibleMovableObjects.Length != 0)
            {
                currentMovableObject = 0;
                SetMovableObject();
            }
            else
            {
                ResetMovableObject();
                firingStage = FiringStage.notFiring;
                aimLaserRenderer.enabled = false;
            }
        }
    }

    private void SetMovableObject()
    {
        if (currentMovableObject != -1)
        {
            var obj = visibleMovableObjects[currentMovableObject];
            if (obj.TryGetComponent(out currentMovableObjectRigidBody))
            {
                if (obj.TryGetComponent(out currentMovableObjectGravity))
                {
                    currentMovableObjectGravity.applyGravity = false;
                }
                currentMovableObjectRigidBody.freezeRotation = true;
                currentMovableObjectRigidBody.useGravity = false;
                currentMovableObjectRigidBody.excludeLayers = LayerMask.GetMask("playerLayer");
                currentMovableObjectRigidBody.constraints = RigidbodyConstraints.FreezePositionY;
            }
            else
            {
                Debug.LogError("Movable object does not have a rigidbody!");
            }
        }
    }

    private void ResetMovableObject()
    {
        if (currentMovableObject != -1)
        {
            if (currentMovableObjectRigidBody != null)
            {
                currentMovableObjectRigidBody.excludeLayers = LayerMask.GetMask();
                currentMovableObjectRigidBody.freezeRotation = false;
                currentMovableObjectRigidBody.useGravity = true;
                currentMovableObjectRigidBody.constraints = RigidbodyConstraints.None;
                if (currentMovableObjectGravity != null)
                {
                    currentMovableObjectGravity.applyGravity = true;
                }
            }
        }
        currentMovableObject = -1;
        currentMovableObjectGravity = null;
        currentMovableObjectRigidBody = null;
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
        audioSource.PlayOneShot(dashSound);
        DashLoaderBorder.GetComponent<CircularProgressBar>().StartProgressBar(dashCooldown);
        IsDashing = true;
    }

    private void FireProjectileCommon()
    {
        LoaderBorder.GetComponent<CircularProgressBar>().StartProgressBar(3.0f);

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
    }

    private void FireDamageProjectile()
    {
        var startingPosition = aimLaserRenderer.GetPosition(0);
        var endingPosition = aimLaserRenderer.GetPosition(1);
        Vector3 projectilePosition = aimLaserRenderer.transform.TransformPoint(startingPosition) + transform.forward;
        var rotation = transform.rotation;
        GameObject projectile = Instantiate(damageProjectilePrefab, projectilePosition, rotation * Quaternion.Euler(90, 0, 0));
        var projectileScript = projectile.GetComponent<ProjectileScript>();
        projectileScript.endingPosition = aimLaserRenderer.transform.TransformPoint(endingPosition);
        audioSource.PlayOneShot(shootSound);
        FireProjectileCommon();
    }

    private void FireIceProjectile()
    {
        var startingPosition = aimLaserRenderer.GetPosition(0);
        var endingPosition = aimLaserRenderer.GetPosition(1);
        Vector3 projectilePosition = aimLaserRenderer.transform.TransformPoint(startingPosition) + transform.forward;
        var rotation = transform.rotation;
        GameObject projectile = Instantiate(iceProjectilePrefab, projectilePosition, rotation * Quaternion.Euler(90, 0, 0));
        var projectileScript = projectile.GetComponent<ProjectileScript>();
        projectileScript.endingPosition = aimLaserRenderer.transform.TransformPoint(endingPosition);
        audioSource.PlayOneShot(shootSound);
        FireProjectileCommon();
    }

    private void FireMagnetProjectile()
    {
        ResetMovableObject();
        FireProjectileCommon();
    }

    private void FiringSequence()
    {
        Vector3 playerFacing = transform.forward;
        switch (firingStage)
        {
            //stop movement while aiming projectile
            case FiringStage.aiming:
                if (weaponSprites[weaponIndex].name == MagnetSpriteName)
                {
                    const int nPoints = 100;
                    if (currentMovableObject == -1) return;
                    var source = aimLaserRenderer.GetPosition(0);
                    var target = aimLaserRenderer.transform.InverseTransformPoint(visibleMovableObjects[currentMovableObject].transform.position);
                    aimLaserRenderer.positionCount = nPoints;
                    for (int i = 1; i < nPoints - 1; i++)
                    {
                        var intermediate = Vector3.Lerp(source, target, i / 100f) + VectorExtensions.Random(0.5f);
                        aimLaserRenderer.SetPosition(i, intermediate);
                    }
                    aimLaserRenderer.SetPosition(nPoints - 1, target);
                }
                else
                {
                    aimLaserRenderer.positionCount = 2;
                    aimLaserRenderer.SetPosition(1, new Vector3(0.05f, .5f, -100));
                }
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
                if (weaponSprites[weaponIndex].name == GunSpriteName)
                {
                    FireDamageProjectile();
                }
                else if (weaponSprites[weaponIndex].name == IceSpriteName)
                {
                    FireIceProjectile();
                }
                else if (weaponSprites[weaponIndex].name == MagnetSpriteName)
                {
                    FireMagnetProjectile();
                }
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

    public void DashAnimationEnded()
    {
        executeDash = true;
    }

    public void SwordAnimationEnded()
    {
        canMove = true;
    }

    public void SwordDrawn()
    {
        FindAndSetupLaser();
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
        }
    }

    public void SetSwordSolidity(bool isSolid)
    {
        swordCollider.enabled = isSolid;
    }

    public void StartFalling()
    {
        IsFalling = AnimationState.Playing;
    }

    public Action OnPlayerAttack;

    public void ExecuteDash()
    {
        if (executeDash)
        {
            rb.isKinematic = true;
            var Xoffsets = new float[] { -.5f, .5f };
            for (int i = 1; i < 4; i++)
            {
                var startingPosition = transform.position + (0.5f * i * Vector3.up);
                var endingPosition = calculatedDashPosition + (0.5f * i * Vector3.up);
                for (int j = 0; j < Xoffsets.Length; j++)
                {
                    var trail = Instantiate(trailPrefab, startingPosition + Xoffsets[j] * Vector3.right, transform.rotation);
                    var trailScript = trail.GetComponent<TrailScript>();
                    trailScript.EndingPosition = endingPosition + Xoffsets[j] * Vector3.right;
                    trailScript.PlayerTransform = transform;
                }
            }
            transform.position = calculatedDashPosition;
        }
        executeDash = false;
    }

    private void StartDashing()
    {
        if (IsDashing)
        {
            if (IsWalking || IsRunning)
            {
                executeDash = true;
            }
            else
            {
                animator.SetDash();
            }
            IsDashing = false;
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
        IsSwordGlowing = true;
        DebugExt.LogCombo($"Can combo at time {Time.time} for {AttackNumber}");
    }

    public void PickupItem()
    {
        if (TouchedPowerup.Powerup.name == IceSpriteName)
            TriggerDialog("IcePowerup_pickup");
        else if (TouchedPowerup.Powerup.name == MagnetSpriteName)
            TriggerDialog("MagnetPowerup_pickup");

        weaponSprites.Add(TouchedPowerup.Powerup);
        gameData.playerData.obtainedPowerups.Add(TouchedPowerup.Powerup.name);
        Destroy(TouchedPowerup.gameObject);
        canMove = true;
        isPickingUpItem = false;
        TouchedPowerup = null;
    }

    private void FlashPlayer()
    {
        foreach (var renderer in renderers)
        {
            renderer.enabled = !renderer.enabled;
        }
    }

    private void StaggerFromHit(Vector3 direction)
    {
        if (healthSlider.value == healthSlider.minValue) return;
        healthSlider.value = Math.Clamp(healthSlider.value - 1, healthSlider.minValue, healthSlider.maxValue);
        if (healthSlider.value != healthSlider.minValue)
        {
            audioSource.PlayOneShot(ouchSound);
        }
        InvokeRepeating(nameof(FlashPlayer), 0, 0.1f);
        isStaggered = true;
        staggerTimer.Start(1.0f); 
        rb.AddForce(direction * 5_000, ForceMode.Impulse);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (isDead) return;
        var layerValue = LayerMask.NameToLayer("deathLayer");
        if (other.gameObject.layer == layerValue)
        {
           ResetPlayer();
        }
        if (isStaggered) return;
        var direction = (transform.position - other.transform.position).normalized;
        if (other.gameObject.name == "ProjectileEnemy(Clone)")
        {
            StaggerFromHit(direction);
        } else if (other.gameObject.name == "Arm_L" || other.gameObject.name == "Arm_R")
        {
            StaggerFromHit(direction);
        }
        else if (other.gameObject.name == "Bone.011" || other.gameObject.name == "Bone.014")
        {
            StaggerFromHit(direction);
        }
    }

    public void OnDestroy()
    {
        StopAllCoroutines();
        gameInput.UnregisterPlayer(this);
        DataPersistenceManager.GetInstance().UnregisterDataPersistenceObject(this);
        Destroy(dashIndicator);
    }

    public void FinalLeverActivated()
    {
        canMove = false;
        canTurn = false;
        canAct = false;
        canFire = false;
        canAttack = false;
        canDash = false;
        IsRunning = false;
        IsWalking = false;
        IsDashing = false;
        IsAttacking = false;
        finalLeverActivated = true;
        IsFalling = AnimationState.None;
    }

    #region Input actions

    public void OnMove(InputAction.CallbackContext context) { }

    public void OnMousePosition(InputAction.CallbackContext context) { }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed && !isDead && isGrounded)
        {
            OnPlayerAttack?.Invoke();
            IsAttacking = true;
            canMove = false;
            Attack();
        }
    }
    public void OnFire(InputAction.CallbackContext context)
    {
        if (isDead || !isGrounded) return;
        if (context.performed)
        {
            Aim();
        }
        else if (context.canceled)
        {
            Fire();
        }
    }
    public void OnRun(InputAction.CallbackContext context)
    {
        if (context.performed && !isDead)
        {
            if (gameData.savedSettings.holdDownToRun)
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
            if (gameData.savedSettings.holdDownToRun)
            {
                IsRunning = false;
                movementSpeed = walkSpeed;
            }
        }
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if (context.performed && !isDead)
        {
            Dash();
        }
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.performed && !isDead && isGrounded)
        {
            if (TouchedPowerup != null && canMove)
            {
                audioSource.PlayOneShot(pickupSound);
                animator.SetPickup();
                canMove = false;
                isPickingUpItem = true;
            }
            var handler = dialogBox.GetComponent<DialogHandler>();
            if (handler.IsInDialog && handler.IsDialogDismissable)
            {
                handler.DismissDialog();
            }
        }
    }

    public void OnChangeEquippedWeapon(InputAction.CallbackContext context)
    {
        if (context.performed && !isDead)
        {
            if (firingStage != FiringStage.notFiring)
                return;
            float value = context.ReadValue<float>();
            var direction = value > 0 ? Direction.Down : Direction.Up;
            if (direction == Direction.Down)
            {
                weaponIndex = (weaponIndex + 1) % weaponSprites.Count;
            }
            else
            {
                weaponIndex = (weaponIndex - 1 + weaponSprites.Count) % weaponSprites.Count;
            }
            UiWeaponImage.sprite = weaponSprites[weaponIndex];
            var renderer = swordPartWithLine.GetComponent<SkinnedMeshRenderer>();
            var materialToSwitch = weaponMaterials.First(mat => renderer.sharedMaterials.Contains(mat));
            if (weaponSprites[weaponIndex].name == GunSpriteName)
            {
                var newMaterial = weaponMaterials.First(mat => mat.name == "GlowLaser");
                renderer.SwitchMaterial(materialToSwitch, newMaterial);
                aimLaserRenderer.sharedMaterial = newMaterial;
                outlineEffect.enabled.value = false;
                ResetMovableObject();
            }
            else if (weaponSprites[weaponIndex].name == IceSpriteName)
            {
                var newMaterial = weaponMaterials.First(mat => mat.name == "light");
                renderer.SwitchMaterial(materialToSwitch, newMaterial);
                aimLaserRenderer.sharedMaterial = newMaterial;
                outlineEffect.enabled.value = false;
                ResetMovableObject();
            }
            else if (weaponSprites[weaponIndex].name == MagnetSpriteName)
            {
                var newMaterial = weaponMaterials.First(mat => mat.name == "GlowMagnet");
                renderer.SwitchMaterial(materialToSwitch, newMaterial);
                aimLaserRenderer.sharedMaterial = newMaterial;
                outlineEffect.enabled.value = true;
            }
            audioSource.PlayOneShot(switchPowerup);
        }
    }

    public void OnSwitchAimedObject(InputAction.CallbackContext context)
    {
        if (context.performed && !isDead)
        {
            if (firingStage != FiringStage.aiming || currentMovableObject == -1 || visibleMovableObjects.Length < 2)
                return;
            var oldMovableObjectIndex = currentMovableObject;
            ResetMovableObject();
            currentMovableObject = (oldMovableObjectIndex + 1) % visibleMovableObjects.Length;
            while (!visibleMovableObjects[currentMovableObject].activeSelf)
            {
                currentMovableObject = (currentMovableObject + 1) % visibleMovableObjects.Length;
            }
            SetMovableObject();
        }
    }

    #endregion


    public void TriggerDialog(string dialogId)
    {
        var dialog = DialogData.GetDialog(dialogId);
        var handler = dialogBox.GetComponent<DialogHandler>();
        handler.SetDialog(dialog.TransformText(gameInput.CurrentControl.value), dialog.WriteDuration, dialog.LingerTime);
    }

    public void SetPowerupsOnRespawn(IEnumerable<Sprite> sprites)
    {
        weaponSprites.Clear();
        weaponSprites.AddRange(sprites);
    }
}
