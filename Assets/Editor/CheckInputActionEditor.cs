using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CheckInputAction))]
public class CheckInputActionEditor : Editor
{
    SerializedProperty checkTypeProp;
    SerializedProperty indexCheckProp;
    SerializedProperty targetSqueezePercentageProp;

    void OnEnable()
    {
        checkTypeProp = serializedObject.FindProperty("CheckType");
        indexCheckProp = serializedObject.FindProperty("IndexCheck");
        targetSqueezePercentageProp = serializedObject.FindProperty("TargetSqueezePercentage");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(checkTypeProp);

        CheckInputAction.checkType checkTypeEnum = (CheckInputAction.checkType)checkTypeProp.enumValueIndex;

        if (checkTypeEnum == CheckInputAction.checkType.Pose || checkTypeEnum == CheckInputAction.checkType.ToolVariation)
        {
            EditorGUILayout.PropertyField(indexCheckProp);
        }
        else if (checkTypeEnum == CheckInputAction.checkType.GripSqueeze)
        {
            targetSqueezePercentageProp.floatValue = EditorGUILayout.Slider("Target Squeeze Percentage", targetSqueezePercentageProp.floatValue, 0, 100);
        }

        EditorGUILayout.PropertyField(serializedObject.FindProperty("InputAction"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("CheckEvent"));

        serializedObject.ApplyModifiedProperties();
    }
}
