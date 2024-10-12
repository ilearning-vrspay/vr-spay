using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[InitializeOnLoad]
public class Autosave
{
    static Autosave()
    {
        EditorApplication.playModeStateChanged += (PlayModeStateChange state) =>
        {
            if (state == PlayModeStateChange.ExitingEditMode)
            {
                Debug.Log("Auto-saving all open scenes...");
                EditorSceneManager.SaveOpenScenes();
                AssetDatabase.SaveAssets();
            }
        };
    }
}
