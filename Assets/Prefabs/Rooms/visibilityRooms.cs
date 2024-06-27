using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class visibilityRooms : MonoBehaviour {
    // FIXME: this trigger is very small so you need to be very close to the door to trigger it
    //        make it bigger

    private void Start()
    {
        SetRoomVisibility(gameObject, false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SetRoomVisibility(gameObject, true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
           SetRoomVisibility(gameObject, false);
        }
    }

    private void SetRoomVisibility(GameObject room, bool isVisible)
    {
        Renderer[] renderers = room.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            renderer.enabled = isVisible;
        }
        // Imposta l'attivazione delle luci
        Light[] lights = room.GetComponentsInChildren<Light>();
        foreach (Light light in lights)
        {
            light.enabled = isVisible;
        }
    }
}
