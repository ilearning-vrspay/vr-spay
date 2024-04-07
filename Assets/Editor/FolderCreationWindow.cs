using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class FolderCreationWindow : EditorWindow
{
    private string baseFolderPath = "Assets";
    private List<string> objectNames = new List<string>();
    private List<string> subfolderNames = new List<string>();
    private string newObjectName = "";
    private string newSubfolderName = "";

    [MenuItem("Tools/Folder Creation Tool")]
    public static void ShowWindow()
    {
        GetWindow<FolderCreationWindow>("Folder Creation Tool");
    }

    void OnGUI()
    {
        GUILayout.Label("Base Folder Path", EditorStyles.boldLabel);
        baseFolderPath = EditorGUILayout.TextField("Path:", baseFolderPath);

        if (GUILayout.Button("Populate From Selection"))
        {
            PopulateFromSelection();
        }

        GUILayout.Label("Objects", EditorStyles.boldLabel);
        newObjectName = EditorGUILayout.TextField("New Object Name:", newObjectName);
        if (GUILayout.Button("Add Object"))
        {
            AddObjectName(newObjectName);
        }

        for (int i = 0; i < objectNames.Count; i++)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(objectNames[i]);
            if (GUILayout.Button("Remove"))
            {
                objectNames.RemoveAt(i);
            }
            GUILayout.EndHorizontal();
        }

        GUILayout.Label("Subfolders", EditorStyles.boldLabel);
        newSubfolderName = EditorGUILayout.TextField("New Subfolder Name:", newSubfolderName);
        if (GUILayout.Button("Add Subfolder"))
        {
            AddSubfolderName(newSubfolderName);
        }

        for (int i = 0; i < subfolderNames.Count; i++)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(subfolderNames[i]);
            if (GUILayout.Button("Remove"))
            {
                subfolderNames.RemoveAt(i);
            }
            GUILayout.EndHorizontal();
        }

        if (GUILayout.Button("Create Folders"))
        {
            CreateFolders();
        }
    }

    private void PopulateFromSelection()
    {
        foreach (var gameObject in Selection.gameObjects)
        {
            AddObjectName(gameObject.name);
        }
    }

    private void AddObjectName(string name)
    {
        if (!string.IsNullOrWhiteSpace(name) && !objectNames.Contains(name))
        {
            objectNames.Add(name);
            newObjectName = ""; // Clear the input field after adding
        }
    }

    private void AddSubfolderName(string name)
    {
        if (!string.IsNullOrWhiteSpace(name) && !subfolderNames.Contains(name))
        {
            subfolderNames.Add(name);
            newSubfolderName = ""; // Clear the input field after adding
        }
    }

    private void CreateFolders()
    {
        foreach (var objectName in objectNames)
        {
            string objectFolderPath = AssetDatabase.GenerateUniqueAssetPath($"{baseFolderPath}/{objectName}");
            AssetDatabase.CreateFolder(baseFolderPath, objectName);

            foreach (var subfolderName in subfolderNames)
            {
                AssetDatabase.CreateFolder(objectFolderPath, subfolderName);
            }
        }
        AssetDatabase.Refresh();
    }
}
