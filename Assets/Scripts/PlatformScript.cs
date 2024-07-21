using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UIElements.Experimental;

public class PlatformScript : MonoBehaviour
{
    enum movementTypes { loop, single, forwardAndBackSingle, forwardAndBackLoop }
    private Vector3 startingPosition;
    private int activeSwitches = 0;
    private int movementStage = 0;
    private bool backtracking = false;
    private bool canMove = true;
    private Timer movementCooldownTimer;

    [SerializeField] private movementTypes movementType = movementTypes.forwardAndBackLoop;
    [SerializeField] private int requiredSwitches = 1;
    [SerializeField] private Vector3[] movements = { new Vector3(0, 4, 0) };
    [SerializeField] private float speed = 4f;
    [SerializeField] private float cycleCooldown = 1f;

    void Start()
    {
        startingPosition = transform.position;

        movementCooldownTimer = new Timer(this)
        {
            OnTimerElapsed = () =>
            {
                canMove = true;
                return null;
            }
        };
        if (movementType == movementTypes.single)
            SetSingleUseSwitches();
    }

    void FixedUpdate()
    {
        if (activeSwitches >= requiredSwitches && canMove)
        {
            movePlatform();
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log($"{collision.gameObject.name} is now child of {gameObject.name}");
            collision.gameObject.transform.parent = transform;
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log($"{collision.gameObject.name} is no longer child of {gameObject.name}");
            // only remove parent if it is the current parent
            if (collision.gameObject.transform.parent == transform)
            {
                collision.gameObject.transform.parent = null;
            }
        }
    }

    private void movePlatform()
    {
        transform.position = Vector3.MoveTowards(transform.position, transform.position + movements[movementStage], speed * Time.fixedDeltaTime * 0.25f);

        var distance = Vector3.Distance(transform.position, startingPosition);

        if (distance > movements[movementStage].magnitude)
        {
            transform.position = startingPosition + movements[movementStage];
            // setting current pos as step position to mesure distance traveled from
            startingPosition = transform.position;
            if (backtracking)
                movementStage -= 1;
            else 
                movementStage += 1;

            // if movement stages are completed, take actions based on movement type
            if (movementStage == movements.Length || movementStage < 0)
            {
                // revert movement
                if (movementType == movementTypes.forwardAndBackSingle || movementType == movementTypes.forwardAndBackLoop)
                {
                    backtracking = !backtracking;
                    if (backtracking)
                        movementStage -= 1;
                    else
                        movementStage += 1;
                    canMove = false;
                    movementCooldownTimer.Start(cycleCooldown);
                    for (int i = 0; i < movements.Length; i++) 
                    {
                        movements[i] = -movements[i];
                    }
                    // stop movement after reverse movement is complete
                    if (movementType == movementTypes.forwardAndBackSingle && !backtracking)
                    {
                        canMove = false;
                        TurnOffSwitches();
                    }
                }
                // wait a moment then restart
                if (movementType == movementTypes.loop)
                {
                    canMove = false;
                    movementCooldownTimer.Start(cycleCooldown);
                    movementStage = 0;
                }
                // stop movement
                if (movementType == movementTypes.single)
                {
                    canMove = false;
                }
            }
        }
    }

    private void TurnOffSwitches()
    {
        var parentObject = transform.parent;
        if (parentObject != null)
        {
            for (int i = 0; i < requiredSwitches; i++)
            {
                var controlSwitch = parentObject.transform.GetChild(i+1);
                controlSwitch.gameObject.GetComponent<SwitchScript>().DeactivateSwitch();
            }
        }
    }

    private void SetSingleUseSwitches()
    {
        var parentObject = transform.parent;
        if (parentObject != null)
        {
            for (int i = 0; i < requiredSwitches; i++)
            {
                var controlSwitch = parentObject.transform.GetChild(i + 1);
                //controlSwitch.gameObject.GetComponent<SwitchScript>().singleUse = true;
            }
        }
    }

    public void Input(int receivedInput)
    {
        activeSwitches += receivedInput;
    }
}
