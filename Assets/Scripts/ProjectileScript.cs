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

        //Debug.Log($"projectile hit: {other.gameObject.name}");
        var isEnemy = other.gameObject.CompareTag("EnemyObj");
        if (IceCube != null && isEnemy)
        {
            var enemyAi = other.gameObject.GetComponent<EnemyAI>();
            if (!enemyAi.IsFrozen)
            {
                Instantiate(IceCube, other.transform.position, other.transform.rotation, other.transform);
                enemyAi.IsFrozen = true;
            }
        }

        var isBlock = other.gameObject.CompareTag("FreezableBlock");
        if (IceCube != null && isBlock)
        {
            var blockScript = other.gameObject.GetComponent<CryoBlockScript>();
            if (!blockScript.isFrozen)
            {
                Vector3 cubePos = new Vector3(other.transform.position.x + 4.5f, other.transform.position.y + 4.2f, other.transform.position.z - 1.8f);
                //Vector3 cubePos = new Vector3(-3.8f, 4.2f, 0.9f);
                Instantiate(IceCube, cubePos, other.transform.rotation, other.transform);
                blockScript.isFrozen = true;
            }
        }

        Destroy(gameObject);
    }

    public void SetTarget(Vector3 target)
    {
        endingPosition = target;
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
