using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileScript : MonoBehaviour
{
    [SerializeField] private float projectileSpeed = 30f;
    [SerializeField] private float maxDistanceFromStart = 50f;
    private Vector3 startingPosition;
    public Vector3 facing;

    void Start()
    {
        startingPosition = transform.position;
    }

    void Update()
    {
        projectileMove();
    }

    void projectileMove()
    {
        //Debug.Log($"facing {facing} | position {transform.position}");
        //transform.Translate(facing * Time.deltaTime * projectileSpeed);
        transform.position += facing * Time.deltaTime * projectileSpeed;

        //destroy the prefab after it travels too far
        var distance = Mathf.Sqrt(Mathf.Pow(transform.position.x - startingPosition.x, 2) + Mathf.Pow(transform.position.z - startingPosition.z, 2));
        if (distance > maxDistanceFromStart)
        {
            Destroy(this.gameObject);
        }
    }
}
