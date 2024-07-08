using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileScript : MonoBehaviour
{
    [SerializeField] private float projectileSpeed = 30f;
    [SerializeField] private float minCollisionDistance = 1f;
    [SerializeField] private GameObject IceCube;
    private Vector3 startingPosition;
    public Vector3 endingPosition;

    void Start()
    {
        startingPosition = transform.position;
    }

    void Update()
    {
        projectileMove();
    }

    private void OnCollisionEnter(Collision other)
    {
        var distance = Mathf.Sqrt(Mathf.Pow(transform.position.x - startingPosition.x, 2) + Mathf.Pow(transform.position.z - startingPosition.z, 2));
        if (distance > minCollisionDistance)
        {
            if (IceCube != null && other.gameObject.CompareTag("Enemy"))
            {
                Instantiate(IceCube, other.transform.position, other.transform.rotation, other.transform);
            }
            Destroy(gameObject);
        }
    }

    void projectileMove()
    {
        transform.position = Vector3.MoveTowards(transform.position, endingPosition, projectileSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, endingPosition) <= 0.1f)
        {
            Destroy(gameObject);
        }
    }
}
