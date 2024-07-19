using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class CheckpointAnimation : MonoBehaviour
{
    [SerializeField] private Sprite[] frames;
    [SerializeField] private int framesPerSecond = 30;
    private Image image;
    // Start is called before the first frame update
    void Start()
    {
        image = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        var index = (int)(Time.time * framesPerSecond) % frames.Length; 
        image.sprite = frames[index];
    }
}
