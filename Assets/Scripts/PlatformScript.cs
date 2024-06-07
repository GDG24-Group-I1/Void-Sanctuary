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
    public int activeSwitches = 0;
    private int movementStage = 0;
    private bool backtracking = false;
    private bool canMove = true;
    private Timer movementCooldownTimer;

    [SerializeField] private movementTypes movementType = movementTypes.forwardAndBackLoop;
    [SerializeField] private int requiredSwitches = 1;
    [SerializeField] private Vector3[] movements = { new Vector3(0, 4, 0) };
    [SerializeField] private int[] travelDistance = { 10 };

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

    void Update()
    {
        if (activeSwitches >= requiredSwitches && canMove)
        {
            movePlatform();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "Player")
        {
            Debug.Log($"{collision.gameObject.name} is now child of platform");
            collision.gameObject.transform.parent = transform;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.name == "Player")
        {
            Debug.Log($"{collision.gameObject.name} is no longer child of platform");
            collision.gameObject.transform.parent = null;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        
    }

    private void movePlatform()
    {
        transform.position += movements[movementStage] * Time.deltaTime;

        var horizontalDistance = Mathf.Sqrt(Mathf.Pow(transform.position.x - startingPosition.x, 2) + Mathf.Pow(transform.position.z - startingPosition.z, 2));
        var distance = Mathf.Sqrt(Mathf.Pow(transform.position.y - startingPosition.y, 2) + Mathf.Pow(horizontalDistance, 2));
        
        if (distance > travelDistance[movementStage])
        {
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
                    movementCooldownTimer.Start(3.0f);
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
                    movementCooldownTimer.Start(3.0f);
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
                controlSwitch.gameObject.GetComponent<SwitchScript>().singleUse = true;
            }
        }
    }
}
