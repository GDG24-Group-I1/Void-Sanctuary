using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class levaTerra : MonoBehaviour
{
    // the lever is single use
    [SerializeField] private bool forDoors = true; // if true, sends signal to a door, else to a platform
    [SerializeField] private string openingDoor = "door_exit"; // the door this lever will open (or platform it will activate)
    [SerializeField] private string closingDoor = "door_entrance"; // the door this lever will close

    [SerializeField] private GameObject swordEnemyPrefab;
    [SerializeField] private GameObject projectileEnemyPrefab;
    [SerializeField] private Vector3[] enemySpawnPoints = { };
    [SerializeField] private EnemyType[] enemySpawnTypes = { };
    public bool active = false;
    public Animator animator;
    void Start()
    {
        animator = GetComponent<Animator>(); // Ottiene il riferimento all'Animator dell'oggetto corrente
    }


    void OnTriggerEnter(Collider other)
    { 
        if (other.gameObject.name == "Projectile(Clone)" || other.gameObject.CompareTag("Sword"))
        {
            if (!active)
            {
                active = true;
                changeLightColor();
                animator.SetTrigger("attivo");
                sendSignal();
            }
        }
    }

    void sendSignal()
    {
        if (forDoors)
        {
            var switches = transform.parent;
            var switchesAndDoors = switches.parent;
            Transform doors = null;

            for (int i = 0; i < switchesAndDoors.childCount; i++)
                if (switchesAndDoors.GetChild(i).name == "Doors")
                    doors = switchesAndDoors.GetChild(i);

            if (doors == null)
                return;

            for (int i = 0; i < doors.childCount; i++)
                if (doors.GetChild(i).name == openingDoor)
                {
                    doors.GetChild(i).gameObject.GetComponent<OpenDoors>().Input(1);
                }
                else if (doors.GetChild(i).name == closingDoor)
                {
                    doors.GetChild(i).gameObject.GetComponent<OpenDoors>().Input(-1);
                }
        }
        else
        {
            var switches = transform.parent;
            var switchesAndDoors = switches.parent;
            Transform doors = null;

            for (int i = 0; i < switchesAndDoors.childCount; i++)
                if (switchesAndDoors.GetChild(i).name == "Doors")
                    doors = switchesAndDoors.GetChild(i);

            if (doors == null)
                return;

            for (int i = 0; i < doors.childCount; i++)
                if (doors.GetChild(i).name == openingDoor)
                {
                    doors.GetChild(i).gameObject.GetComponent<PlatformScript>().Input(1);
                }
        }

        if(enemySpawnPoints.Length > 0)
        {


        }
    }

    void changeLightColor()
    {
        var lightColor = Color.green;
        if (!active)
            lightColor = Color.red;

        for (int i = 0; i < gameObject.transform.childCount; i++)
            if (gameObject.transform.GetChild(i).name == "Sphere")
            {
                var light = gameObject.transform.GetChild(i);

                // Ottieni il materiale del MeshRenderer
                Material material = light.GetComponent<MeshRenderer>().materials[0];
                material.color = lightColor;

                // Abilita l'emissione sul materiale
                material.EnableKeyword("_EMISSION");

                // Cambia il colore dell'emissione
                material.SetColor("_EmissionColor", lightColor);
            }
    }
}
