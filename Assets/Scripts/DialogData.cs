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
        {"SpawnRoom", new Dialog("Numerous system malfunction detected \nProcede to the generator to assess damages", 2f, 1f) },
        {"Room_2", new Dialog("The emergency lockdown has blocked the way forward \nOverride it", 2f, 1f) },
        {"Corridor_2_3", new Dialog("Structural damage registered \nYou must find another way through", 2f, 1f) },
        {"Room_3", new Dialog("Debris detected \nMaintenance units cleared for magnetic manipulation", 2f, 1f) },
        {"MagnetPowerup_pickup", new Dialog("Magnetic manipulator acquired. Press {SwitchPowerup} to change active ability, Hold {RangedAttack} to activate", 2f, 1f) },
        {"Corridor_3_4", new Dialog("Hostile security units detected ahead \nPrepare for offensive maintenance", 2f, 1f) },
        {"IcePowerup_pickup", new Dialog("Cryo beam acquired \nPress {SwitchPowerup} to change active ability", 2f, 1f) },
        {"CryoRoom", new Dialog("Pathfinding algorithm suggest creating a bridge by halting the conveyors", 2f, 1f) },
        {"GeneratorRoom", new Dialog("Begin generator reboot sequence \nWARNING: security will react to unauthorized access", 2f, 1f) },
        {"DownRoom", new Dialog("Generator succesfully restarted, procede to the bridge to verify system integrity", 2f, 1f) },
        {"BossRoom", new Dialog("WARNING: high level security asset detected", 2f, 1f) },
        {"BossDefeat", new Dialog("Turn off the security systems to begin proper maintenance", 2f, 1f) }
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