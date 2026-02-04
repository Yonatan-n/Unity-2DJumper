#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class SingletonEditorAutoReset
{
    static SingletonEditorAutoReset()
    {
        EditorApplication.playModeStateChanged += OnPlayModeChanged;
    }

    private static void OnPlayModeChanged(PlayModeStateChange state)
    {
        if (state != PlayModeStateChange.EnteredPlayMode)
            return;

        // Find even inactive & DontDestroyOnLoad objects
        var allObjects = Resources.FindObjectsOfTypeAll<MonoBehaviour>();

        foreach (var obj in allObjects)
        {
            if (obj is ISingleton singleton)
            {
                singleton.EditorReset();
            }
        }
    }
}
#endif
