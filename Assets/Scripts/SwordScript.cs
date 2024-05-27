using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordScript : MonoBehaviour
{
    [SerializeField] private float swipeSpeed = 450;
    [SerializeField] private float maxRotation = 120;
    private float rotation = 0;
    public Vector3 pivot;
    public Vector3 axis;
    public int combo;

    void Start()
    {
        axis = new Vector3(0, 1, 0);
        switch (combo)
        {
            case 1:
                transform.RotateAround(pivot, axis, -maxRotation / 2);
                break;
            case 2:
                transform.RotateAround(pivot, axis, maxRotation / 2);
                break;
            case 3:
                transform.RotateAround(pivot, axis, -maxRotation / 2);
                break;
            default:
                break;
        }
    }

    void Update()
    {
        swordSwipe();
    }

    void swordSwipe()
    {
        switch (combo)
        {
            case 1:
                transform.RotateAround(pivot, axis, swipeSpeed * Time.deltaTime);
                rotation += swipeSpeed * Time.deltaTime;

                //destroy the prefab after it travels too far
                if (rotation >= maxRotation)
                {
                    Destroy(this.gameObject);
                }
                break;
            case 2:
                transform.RotateAround(pivot, axis, -swipeSpeed * Time.deltaTime);
                rotation += swipeSpeed * Time.deltaTime;

                //destroy the prefab after it travels too far
                if (rotation >= maxRotation)
                {
                    Destroy(this.gameObject);
                }
                break;
            case 3:
                transform.RotateAround(pivot, axis, swipeSpeed * Time.deltaTime);
                rotation += swipeSpeed * Time.deltaTime;

                //destroy the prefab after it travels too far
                if (rotation >= maxRotation)
                {
                    Destroy(this.gameObject);
                }
                break;
            default:
                break;
        }
    }
}
