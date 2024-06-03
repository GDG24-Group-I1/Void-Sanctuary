using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformScript : MonoBehaviour
{
    public int activeSwitches = 0;
    void Start()
    {
        
    }

    void Update()
    {
        if (activeSwitches == 1)
        {
            movePlatform();
        }
    }

    void movePlatform() 
    {
        Debug.Log("moving platform");
    }
}
