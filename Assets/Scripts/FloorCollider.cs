using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorCollider : MonoBehaviour
{
    public Action CollisionEnterCallback { get; set; }
    public Action CollisionExitCallback { get; set; }

    public Action CollisionStayCallback { get; set; }

    private LayerMask groundLayer;

    private void Start()
    {
        groundLayer = LayerMask.NameToLayer("groundLayer");
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.collider.gameObject.layer == groundLayer)
        {
            CollisionStayCallback?.Invoke();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject.layer == groundLayer)
        {
            CollisionEnterCallback?.Invoke();
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.gameObject.layer == groundLayer)
        {
            CollisionExitCallback?.Invoke();
        }
    }
}
