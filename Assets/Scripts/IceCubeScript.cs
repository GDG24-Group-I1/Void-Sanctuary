using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceCubeScript : MonoBehaviour
{
    [SerializeField] private float Timeout;
    // Start is called before the first frame update
    void Start()
    {
        if (!float.IsInfinity(Timeout))
        {
            transform.parent.GetComponent<EnemyAI>().IsFrozen = false;
            Destroy(gameObject, Timeout);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "Projectile(Clone)")
        {
            Debug.Log("Ice cube hit, unfreezing enemy");
            transform.parent.GetComponent<EnemyAI>().IsFrozen = false;
            Destroy(gameObject);
        }
    }
}
