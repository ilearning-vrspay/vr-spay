using UnityEngine;
using UnityEditor;

public class GameObjectCreatorTool : EditorWindow
{
    private string namesInput = "";

    [MenuItem("Tools/Create Empty Game Objects")]
    public static void ShowWindow()
    {
        GetWindow<GameObjectCreatorTool>("Create Empty Game Objects");
    }

    private void OnGUI()
    {
        GUILayout.Label("Enter Names (comma-separated):");
        namesInput = EditorGUILayout.TextField(namesInput);

        if (GUILayout.Button("Create Game Objects"))
        {
            CreateGameObjects();
        }
    }

    private void CreateGameObjects()
    {
        string[] names = namesInput.Split(',');

        foreach (string name in names)
        {
            GameObject newGameObject = new GameObject(name.Trim());
            newGameObject.transform.parent = Selection.activeTransform; // Make the new game object a child of the currently selected game object
        }
    }
}
