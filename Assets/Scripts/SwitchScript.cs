using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SwitchScript : MonoBehaviour
{
    public bool isActive = false;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"switch was hit by {other.gameObject.name}");
        if (other.gameObject.name == "Sword(Clone)" || other.gameObject.name == "Projectile(Clone)")
        {
            isActive = !isActive;
            if (isActive)
            {
                gameObject.GetComponent<MeshRenderer>().material.color = Color.green;
            }
            else
            {
                gameObject.GetComponent<MeshRenderer>().material.color = Color.yellow;
            }
                

        }
    }
}
