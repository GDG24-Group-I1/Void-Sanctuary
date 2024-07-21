using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorCollider : MonoBehaviour
{
    public Action<Collision> CollisionEnterCallback { get; set; }
    public Action<Collision> CollisionExitCallback { get; set; }

    public Action<Collision> CollisionStayCallback { get; set; }

    private LayerMask groundLayer;

    private void Start()
    {
        groundLayer = LayerMask.NameToLayer("groundLayer");
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.collider.gameObject.layer == groundLayer)
        {
            CollisionStayCallback?.Invoke(collision);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject.layer == groundLayer)
        {
            CollisionEnterCallback?.Invoke(collision);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.gameObject.layer == groundLayer)
        {
            CollisionExitCallback?.Invoke(collision);
        }
    }
}
