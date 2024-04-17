using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(HandInteractionSystem))]
public class HandInteractionSystemEditor : Editor
{
    public override void OnInspectorGUI()
    {
        //DrawDefaultInspector();
        // Update the serialized object
        serializedObject.Update();

        // Manually draw the script field in a disabled group to make it read-only
        EditorGUI.BeginDisabledGroup(true);
        SerializedProperty scriptProp = serializedObject.FindProperty("m_Script");
        EditorGUILayout.PropertyField(scriptProp);
        EditorGUI.EndDisabledGroup();

        // Draw all properties except the inputActionBindings array
        DrawPropertiesExcluding(serializedObject, new string[] { "inputActionBindings", "m_Script"});

        SerializedProperty bindings = serializedObject.FindProperty("inputActionBindings");
        if (bindings != null)
        {
            EditorGUILayout.PropertyField(bindings, new GUIContent("Input Action Bindings"), false);

            if (bindings.isExpanded || bindings.arraySize == 0) // Check if the bindings is expanded OR has no elements
            {
                EditorGUI.indentLevel++;

                // Property field for array size, shown even if array size is 0
                EditorGUILayout.PropertyField(bindings.FindPropertyRelative("Array.size"));

                for (int i = 0; i < bindings.arraySize; i++)
                {
                    SerializedProperty binding = bindings.GetArrayElementAtIndex(i);
                    EditorGUILayout.BeginHorizontal();

                    // Show the foldout for the element
                    binding.isExpanded = EditorGUILayout.Foldout(binding.isExpanded, $"Element {i}", true);

                    // Show a remove button
                    if (GUILayout.Button("-", GUILayout.Width(30)))
                    {
                        bindings.DeleteArrayElementAtIndex(i);
                        break; // Exit the loop to prevent further iteration on a modified array
                    }

                    EditorGUILayout.EndHorizontal();

                    // If the foldout is open, draw the properties for this element
                    if (binding.isExpanded)
                    {
                        EditorGUI.indentLevel++;
                        SerializedProperty inputType = binding.FindPropertyRelative("inputType");
                        SerializedProperty inputActionReference = binding.FindPropertyRelative("inputActionReference");

                        // Draw properties for this binding element
                        EditorGUILayout.PropertyField(inputActionReference);
                        EditorGUILayout.PropertyField(inputType);

                        // Draw the correct UnityEvent based on the selected InputType
                        HandInteractionSystem.InputActionBinding.InputType type = (HandInteractionSystem.InputActionBinding.InputType)inputType.enumValueIndex;
                        switch (type)
                        {
                            case HandInteractionSystem.InputActionBinding.InputType.Button:
                                SerializedProperty onInputReceived = binding.FindPropertyRelative("onInputReceived");
                                EditorGUILayout.PropertyField(onInputReceived, new GUIContent("On Input Received"));
                                break;
                            case HandInteractionSystem.InputActionBinding.InputType.Axis:
                                SerializedProperty onContinuousInputReceived = binding.FindPropertyRelative("onContinuousInputReceived");
                                EditorGUILayout.PropertyField(onContinuousInputReceived, new GUIContent("On Continuous Input Received"));
                                break;
                        }
                        EditorGUI.indentLevel--;
                    }
                }

                EditorGUI.indentLevel--;
            }

            // Button to add a new element, always shown outside the if condition for bindings.isExpanded
            if (GUILayout.Button("Add New Binding"))
            {
                bindings.arraySize++;
            }
        }

        serializedObject.ApplyModifiedProperties();
    }
}
