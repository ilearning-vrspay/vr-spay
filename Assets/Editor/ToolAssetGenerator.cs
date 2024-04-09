using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class ToolAssetGenerator : EditorWindow
{
    private string baseFolderPath = "Assets/YourBaseFolder";
    private string basePrefix = "";
    private List<string> newPrefixes = new List<string>();

    [MenuItem("Tools/Tool Asset Generator")]
    public static void ShowWindow()
    {
        GetWindow<ToolAssetGenerator>("Tool Asset Generator");
    }

    void OnGUI()
    {
        GUILayout.Label("Generate Tool Assets", EditorStyles.boldLabel);

        baseFolderPath = EditorGUILayout.TextField("Base Folder Path", baseFolderPath);
        basePrefix = EditorGUILayout.TextField("Prefix to Replace", basePrefix);

        if (GUILayout.Button("Add New Prefix"))
        {
            newPrefixes.Add("");
        }

        for (int i = 0; i < newPrefixes.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            newPrefixes[i] = EditorGUILayout.TextField($"Prefix {i + 1}", newPrefixes[i]);
            if (GUILayout.Button("X", GUILayout.Width(20)))
            {
                newPrefixes.RemoveAt(i);
            }
            EditorGUILayout.EndHorizontal();
        }

        if (GUILayout.Button("Generate Assets"))
        {
            foreach (string prefix in newPrefixes)
            {
                DuplicateAndRenameAssets(baseFolderPath, prefix);
            }
        }
    }

    private void DuplicateAndRenameAssets(string path, string newPrefix)
    {
        // Ensure the path ends with a "/"
        if (!path.EndsWith("/")) path += "/";
        // This line remains the same; you might want to adjust it based on how you want to name the duplicated folders
        string modifiedPath = RemoveLastPartOfPath(baseFolderPath);
        string newFolderPath = modifiedPath + "/" + newPrefix;
        Debug.Log("new folder path oy" + newFolderPath);
        // Duplicate the folder
        AssetDatabase.CopyAsset(path.TrimEnd('/'), newFolderPath.TrimEnd('/'));
        AssetDatabase.Refresh();

        // Rename contents
        var assets = AssetDatabase.FindAssets("", new[] { newFolderPath });
        foreach (var assetGUID in assets)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(assetGUID);
            string fileName = Path.GetFileName(assetPath);
            // Use basePrefix to replace with newPrefix in the file names
            if (!string.IsNullOrEmpty(basePrefix) && fileName.Contains(basePrefix))
            {
                string newFileName = fileName.Replace(basePrefix, newPrefix); 
                string newPath = assetPath.Replace(fileName, newFileName);
                AssetDatabase.RenameAsset(assetPath, newFileName);
            }
        }

        AssetDatabase.Refresh();
    }

    private string RemoveLastPartOfPath(string originalPath)
    {
        // Split the original path into segments based on the '/' delimiter
        var segments = originalPath.Split('/');

        // If there's only one segment, there's nothing to remove, return the original path
        if (segments.Length <= 1) return originalPath;

        // Reconstruct the path without the last segment
        var newPathSegments = segments.Take(segments.Length - 1);
        string newPath = string.Join("/", newPathSegments);

        return newPath;
    }
}
