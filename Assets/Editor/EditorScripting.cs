using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Unity.VisualScripting;
using UnityEngine.Assertions;
using System.Linq;

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
        var button = GUILayout.Button("Generate unique IDs for Doors", GUILayout.Height(50), GUILayout.Width(250));
        if (button)
        {
            GenerateDoorIds();
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

    private void GenerateDoorIds()
    {
        var prefabPaths = AssetDatabase.FindAssets("Rooms t:Prefab", new[] { "Assets/Prefabs" });
        var sounds = AssetDatabase.FindAssets("t:AudioClip", new[] { "Assets/Sounds" }).Select(x => AssetDatabase.GUIDToAssetPath(x)).ToArray();
        var openDoorSound = sounds.Where(x => x.Contains("Door Opening")).First();
        var puzzleSolveSound = sounds.Where(x => x.Contains("PuzzleSolved")).First();
        var leverActivateSound = sounds.Where(x => x.Contains("Lever")).First();
        var buttonActivateSound = sounds.Where(x => x.Contains("Button")).First();
        var openDoorClip = AssetDatabase.LoadAssetAtPath<AudioClip>(openDoorSound);
        var puzzleSolveClip = AssetDatabase.LoadAssetAtPath<AudioClip>(puzzleSolveSound);
        var leverActivateClip = AssetDatabase.LoadAssetAtPath<AudioClip>(leverActivateSound);
        var buttonActivateClip = AssetDatabase.LoadAssetAtPath<AudioClip>(buttonActivateSound);
        Assert.IsTrue(prefabPaths.Length == 1);
        var prefabPath = AssetDatabase.GUIDToAssetPath(prefabPaths[0]);
        var rooms = PrefabUtility.LoadPrefabContents(prefabPath);
        var doors = rooms.GetComponentsInChildren<OpenDoors>();
        var doors2 = rooms.GetComponentsInChildren<SwitchPorta>();
        for (int i = 0; i < doors.Length; i++)
        {
            if (doors[i].doorId == null || doors[i].doorId == "")
            {
                doors[i].doorId = System.Guid.NewGuid().ToString();
            }
            doors[i].SetSounds(openDoorClip, puzzleSolveClip);
        }
        for (int i = 0; i < doors2.Length; i++)
        {
            if (doors2[i].doorId == null || doors2[i].doorId == "")
            {
                doors2[i].doorId = System.Guid.NewGuid().ToString();
            }
            doors[i].SetSounds(openDoorClip, puzzleSolveClip);
        }

        var leve = rooms.GetComponentsInChildren<levaTerra>();
        var interruttori = rooms.GetComponentsInChildren<InterrutoreAllarme>();
        var switches = rooms.GetComponentsInChildren<SwitchScript>();
        var bossLevers = rooms.GetComponentsInChildren<BossLever>();

        for (int i = 0; i < leve.Length; i++)
        {
            if (!leve[i].TryGetComponent(out AudioSource audioSource))
            {
                audioSource = leve[i].gameObject.AddComponent<AudioSource>();
            }
            audioSource.playOnAwake = false;
            audioSource.clip = leverActivateClip;
        }
        for (int i = 0; i < interruttori.Length; i++)
        {
            if (!interruttori[i].TryGetComponent(out AudioSource audioSource))
            {
                audioSource = interruttori[i].gameObject.AddComponent<AudioSource>();
            }
            audioSource.playOnAwake = false;
            audioSource.clip = leverActivateClip;
        }
        for (int i = 0; i < switches.Length; i++)
        {
            if (!switches[i].TryGetComponent(out AudioSource audioSource))
            {
                audioSource = switches[i].gameObject.AddComponent<AudioSource>();
            }
            audioSource.playOnAwake = false;
            audioSource.clip = buttonActivateClip;
        }
        for (int i = 0; i < bossLevers.Length; i++)
        {
            if (!bossLevers[i].TryGetComponent(out AudioSource audioSource))
            {
                audioSource = bossLevers[i].gameObject.AddComponent<AudioSource>();
            }
            audioSource.playOnAwake = false;
            audioSource.clip = leverActivateClip;
        }

        PrefabUtility.SaveAsPrefabAsset(rooms, prefabPath);
    }

    private void ChangeStateOfLightAndRenderersInPrefab(string prefabPath)
    {
        var room = PrefabUtility.LoadPrefabContents(prefabPath);
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
