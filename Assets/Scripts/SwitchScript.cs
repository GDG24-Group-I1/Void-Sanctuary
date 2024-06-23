using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SwitchScript : MonoBehaviour
{
    public bool isActive = false;
    public bool singleUse = false;
    public bool foreverOn = false;
    public float cooldownTime = 2.0f;
    private bool onCooldown = false;
    private Timer cooldownTimer;

    void Start()
    {
        cooldownTimer = new Timer(this)
        {
            OnTimerElapsed = () => { onCooldown = false; return null; }
        };
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (onCooldown)
            return;
        if (other.gameObject.name == "Projectile(Clone)" || other.gameObject.CompareTag("Sword"))
        {
            // broken is used to disable single use switches after use
            onCooldown = true;
            cooldownTimer.Start(cooldownTime);
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
        //gameObject.GetComponent<MeshRenderer>().material.color = Color.green;
        gameObject.GetComponent<MeshRenderer>().materials[0].color = Color.green;

            // Ottieni il materiale del MeshRenderer
             Material material = gameObject.GetComponent<MeshRenderer>().materials[0];

            // Abilita l'emissione sul materiale
             material.EnableKeyword("_EMISSION");

             // Cambia il colore dell'emissione
            material.SetColor("_EmissionColor", Color.green);
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
