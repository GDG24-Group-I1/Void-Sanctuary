using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static UnityEngine.InputSystem.InputAction;

enum Direction
{
   Up,
   Down
}

public class MenuButtonSelector : MonoBehaviour
{
    private GameInput input;
    private int selectedObjectIndex;
    private GameObject currentSelectedObject;
    // Start is called before the first frame update
    void Start()
    {
        input = GameObject.FindWithTag("InputHandler").GetComponent<GameInput>();
        input.OnChangeCurrentSelectedControl = ChangeSelectedObject;
        input.OnCurrentSelectedControlClick = ClickSelectedObject;
        selectedObjectIndex = 0;
        currentSelectedObject = transform.GetChild(selectedObjectIndex).gameObject;
        AddOutline(currentSelectedObject);
    }

    private void ClickSelectedObject(CallbackContext ctx)
    {
        // TODO: handle all controls
        var button = currentSelectedObject.GetComponentInChildren<Button>();
        var toggle = currentSelectedObject.GetComponentInChildren<Toggle>();
        if (button != null)
        {
            button.onClick.Invoke();
        } else if (toggle != null)
        {
            toggle.isOn = !toggle.isOn;
        }
    }

    private void AddOutline(GameObject obj)
    {
        var outline = obj.GetComponentInChildren<Outline>();
        if (outline != null)
        {
            outline.enabled = true;
        } else
        {
            // FIXME: this does not handle tmp_text
            var text = obj.GetComponentInChildren<Text>();
            var image = obj.GetComponentInChildren<Image>();
            if (image != null)
            {
                outline = image.gameObject.AddComponent<Outline>();
            } else if (text != null)
            {
                outline = text.gameObject.AddComponent<Outline>();
            }
            outline.effectColor = Color.yellow;
            outline.effectDistance = new Vector2(5, 5);
        }
    }

    private void SwitchObject(Direction dir)
    {
        var oldIndex = selectedObjectIndex;
        if (dir == Direction.Down)
        {
            selectedObjectIndex = Math.Min(selectedObjectIndex + 1, transform.childCount - 1);
        } else
        {
            selectedObjectIndex = Math.Max(selectedObjectIndex - 1, 0);
        }
        if (oldIndex != selectedObjectIndex)
        {
            var outline = currentSelectedObject.GetComponentInChildren<Outline>();
            if (outline != null)
            {
                outline.enabled = false;
            }
            currentSelectedObject = transform.GetChild(selectedObjectIndex).gameObject;
            AddOutline(currentSelectedObject);
        }
    }

    private void ChangeSelectedObject(CallbackContext ctx)
    {
        float value = ctx.ReadValue<float>();
        SwitchObject(value > 0 ? Direction.Down : Direction.Up);
        Debug.Log($"{currentSelectedObject.name}");
    }
}
