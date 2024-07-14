using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class PressurePlateScript : MonoBehaviour
{
    private bool active = false;
    private int objectsOnTrigger = 0;
    [SerializeField] private string targetObject = "door_exit"; // the object this pressureplate will trigger

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        objectsOnTrigger += 1;
        if (!active)
        {
            active = true;
            Material mat = gameObject.GetComponent<MeshRenderer>().materials[0];
            mat.color = UnityEngine.Color.green;
            mat.EnableKeyword("_EMISSION");
            mat.SetColor("_EmissionColor", UnityEngine.Color.green);
            sendSignal(1);
        }
    }
    void OnTriggerExit(Collider other)
    {
        objectsOnTrigger -= 1;
        if (objectsOnTrigger == 0)
        {
            active = false;
            Material mat = gameObject.GetComponent<MeshRenderer>().materials[0];
            mat.color = UnityEngine.Color.red;
            mat.EnableKeyword("_EMISSION");
            mat.SetColor("_EmissionColor", UnityEngine.Color.red);
            sendSignal(-1);
        }
    }

    void sendSignal(int signal)
    {
        var room = transform.parent.parent.parent;
        Transform doors = null;

        for (int i = 0; i < room.childCount; i++)
            if (room.GetChild(i).name == "Doors")
            {
                doors = room.GetChild(i);
            }

        if (doors == null)
            return;

        for (int i = 0; i < doors.childCount; i++)
            if (doors.GetChild(i).name == targetObject)
            {
                doors.GetChild(i).gameObject.GetComponent<OpenDoors>().Input(signal);
                Debug.Log($"input: {signal}");
            }
    }
}
