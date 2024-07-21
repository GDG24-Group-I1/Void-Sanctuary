using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

enum Direction
{
   Up,
   Down
}

public class MenuButtonSelector : MonoBehaviour, IDataPersistence
{
    [SerializeField] private Animator animator;
    [SerializeField] private AudioSource audioSource;
    private GameData gameData;
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private GameObject xboxControllerView;
    [SerializeField] private GameObject psControllerView;
    [SerializeField] private GameObject mouseKeyboardView;
    [SerializeField] private Toggle holdDownToRunToggle;
    [SerializeField] private Toggle slowDownAttackToggle;
    [SerializeField] private Toggle drawDashIndicatorToggle;

    private GameInput inputHandler;

    private void Start()
    {
        inputHandler = GameObject.FindWithTag("InputHandler").GetComponent<GameInput>();
    }

    public GameInput GetGameInput()
    {
        return inputHandler;
    }

    public void OnAudioChange(float value)
    {
        gameData.savedSettings.volume = value;
        AudioListener.volume = value;
    }

    public void TogglePauseMenu(bool open, ControlType currentControlType)
    {
        xboxControllerView.SetActive(currentControlType == ControlType.XboxController || currentControlType == ControlType.OtherController);
        psControllerView.SetActive(currentControlType == ControlType.PSController);
        mouseKeyboardView.SetActive(currentControlType == ControlType.Mouse);
        audioSource.Play();
        if (open)
        {
            animator.ResetTrigger("Close");
            animator.SetTrigger("Expand");
        } else
        {
            animator.ResetTrigger("Expand");
            animator.SetTrigger("Close");
        }
    }

    public void LoadData(GameData data)
    {
        gameData = data;
        AudioListener.volume = gameData.savedSettings.volume;
        volumeSlider.value = gameData.savedSettings.volume;
        holdDownToRunToggle.isOn = gameData.savedSettings.holdDownToRun;
        drawDashIndicatorToggle.isOn = gameData.savedSettings.drawDashIndicator;
    }

    public void OnHoldDownToRunChanged(bool value)
    {
        gameData.savedSettings.holdDownToRun = value;
    }
    public void OnDrawDashIndicatorChanged(bool value)
    {
        gameData.savedSettings.drawDashIndicator = value;
    }
    public void OnExitGameClicked()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenu");
    }
}
