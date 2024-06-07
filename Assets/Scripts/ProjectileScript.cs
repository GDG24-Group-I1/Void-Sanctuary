using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileScript : MonoBehaviour
{
    [SerializeField] private float projectileSpeed = 30f;
    [SerializeField] private float maxDistanceFromStart = 50f;
    [SerializeField] private float minCollisionDistance = 1f;
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

    private void OnTriggerEnter(Collider other)
    {
        var distance = Mathf.Sqrt(Mathf.Pow(transform.position.x - startingPosition.x, 2) + Mathf.Pow(transform.position.z - startingPosition.z, 2));
        //Debug.Log($"distance = {distance}");
        if (distance > minCollisionDistance)
        {
            //Debug.Log("kill projectile after collision");
            Destroy(this.gameObject);
        }
    }

    void projectileMove()
    {
        transform.position += facing * Time.deltaTime * projectileSpeed;

        //destroy the prefab after it travels too far
        var distance = Mathf.Sqrt(Mathf.Pow(transform.position.x - startingPosition.x, 2) + Mathf.Pow(transform.position.z - startingPosition.z, 2));
        if (distance > maxDistanceFromStart)
        {
            Destroy(this.gameObject);
        }
    }
}
