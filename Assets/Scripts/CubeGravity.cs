using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeGravity : MonoBehaviour
{
    private Rigidbody rb;
    public float gravity = 9.8f;
    public bool applyGravity = true;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (applyGravity)
        {
            rb.AddForce(Vector3.down * gravity, ForceMode.VelocityChange);
        }
    }
}
