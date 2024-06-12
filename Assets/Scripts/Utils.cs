// #define COMBO_DEBUG
using UnityEngine;

public static class DebugExt
{
    public static void LogCombo(string message, bool always = false)
    {
        if (always)
        {
            Debug.Log($"[COMBO]: {message}");
        }
        else
        {
#if COMBO_DEBUG
        Debug.Log($"[COMBO]: {message}");
#endif
        }
    }
}