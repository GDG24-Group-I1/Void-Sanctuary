// #define COMBO_DEBUG
using System.Linq;
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

public static class RendererExtensions
{
    public static void SwitchMaterial(this Renderer renderer, Material originalMaterial, Material newMaterial)
    {
        var newMaterials = renderer.sharedMaterials.Select(x => x == originalMaterial ? newMaterial : x).ToList();
        renderer.SetSharedMaterials(newMaterials);
    }
}