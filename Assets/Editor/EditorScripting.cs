using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Unity.VisualScripting;

public class EditorScripting : EditorWindow
{
    bool toggled = false;
    [MenuItem("Window / Custom Controls")]
    public static void ShowWindow()
    {
        GetWindow<EditorScripting>();
    }

    private void OnGUI()
    {
        EditorGUILayout.BeginVertical();
        GUILayout.FlexibleSpace();
        var newValue = GUILayout.Toggle(toggled, "Enable room prefab renderers and lights", GUILayout.Height(50), GUILayout.Width(250));
        if (newValue != toggled)
        {
            toggled = newValue;
            ChangePrefabButtonClicked();
        }
        EditorGUILayout.EndVertical();
    }

    private void ChangePrefabButtonClicked()
    {
        var prefabPaths = AssetDatabase.FindAssets("t:Prefab", new[] { "Assets/Prefabs/Rooms" });
        for (int i = 0; i < prefabPaths.Length; i++)
        {
            var prefabPath = AssetDatabase.GUIDToAssetPath(prefabPaths[i]);
            ChangeStateOfLightAndRenderersInPrefab(prefabPath);
        }
    }

    private void ChangeStateOfLightAndRenderersInPrefab(string prefabPath)
    {
        var room = PrefabUtility.LoadPrefabContents(prefabPath);
        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        Renderer[] renderers = room.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            if (renderer.enabled != toggled)
                renderer.enabled = toggled;
        }
        // Imposta l'attivazione delle luci
        Light[] lights = room.GetComponentsInChildren<Light>();
        foreach (Light light in lights)
        {
            if (light.enabled != toggled)
                light.enabled = toggled;
        }
        var property = PrefabUtility.GetPropertyModifications(room);
        PrefabUtility.SaveAsPrefabAsset(room, prefabPath);
        PrefabUtility.UnloadPrefabContents(room);
    }
}
