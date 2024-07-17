using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(TrailRenderer))]
public class TrailScript : MonoBehaviour
{
    public Vector3 EndingPosition { get; set; }
    public Transform PlayerTransform { get; set; }
    [SerializeField] private float trailSpeed;
    [SerializeField] private float lingerTime;

    private Timer lingerTimer;
    private TrailRenderer trailRenderer;

    private void Start()
    {
        Assert.IsNotNull(PlayerTransform, "Player transform may not be null in TrailScript");
        lingerTimer = new Timer()
        {
            OnTimerElapsed = () =>
            {
                if(this != null)
                    Destroy(gameObject);
                return null;
            }
        };
        trailRenderer = GetComponent<TrailRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.parent != null) return;
        if (Vector3.Distance(transform.position, EndingPosition) <= 0.1f)
        {
            transform.parent = PlayerTransform;
            trailRenderer.time = lingerTime;
            lingerTimer.Start(lingerTime);
        }
        transform.position = Vector3.MoveTowards(transform.position, EndingPosition, trailSpeed * Time.deltaTime);
    }
}
