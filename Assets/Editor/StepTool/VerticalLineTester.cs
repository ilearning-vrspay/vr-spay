// using UnityEditor;
// using UnityEngine;

// public class VerticalLineTester : EditorWindow
// {
//     private Vector2 scrollPosition;
//     private int numLines = 5; // Variable to control the number of lines
//     private float lineSpacing = 5.0f; // Variable to control the spacing between the lines
//     private const float lineHeight = 18.0f; // Approximate height of one line of text in pixels

//     [MenuItem("Window/Vertical Line Tester")]
//     public static void ShowWindow()
//     {
//         GetWindow<VerticalLineTester>("Vertical Line Tester");
//     }

//     void OnGUI()
//     {
//         scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

//         // Sliders to adjust number of lines and spacing
//         numLines = EditorGUILayout.IntSlider("Number of Lines", numLines, 1, 20);
//         lineSpacing = EditorGUILayout.Slider("Line Spacing", lineSpacing, 0, 20);

//         EditorGUILayout.Space(10.0f);

//         EditorGUILayout.BeginHorizontal();

//         // Calculate the height of the vertical line
//         float totalHeight = numLines * (lineHeight + lineSpacing) - lineSpacing;

//         // Vertical line with a custom color
//         GUIStyle lineStyle = new GUIStyle();
//         lineStyle.normal.background = EditorGUIUtility.whiteTexture; // Use a white texture to make the line bright
//         GUILayout.Box(GUIContent.none, lineStyle, GUILayout.Width(2), GUILayout.Height(totalHeight));

//         // Begin a vertical layout group
//         EditorGUILayout.BeginVertical();
        
//         // Create a bunch of labels based on the numLines variable
//         for (int i = 0; i < numLines; i++)
//         {
//             EditorGUILayout.BeginHorizontal();
            
//             // Use a vertical layout group to center the horizontal line vertically
//             EditorGUILayout.BeginVertical(GUILayout.Height(lineHeight));
//             GUILayout.FlexibleSpace();
//             GUILayout.Box(GUIContent.none, lineStyle, GUILayout.Width(20), GUILayout.Height(2)); // Horizontal line
//             // GUILayout.FlexibleSpace();
//             EditorGUILayout.EndVertical();

//             // Align the label immediately after the horizontal line
//             EditorGUILayout.LabelField("Tester", GUILayout.Height(lineHeight), GUILayout.Width(10));

//             EditorGUILayout.EndHorizontal();
            
//             GUILayout.Space(lineSpacing);
//         }

//         // End the vertical layout group
//         EditorGUILayout.EndVertical();

//         // End the horizontal layout group
//         EditorGUILayout.EndHorizontal();

//         EditorGUILayout.EndScrollView();
//     }
// }



using UnityEditor;
using UnityEngine;

public class VerticalLineTester : EditorWindow
{
    private Vector2 scrollPosition;
    private int numLines = 5; // Variable to control the number of lines
    private float lineSpacing = 5.0f; // Variable to control the spacing between the lines
    private const float lineHeight = 18.0f; // Approximate height of one line of text in pixels

    [MenuItem("Window/Vertical Line Tester")]
    public static void ShowWindow()
    {
        GetWindow<VerticalLineTester>("Vertical Line Tester");
    }

    void OnGUI()
    {
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        // Sliders to adjust number of lines and spacing
        numLines = EditorGUILayout.IntSlider("Number of Lines", numLines, 1, 20);
        lineSpacing = EditorGUILayout.Slider("Line Spacing", lineSpacing, 0, 20);

        EditorGUILayout.Space(10.0f);

        EditorGUILayout.BeginHorizontal();

        // Calculate the height of the vertical line
        float totalHeight = numLines * (lineHeight + lineSpacing) - lineSpacing;

        // Vertical line with a custom color
        GUIStyle lineStyle = new GUIStyle();
        lineStyle.normal.background = EditorGUIUtility.whiteTexture; // Use a white texture to make the line bright
        GUILayout.Box(GUIContent.none, lineStyle, GUILayout.Width(2), GUILayout.Height(totalHeight));

        // Begin a vertical layout group
        EditorGUILayout.BeginVertical();

        // Create a bunch of labels based on the numLines variable
        for (int i = 0; i < numLines; i++)
        {
            EditorGUILayout.BeginHorizontal();

            // Use a vertical layout group to center the horizontal line vertically
            EditorGUILayout.BeginVertical(GUILayout.Width(20), GUILayout.Height(lineHeight));
            GUILayout.FlexibleSpace();
            GUILayout.Box(GUIContent.none, lineStyle, GUILayout.Width(20), GUILayout.Height(2)); // Horizontal line
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndVertical();

            // Align the label immediately after the horizontal line
            EditorGUILayout.LabelField("Tester", GUILayout.Height(lineHeight), GUILayout.Width(50));
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(lineSpacing);
        }

        // End the vertical layout group
        EditorGUILayout.EndVertical();

        // End the horizontal layout group
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndScrollView();
    }
}