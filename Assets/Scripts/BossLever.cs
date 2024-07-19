using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
public class BossLever : MonoBehaviour
{
    [SerializeField] private bool isEnabled = false;
    [SerializeField] private float requiredFadeTime = 3.0f;
    private Timer fadeTimer;
    private Image fadeImage;
    private bool isFading;
    private Animator animator;
    private AudioSource audioSource;

    private float startFadeTime;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        isFading = false;
        fadeImage = GameObject.Find("GameUI").transform.Find("FadeImage").GetComponent<Image>(); 
        fadeTimer = new Timer(this)
        {
            OnTimerElapsed = () =>
            {
                SceneManager.LoadSceneAsync("CreditScene");
                return null;
            }
        };
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isFading)
        {
            var alpha = (Time.time - startFadeTime) / requiredFadeTime;
            fadeImage.color = fadeImage.color.CopyWithAlpha(alpha);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isEnabled) return;
        if (other.gameObject.CompareTag("Sword"))
        {
            isEnabled = false;
            isFading = true;
            startFadeTime = Time.time;
            fadeTimer.Start(requiredFadeTime);
            GameObject.FindWithTag("Player").GetComponent<Player>().FinalLeverActivated();
            animator.SetTrigger("attivo");
            audioSource.Play();
        }
    }

    public void SetEnabled(bool enabled)
    {
        isEnabled = enabled;
    }
}
