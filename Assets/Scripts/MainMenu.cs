using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

[RequireComponent(typeof(AudioSource))]
public class MainMenu : MonoBehaviour, IDataPersistence
{
    [SerializeField] private VideoPlayer videoPlayer;
    private AudioSource audioSource;
    // Start is called before the first frame update
    [SerializeField] private int frameToPause = 25;
    private GameData gameData;
    [SerializeField] private GameObject canvas;
    [SerializeField] private GameObject menuButtonsPane;
    [SerializeField] private GameObject settingsPane;
    [SerializeField] private GameObject controlsPane;
    [SerializeField] private GameObject creditsPane;

    [SerializeField] private Slider volumeSlider;
    [SerializeField] private Toggle holdRunToggle;
    [SerializeField] private Toggle slowDownAttackToggle;

    public void LoadData(GameData data)
    {
        gameData = data;
        holdRunToggle.isOn = gameData.savedSettings.holdDownToRun;
        slowDownAttackToggle.isOn = gameData.savedSettings.slowDownAttack;
        volumeSlider.value = gameData.savedSettings.volume;
    }
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        canvas.SetActive(false);
        videoPlayer.SetDirectAudioMute(0, true);
        videoPlayer.sendFrameReadyEvents = true;
        videoPlayer.frameReady += OnFrameReady;
        videoPlayer.Play();
        audioSource.Play();
    }

    private void OnFrameReady(VideoPlayer source, long frameIdx)
    {
        if (frameIdx == frameToPause)
        {
            source.Pause();
            canvas.SetActive(true);
            videoPlayer.sendFrameReadyEvents = false;
        }
    }

    #region MainMenuButtons

    public void OnExitGameClicked()
    {
        Application.Quit();
    }

    public void OnSettingsClicked()
    {
        videoPlayer.gameObject.SetActive(false);
        var settingsPaneAnimator = settingsPane.GetComponent<Animator>();
        menuButtonsPane.SetActive(false);
        settingsPane.SetActive(true);
        controlsPane.SetActive(true);
        creditsPane.SetActive(false);
        settingsPaneAnimator.ResetTrigger("Close");
        settingsPaneAnimator.SetTrigger("Expand");
        var slider = settingsPane.GetComponentInChildren<Slider>();
        slider.value = gameData.savedSettings.volume;
        EventSystem.current.SetSelectedGameObject(slider.gameObject);
    }

    public void OnStartGameClicked()
    {
        audioSource.Stop();
        videoPlayer.SetDirectAudioMute(0, false);
        videoPlayer.Play();
        videoPlayer.loopPointReached += (e) => SceneManager.LoadScene("GameScene");
        canvas.SetActive(false);
    }
    #endregion


    #region SettingsMenuButtons
    public void OnSettingsClosed()
    {
        var settingsPaneAnimator = settingsPane.GetComponent<Animator>();
        settingsPaneAnimator.ResetTrigger("Expand");
        settingsPaneAnimator.SetTrigger("Close");
        EventSystem.current.SetSelectedGameObject(menuButtonsPane.GetComponentInChildren<Button>().gameObject);
        videoPlayer.gameObject.SetActive(true);
        videoPlayer.sendFrameReadyEvents = true;
        videoPlayer.frameReady += OnFrameReady;
        videoPlayer.Play();
    }

    public void OnVolumeSliderChange(float volume)
    {
        gameData.savedSettings.volume = volume;
        AudioListener.volume = volume;
    }

    public void OnControlsSettingClick()
    {
        controlsPane.SetActive(true);
        creditsPane.SetActive(false);
    }

    public void OnCreditsSettingClick()
    {
        controlsPane.SetActive(false);
        creditsPane.SetActive(true);
    }

    public void OnHoldDownToRunChanged(bool value)
    {
        gameData.savedSettings.holdDownToRun = value;
    }

    public void OnSlowdownAttackChanged(bool value)
    {
        gameData.savedSettings.slowDownAttack = value;
    }

    public void OnResetSaveDataClicked()
    {
        DataPersistenceManager.GetInstance().ResetGame(ResetType.ResetSaveData);
    }

    public void OnResetSettingsClicked()
    {
        DataPersistenceManager.GetInstance().ResetGame(ResetType.ResetSettings);
    }

    #endregion
}