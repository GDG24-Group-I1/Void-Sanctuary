using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeGravity : MonoBehaviour
{
    private Rigidbody rb;
    public float gravity = 9.8f;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rb.AddForce(Vector3.down * gravity, ForceMode.VelocityChange);
    }
}
