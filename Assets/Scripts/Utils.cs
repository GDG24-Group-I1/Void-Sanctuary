// #define COMBO_DEBUG
// #define CAMERA_DEBUG
using System.Collections.Generic;
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

public static class CachedResources
{
    private static readonly Dictionary<string, Object> cache = new();

    public static T Load<T>(string path) where T : Object
    {
        if (cache.ContainsKey(path))
        {
            return (T)cache[path];
        }
        else
        {
            T resource = Resources.Load<T>(path);
            if (resource == null)
            {
                Debug.LogError($"Failed to load resource at path: {path}");
                return null;
            }
            cache.Add(path, resource);
            return resource;
        }

    }
}