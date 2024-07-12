using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

enum Direction
{
   Up,
   Down
}

public class MenuButtonSelector : MonoBehaviour
{
    private Selectable firstSelectable;
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
}
