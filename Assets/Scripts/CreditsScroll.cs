using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CreditsScroll : MonoBehaviour
{
    [SerializeField] private float scrollSpeed = 1.0f;
    [SerializeField] private float stopPoint = 200;
    private Timer timer;
    private bool started;
    // Start is called before the first frame update
    void Start()
    {
        timer = new Timer(this)
        {
            OnTimerElapsed = () =>
            {
                started = true;
                return null;
            }
        };
        timer.Start(2f);
    }

    // Update is called once per frame
    void Update()
    {
        if (!started) return;
        transform.position = transform.position.ShiftBy(y: Time.deltaTime * scrollSpeed);
        if (transform.position.y > stopPoint)
        {
            started = false;
        }
    }

    public void OnMainMenuButtonClick()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
