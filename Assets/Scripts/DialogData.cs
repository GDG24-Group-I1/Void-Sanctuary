using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
public struct Dialog
{
    private static readonly Regex ReplaceRegex = new(@"{(\w+)}", RegexOptions.Compiled);
    private readonly string text;
    public float WriteDuration;
    public float? LingerTime;

    public Dialog(string text, float writeDuration, float? lingerTime = null)
    {
        this.text = text;
        WriteDuration = writeDuration;
        LingerTime = lingerTime;
    }

    public static string Replacer(ControlType type, Match match)
    {
        if (Enum.TryParse(match.Groups[1].Value, out PlayerInput icon))
        {
            return ButtonCharMapping.GetCharEquivalent(type, icon);
        }
        return "?";
    }

    public readonly string TransformText(ControlType type)
    {
        var newText = ReplaceRegex.Replace(text, delegate (Match m)
        {
            return Replacer(type, m);
        });
        return newText;
    }
}

public enum PlayerInput
{
    MeleeAttack,
    RangedAttack,
    Aim,
    Move,
    Run,
    Dash,
    SwitchPowerup,
    Interact,
    SwitchTarget, // magnet powerup specific
    MoveTarget,   // magnet powerup specific
    Pause,
}

public static class ButtonCharMapping
{

    static readonly string[] MeleeAttack = { "\u020b", "\u0201", "\u0228" };
    static readonly string[] RangedAttack = { "\u020a", "\u01fd", "\u0224" };
    static readonly string[] Aim = { "\u020c", "\u0204", "\u022b" };
    static readonly string[] Move = { "WASD", "\u0204", "\u022b" };
    static readonly string[] Run = { "SHIFT", "\u2193 \u0204", "\u2193 \u022b" };
    static readonly string[] Dash = { "SPACE", "\u0203", "\u022a" };
    static readonly string[] SwitchPowerup = { "TAB", "\u0206 \u0207", "\u022d \u022e" };
    static readonly string[] Interact = { "E", "\u0200", "\u0227" };
    static readonly string[] SwitchTarget = { "TAB", "\u2193 \u0204", "\u2193 \u022b" };
    static readonly string[] MoveTarget = { "WASD", "\u0204", "\u022b" };
    static readonly string[] Pause = { "ESC", "\u0209", "\u0230" };

    static readonly Dictionary<PlayerInput, string[]> Mapping = new()
    {
        { PlayerInput.MeleeAttack, MeleeAttack },
        { PlayerInput.RangedAttack, RangedAttack },
        { PlayerInput.Aim, Aim },
        { PlayerInput.Move, Move },
        { PlayerInput.Run, Run },
        { PlayerInput.Dash, Dash },
        { PlayerInput.SwitchPowerup, SwitchPowerup },
        { PlayerInput.Interact, Interact },
        { PlayerInput.SwitchTarget, SwitchTarget },
        { PlayerInput.MoveTarget, MoveTarget },
        { PlayerInput.Pause, Pause },
    };

    public static string GetCharEquivalent(ControlType type, PlayerInput icon)
    {
        int index = type switch
        {
            ControlType.Mouse => 0,
            ControlType.PSController => 1,
            ControlType.XboxController => 2,
            ControlType.OtherController => 2,
            _ => 0
        };
        if (Mapping.TryGetValue(icon, out string[] value))
        {
            return value[index];
        }
        return "?";
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
        {"PickupText", new Dialog("Use {MeleeAttack} to attack and and {RangedAttack} to start a ranged attack then aim with {Aim}", 2.5f, 2f) }
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