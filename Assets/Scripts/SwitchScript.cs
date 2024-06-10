using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SwitchScript : MonoBehaviour
{
    public bool isActive = false;
    public bool singleUse = false;
    public bool foreverOn = false;

    void Start()
    {

    }

    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Projectile(Clone)" || other.gameObject.CompareTag("Sword"))
        {
            // broken is used to disable single use switches after use
            if (foreverOn)
                return;

            isActive = !isActive;
            if (isActive)
            {
                ActivateSwitch();
                if (singleUse)
                    DisableSwitch();
            }
            else
            {
                DeactivateSwitch();
            }
        }
    }

    public void ActivateSwitch()
    {
        gameObject.GetComponent<MeshRenderer>().material.color = Color.green;
        var parentObject = transform.parent;
        if (parentObject != null)
        {
            var nChildren = parentObject.childCount;
            for (int i = 0; i < nChildren; i++)
            {
                var child = parentObject.transform.GetChild(i);
                if (child.gameObject.tag == "Platform")
                    child.gameObject.GetComponent<PlatformScript>().activeSwitches += 1;
            }
        }
    }

    public void DeactivateSwitch()
    {
        gameObject.GetComponent<MeshRenderer>().material.color = Color.yellow;
        var parentObject = transform.parent;
        if (parentObject != null)
        {
            var nChildren = parentObject.childCount;
            for (int i = 0; i < nChildren; i++)
            {
                var child = parentObject.transform.GetChild(i);
                if (child.gameObject.tag == "Platform")
                    child.gameObject.GetComponent<PlatformScript>().activeSwitches -= 1;
            }
        }
    }

    public void DisableSwitch()
    {
        foreverOn = true;
        gameObject.GetComponent<MeshRenderer>().material.color = Color.blue;
    }
}
