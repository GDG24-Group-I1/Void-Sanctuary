using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    [SerializeField] private Vector3[] enemySpawnPoints = { new Vector3(124.5f, 35, -136.5f), new Vector3(88.5f, 35, -136.5f), new Vector3(124.5f, 35, -162.5f), new Vector3(88.5f, 35, -162.5f) };
    [SerializeField] private EnemyType[] enemySpawnTypes = { EnemyType.Ranged, EnemyType.Ranged, EnemyType.Ranged, EnemyType.Ranged };
    [SerializeField] private Transform parentRoom;

    public bool active = false;
    public Animator animator;
    private AudioSource audioSource;

    void Start()
    {
        animator = GetComponent<Animator>(); // Ottiene il riferimento all'Animator dell'oggetto corrente
        audioSource = GetComponent<AudioSource>();
    }


    void OnTriggerEnter(Collider other)
    { 
        if (other.gameObject.name == "Projectile(Clone)" || other.gameObject.CompareTag("Sword"))
        {
            if (!active)
            {
                active = true;
                audioSource.Play();
                changeLightColor();
                animator.SetTrigger("attivo");
                SendSignal();
            }
        }
    }

    void SendSignal()
    {
        if (forDoors)
        {
            var switches = transform.parent;
            var switchesAndDoors = switches.parent;
            Transform doors = null;

            for (int i = 0; i < switchesAndDoors.childCount; i++)
            {
                if (switchesAndDoors.GetChild(i).name == "Doors")
                    doors = switchesAndDoors.GetChild(i);
            }

            if (doors == null)
                return;

            for (int i = 0; i < doors.childCount; i++)
            {
                var door = doors.GetChild(i);
                if (door.name == openingDoor)
                {
                    door.gameObject.GetComponent<OpenDoors>().Input(1);
                }
                else if (door.name == closingDoor)
                {
                    door.gameObject.GetComponent<OpenDoors>().Input(-1);
                }
            }
        }
        else
        {
            var switches = transform.parent;
            var switchesAndDoors = switches.parent;
            Transform doors = null;

            for (int i = 0; i < switchesAndDoors.childCount; i++){
                if (switchesAndDoors.GetChild(i).name == "Doors")
                    doors = switchesAndDoors.GetChild(i);
            }

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
            if (enemySpawnPoints.Length != enemySpawnTypes.Length)
                return;

            GameObject enemy;
            Transform player = GameObject.FindGameObjectWithTag("Player").gameObject.transform;
            for (int i = 0; i < enemySpawnPoints.Length; i++)
            {
                if (enemySpawnTypes[i] == EnemyType.Melee)
                {
                    enemy = Instantiate(swordEnemyPrefab, enemySpawnPoints[i], Quaternion.Euler(90, 0, 0));
                    enemy.transform.SetParent(parentRoom, true);
                    enemy.GetComponent<EnemyAI>().player = player;
                }
                else if (enemySpawnTypes[i] == EnemyType.Ranged)
                {
                    enemy = Instantiate(projectileEnemyPrefab, enemySpawnPoints[i], Quaternion.Euler(90, 0, 0));
                    enemy.transform.SetParent(parentRoom, true);
                    enemy.GetComponent<EnemyAI>().player = player;
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        foreach (var (point, type) in enemySpawnPoints.ZipPair(enemySpawnTypes))
        {
            Gizmos.color = type == EnemyType.Melee ? Color.red : Color.blue;
            Gizmos.DrawWireSphere(point, 1f);
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
