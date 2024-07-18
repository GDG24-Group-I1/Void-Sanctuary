using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.UI.ScrollRect;

public class CryoBlockScript : MonoBehaviour
{
    private Timer offsetTimer;
    public bool isFrozen = false; 
    public bool canMove = false;

    [SerializeField] private Vector3 startingPosition;
    [SerializeField] private Vector3 finalPosition;
    [SerializeField] private float speed = 4f;

    void Start()
    {
        transform.position = startingPosition;
        offsetTimer = new Timer(this)
        {
            OnTimerElapsed = () =>
            {
                canMove = true;
                return null;
            }
        };
        offsetTimer.Start(Random.Range(0, 3));
    }

    // Update is called once per frame
    void Update()
    {
        if (!isFrozen && canMove)
            movePlatform();
    }

    private void movePlatform()
    {
        transform.position = Vector3.MoveTowards(transform.position, finalPosition, speed * Time.fixedDeltaTime);

        var distance = Vector3.Distance(transform.position, finalPosition);
        if (distance <= 0.01)
            transform.position = startingPosition;
    }

}
