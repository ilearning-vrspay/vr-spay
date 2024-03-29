using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(InstructionDeliveryControllerEditor))]
public class CommentEditor : Editor
{
    SerializedProperty description;

    private void OnEnable()
    {
        // Bind the SerializedProperty to the description field in MyComponent
        description = serializedObject.FindProperty("description");
    }

    public override void OnInspectorGUI()
    {
        // Draw the default inspector
        DrawDefaultInspector();

        // Make changes to serializedObject editable in the Inspector
        serializedObject.Update();

        // Add a text area for the description
        EditorGUILayout.LabelField("Component Description");
        description.stringValue = EditorGUILayout.TextArea(description.stringValue, GUILayout.MaxHeight(75));

        // Apply changes to the serializedObject
        serializedObject.ApplyModifiedProperties();
    }
}
