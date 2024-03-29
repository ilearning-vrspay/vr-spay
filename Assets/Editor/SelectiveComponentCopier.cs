using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

public class SelectiveComponentCopier : EditorWindow
{
    private GameObject sourceObject;
    private List<GameObject> targetObjects = new List<GameObject>();
    private List<bool> componentSelections = new List<bool>(); // List to store component selection states

    [MenuItem("Tools/Selective Component Copier")]
    public static void ShowWindow()
    {
        GetWindow<SelectiveComponentCopier>("Selective Comp Copier");
    }

    void OnGUI()
    {
        EditorGUILayout.HelpBox("Select source and target GameObjects. Then select components to copy.", MessageType.Info);
        
        sourceObject = EditorGUILayout.ObjectField("Source Object", sourceObject, typeof(GameObject), true) as GameObject;

        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("Target Objects:", EditorStyles.boldLabel);
        for (int i = 0; i < targetObjects.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            targetObjects[i] = EditorGUILayout.ObjectField(targetObjects[i], typeof(GameObject), true) as GameObject;
            if (GUILayout.Button("Remove", GUILayout.MaxWidth(80)))
            {
                targetObjects.RemoveAt(i);
                componentSelections.Clear(); // Clear component selections when removing a target
                i--;
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginHorizontal();

        float buttonWidth = (position.width - 10) / 2; // Calculate button width

        if (GUILayout.Button("Add Targets From Selection", GUILayout.Width(buttonWidth)))
        {
            AddTargetObjectsFromSelection();
        }
        if (GUILayout.Button("Add Target Field", GUILayout.Width(buttonWidth)))
        {
            AddTargetObjectManually();
        }

        EditorGUILayout.EndHorizontal();

        if (sourceObject != null)
        {
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Select Components to Copy:", EditorStyles.boldLabel);

            Component[] components = sourceObject.GetComponents<Component>().Where(c => !(c is Transform)).ToArray();

            // Ensure the component selections list is initialized with the correct size
            if (componentSelections.Count != components.Length)
            {
                componentSelections = new List<bool>(new bool[components.Length]);
            }

            for (int i = 0; i < components.Length; i++)
            {
                if (components[i] != null)
                {
                    // Use custom GUILayout.Toggle to manually handle selection states
                    componentSelections[i] = GUILayout.Toggle(componentSelections[i], components[i].GetType().Name);
                }
            }
        }

        EditorGUILayout.Separator();
        if (GUILayout.Button("Copy Selected Components", GUILayout.ExpandWidth(true)) && sourceObject != null && targetObjects.Count > 0)
        {
            foreach (var target in targetObjects)
            {
                CopySelectedComponents(sourceObject, target);
            }
        }
    }

    private void AddTargetObjectsFromSelection()
    {
        GameObject[] selectedObjects = Selection.gameObjects;
        targetObjects.AddRange(selectedObjects);
    }

    private void AddTargetObjectManually()
    {
        targetObjects.Add(null);
    }

    private void CopySelectedComponents(GameObject source, GameObject target)
    {
        Component[] components = source.GetComponents<Component>().Where(c => !(c is Transform)).ToArray();
        
        for (int i = 0; i < components.Length && i < componentSelections.Count; i++)
        {
            if (componentSelections[i] && components[i] != null)
            {
                UnityEditorInternal.ComponentUtility.CopyComponent(components[i]);
                UnityEditorInternal.ComponentUtility.PasteComponentAsNew(target);
            }
        }
    }
}
