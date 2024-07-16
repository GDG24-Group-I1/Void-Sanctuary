using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ResetCubes : MonoBehaviour
{
    // Start is called before the first frame update
    private Vector3[] originalPositions;
    private Rigidbody[] cubeObjects;

    void Start()
    {
        cubeObjects = GameObject.FindGameObjectsWithTag("MovableObject").Select(x => x.GetComponent<Rigidbody>()).ToArray();
        originalPositions = cubeObjects.Select(x => x.transform.position).ToArray();
    }

    private void OnTriggerEnter(Collider other)
    {
        foreach (var (cube, ogPosition) in cubeObjects.Zip(originalPositions, (x, y) => (x, y)))
        {
            cube.position = ogPosition;
        }
    }
}