using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public struct Dialog
{
    private readonly string text;
    public string Text { get => TransformText(); }
    public float WriteDuration;
    public float? LingerTime;

    public Dialog(string text, float writeDuration, float? lingerTime = null)
    {
        this.text = text;
        WriteDuration = writeDuration;
        LingerTime = lingerTime;
    }

    public string TransformText()
    {
        return text;
    }
}

public static class DialogData
{
    public static Dialog MissingDialog = new("Dialog not found", 1f);

    public static Dictionary<string, Dialog> Dialogs = new()
    {
        {"Test1", new Dialog("This is a test dialog", 1f) },
        {"Test2", new Dialog("This is a bit of a longer dialog\nEven with a newline!!!", 4f) },
        {"RepeatTest1", new Dialog("I am supposed to be used as a test for repeated dialogs", 1.5f) },
        {"PickupText", new Dialog("You can press E (or {X} on controller) to pick up items", 2.5f, 2f) }
    };

    public static Dialog GetDialog(string dialogId)
    {
        if (Dialogs.ContainsKey(dialogId))
        {
            return Dialogs[dialogId];
        }
        else
        {
            Debug.LogError("Dialog with id " + dialogId + " not found");
            return MissingDialog;
        }
    }
}