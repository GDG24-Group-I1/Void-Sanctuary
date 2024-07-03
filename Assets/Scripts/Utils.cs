// #define COMBO_DEBUG
// #define CAMERA_DEBUG
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

    public static void LogCamera(string message, bool always = false)
    {
        if (always)
        {
            Debug.Log($"[CAMERA]: {message}");
        }
        else
        {
#if CAMERA_DEBUG
            Debug.Log($"[CAMERA]: {message}");
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

public static class GameObjectExtensions
{
    public static GameObject FindInactive(string name, string ancestor)
    {
        var ancestorObject = GameObject.Find(ancestor);
        if (ancestorObject == null) return null;
        var child = ancestorObject.transform.Find(name);
        return child == null ? null : child.gameObject;
    }
}