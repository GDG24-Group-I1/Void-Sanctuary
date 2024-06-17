using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingTrigger : MonoBehaviour
{
    public List<Transform> points; // Lista di punti tra cui la piattaforma si muove
    public float speed = 2.0f; // Velocità di movimento della piattaforma
    private int targetIndex = 0; // Indice del punto target attuale
    private bool isMoving = false; // Stato del movimento della piattaforma

    void Start()
    {
        if (points.Count > 0)
        {
            // Inizialmente, la piattaforma è ferma al primo punto della lista
            transform.position = points[0].position;
            targetIndex = 1; // Il prossimo punto verso cui la piattaforma si muove
        }
    }

    void Update()
    {
        if (isMoving && points.Count > 1)
        {
            // Muovi la piattaforma verso la posizione target attuale
            transform.position = Vector3.MoveTowards(transform.position, points[targetIndex].position, speed * Time.deltaTime);

            // Controlla se la piattaforma ha raggiunto la posizione target
            if (Vector3.Distance(transform.position, points[targetIndex].position) < 0.1f)
            {
                // Passa al prossimo punto nella lista
                targetIndex = (targetIndex + 1) % points.Count;
            }
        }
    }

    // Metodo pubblico per iniziare il movimento
    public void StartMoving()
    {
        isMoving = true;
    }

    // Funzione per visualizzare le posizioni in cui la piattaforma si muove nell'editor
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        for (int i = 0; i < points.Count; i++)
        {
            if (points[i] != null)
            {
                Gizmos.DrawSphere(points[i].position, 0.1f);
                if (i > 0)
                {
                    Gizmos.DrawLine(points[i - 1].position, points[i].position);
                }
            }
        }
    }
}
