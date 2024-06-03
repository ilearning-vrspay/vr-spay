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

        GUI.enabled = false;
        EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour((CheckInputAction)target), typeof(CheckInputAction), false);
        GUI.enabled = true;
        EditorGUILayout.PropertyField(checkTypeProp);

        CheckInputAction.checkType checkTypeEnum = (CheckInputAction.checkType)checkTypeProp.enumValueIndex;

        switch (checkTypeEnum)
        {
            case CheckInputAction.checkType.PoseChange:
                EditorGUILayout.PropertyField(indexCheckProp);
                break;
            case CheckInputAction.checkType.ToolVariationChange:
                EditorGUILayout.PropertyField(indexCheckProp);
                break;
            case CheckInputAction.checkType.GripSqueeze:
                targetSqueezePercentageProp.floatValue = EditorGUILayout.Slider("Target Squeeze Percentage", targetSqueezePercentageProp.floatValue, 0, 100);
                break;
            case CheckInputAction.checkType.PickupTool:
                break;
            case CheckInputAction.checkType.ReleaseTool:
                break;
        }

        



        

        EditorGUILayout.PropertyField(serializedObject.FindProperty("InputAction"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("CheckEvent"));

        serializedObject.ApplyModifiedProperties();
    }
}
