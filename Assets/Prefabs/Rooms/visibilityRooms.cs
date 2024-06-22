using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class visibilityRooms : MonoBehaviour
{
    private GameObject[] rooms; // Array contenente tutte le stanze

    private void Start()
    {
        rooms = GameObject.FindGameObjectsWithTag("rooms");
        // Assicurati che solo la stanza iniziale sia visibile all'inizio
        UpdateRoomVisibility(null);
    }

    // FIXME: this trigger is very small so you need to be very close to the door to trigger it
    //        make it bigger

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            UpdateRoomVisibility(gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            UpdateRoomVisibility(null);
        }
    }

    private void UpdateRoomVisibility(GameObject currentRoom)
    {
        foreach (GameObject room in rooms)
        {
            bool isActive = room == currentRoom;
            SetRoomVisibility(room, isActive);
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
