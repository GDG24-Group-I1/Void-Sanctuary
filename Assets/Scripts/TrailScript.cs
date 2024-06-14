using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailScript : MonoBehaviour
{
    public Vector3 EndingPosition { get; set; }
    [SerializeField] private float trailSpeed;

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, EndingPosition, trailSpeed * Time.deltaTime);
    }
}
