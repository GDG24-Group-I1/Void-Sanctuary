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
    private Selectable[] selectables;
    // Start is called before the first frame update
    void Start()
    {
        input = GameObject.FindWithTag("InputHandler").GetComponent<GameInput>();
        input.OnChangeCurrentSelectedControl = ChangeSelectedObject;
        input.OnCurrentSelectedControlClick = ClickSelectedObject;
        selectedObjectIndex = 0;
        var selectables = new List<Selectable>();
        for (int i = 0; i < transform.childCount; i++)
        {
            var selectable = transform.GetChild(i).GetComponentInChildren<Selectable>();
            if (selectable != null)
            {
                selectables.Add(selectable);
            }
        }
        this.selectables = selectables.ToArray();
        SelectObject(this.selectables[selectedObjectIndex]);
    }

    private void ClickSelectedObject(CallbackContext ctx)
    {
        // TODO: handle all controls
        var currentSelectedObject = selectables[selectedObjectIndex].gameObject;
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

    private void SelectObject(Selectable obj)
    {
        obj.Select();
    }

    private void SwitchObject(Direction dir)
    {
        var oldIndex = selectedObjectIndex;
        if (dir == Direction.Down)
        {
            selectedObjectIndex = Math.Min(selectedObjectIndex + 1, selectables.Length - 1);
        } else
        {
            selectedObjectIndex = Math.Max(selectedObjectIndex - 1, 0);
        }
        if (oldIndex != selectedObjectIndex)
        {
            var selectable = selectables[oldIndex];
            if (selectable != null)
            {
                selectable.OnDeselect(null);
            }
            SelectObject(selectables[selectedObjectIndex]);
        }
    }

    private void ChangeSelectedObject(CallbackContext ctx)
    {
        float value = ctx.ReadValue<float>();
        SwitchObject(value > 0 ? Direction.Down : Direction.Up);
        Debug.Log($"{selectables[selectedObjectIndex].gameObject.name}");
    }
}
