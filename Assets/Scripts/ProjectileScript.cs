using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileScript : MonoBehaviour
{
    [SerializeField] private float projectileSpeed = 30f;
    [SerializeField] private GameObject IceCube;
    [SerializeField] private LayerMask ignoreLayers;
    public Vector3 endingPosition;

    void Start()
    {

    }

    void Update()
    {
        projectileMove();
    }

    private void OnCollisionEnter(Collision other)
    {
        if (((1 << other.gameObject.layer) & ignoreLayers) != 0)
        {
            return;
        }

        Debug.Log($"projectile hit: {other.gameObject.name}");
        if (IceCube != null && other.gameObject.CompareTag("Enemy"))
        {
            Instantiate(IceCube, other.transform.position, other.transform.rotation, other.transform);
        }

        Destroy(gameObject);
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
