using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 20f;
    [SerializeField] private float groundDrag = 6f;
    [SerializeField] private GameInput gameInput;
    [SerializeField] private Transform camera_direction;

    private Rigidbody rb;
    private LayerMask groundLayer;
    private bool isGrounded;
    private CapsuleCollider playerCollider;
    private bool isWalking;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        playerCollider = GetComponentInChildren<CapsuleCollider>();
        if (playerCollider == null)
        {
            Debug.LogError("Player collider not found");
        }
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        // Get input for movement
        Vector2 movementVector = gameInput.GetMovementVectorNormalized();
        Vector3 moveDir = new Vector3(movementVector.x, 0, movementVector.y);
        moveDir = camera_direction.forward * moveDir.z + camera_direction.right * moveDir.x;
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

        // Check if the player is walking
        isWalking = moveDir != Vector3.zero;

        // Smoothly rotate player towards movement direction

        float rotationSpeed = 20f;
        transform.forward = Vector3.Slerp(transform.forward, moveDir, Time.deltaTime * rotationSpeed);

    }

    public bool IsWalking()
    {
        return isWalking;
    }
}
