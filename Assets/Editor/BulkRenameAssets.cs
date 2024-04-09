using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;

public class BulkRenameAssets : EditorWindow
{
    private string searchPattern = "*";
    private string oldPrefix = "";
    private string newPrefix = "";

    [MenuItem("Tools/Bulk Rename Assets")]
    public static void ShowWindow()
    {
        GetWindow<BulkRenameAssets>("Bulk Rename Assets");
    }

    void OnGUI()
    {
        GUILayout.Label("Bulk Rename Assets", EditorStyles.boldLabel);

        searchPattern = EditorGUILayout.TextField("Search Pattern (e.g., *.prefab)", searchPattern);
        oldPrefix = EditorGUILayout.TextField("Old Prefix", oldPrefix);
        newPrefix = EditorGUILayout.TextField("New Prefix", newPrefix);

        if (GUILayout.Button("Rename"))
        {
            RenameAssets();
        }
    }

    private void RenameAssets()
    {
        // Confirm with the user since this operation can't be undone easily
        if (!EditorUtility.DisplayDialog("Bulk Rename Assets",
            "Are you sure you want to rename assets? This operation cannot be easily undone.",
            "Yes", "No"))
        {
            return;
        }

        // Get all asset paths that match the search pattern
        string[] allAssetPaths = AssetDatabase.GetAllAssetPaths()
            .Where(path => path.Contains(oldPrefix) && path.EndsWith(Path.GetExtension(searchPattern))).ToArray();

        foreach (string path in allAssetPaths)
        {
            string directory = Path.GetDirectoryName(path);
            string fileName = Path.GetFileName(path);
            if (fileName.StartsWith(oldPrefix))
            {
                string newFileName = fileName.Replace(oldPrefix, newPrefix);
                string newPath = Path.Combine(directory, newFileName);

                // Perform the rename operation
                AssetDatabase.RenameAsset(path, newFileName);
                Debug.Log($"Renamed {fileName} to {newFileName}");
            }
        }

        // Refresh the AssetDatabase to show the renamed assets
        AssetDatabase.Refresh();
    }
}
