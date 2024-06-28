using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class CircularProgressBar : MonoBehaviour
{
    private Image progressBarImage;
    private float startTime;
    private float endTime;
    // Start is called before the first frame update
    void Start()
    {
        progressBarImage = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        progressBarImage.fillAmount = (Time.time - startTime) / (endTime - startTime);
    }

    public void StartProgressBar(float seconds)
    {
        progressBarImage.fillAmount = 0;
        startTime = Time.time;
        endTime = Time.time + seconds;
    }
}
