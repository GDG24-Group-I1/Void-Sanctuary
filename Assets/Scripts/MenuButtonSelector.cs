using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

enum Direction
{
   Up,
   Down
}

public class MenuButtonSelector : MonoBehaviour, IDataPersistence
{
    private Selectable firstSelectable;
    [SerializeField] private Animator animator;
    [SerializeField] private AudioSource audioSource;
    private float volume;
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private GameObject xboxControllerView;
    [SerializeField] private GameObject psControllerView;
    [SerializeField] private GameObject mouseKeyboardView;
    // Start is called before the first frame update
    void Awake()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).TryGetComponent<Selectable>(out var selectable))
            {
                firstSelectable = selectable;
                break;
            }
        }
    }

    private void OnEnable()
    {
        if (EventSystem.current.currentSelectedGameObject == null && firstSelectable != null)
        {
            firstSelectable.Select();
        }
    }

    public void OnAudioChange(float value)
    {
        volume = value;
        AudioListener.volume = volume;
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
        volume = data.savedSettings.volume;
        AudioListener.volume = volume;
        volumeSlider.value = volume;
    }

    public void SaveData(GameData data)
    {
        data.savedSettings.volume = volume;
    }
}
