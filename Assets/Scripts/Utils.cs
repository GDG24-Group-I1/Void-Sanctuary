// #define COMBO_DEBUG
// #define CAMERA_DEBUG
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
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
    public static GameObject FindInactive(string name, string parent)
    {
        var ancestorObject = GameObject.Find(parent);
        if (ancestorObject == null) return null;
        var child = ancestorObject.transform.Find(name);
        return child == null ? null : child.gameObject;
    }
    public static T GetComponentByName<T>(this GameObject obj, string name, bool recursive = false, bool inactive = true) where T : Component
    {
        try
        {
            if (recursive)
            {
                return obj.GetComponentsInChildren<T>(inactive).First(x => x.name == name);
            }
            else
            {
                return obj.GetComponents<T>().First(x => x.name == name);
            }
        } catch (System.InvalidOperationException)
        {
            Debug.LogError($"Failed to find component of type {typeof(T)} with name {name} on object {obj.name}");
            return null;
        }
    }
}

public static class ColorExtensions
{
    public static Color CopyWithAlpha(this Color color, float alpha)
    {
        return new Color(color.r, color.g, color.b, alpha);
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