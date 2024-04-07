using UnityEngine;
using UnityEditor;
using Newtonsoft.Json.Linq;
using UnityEngine.Timeline;

/// <summary>
/// Custom editor for InstructionDeliveryController. 
/// Allows us to automatically generate timelines with an audio track with the help of a socket TTS python server. 
/// Allows us to play instrcutions in play mode whenever we press on a button.
/// </summary>
[CustomEditor(typeof(InstructionDeliveryController))]
public class InstructionDeliveryControllerEditor : Editor
{    
    SerializedProperty metaDataProperty;
    private void OnEnable()
    {
        metaDataProperty = serializedObject.FindProperty("MetaData");
    }
    public override void OnInspectorGUI()
    {
        // base.OnInspectorGUI();
        serializedObject.Update();

        // a reference to this editor's script and metadata 
        InstructionDeliveryController instructionDeliveryController = (InstructionDeliveryController)target;
        InstructionDeliveryMetadata instructionDeliveryMetadata = instructionDeliveryController.MetaData;

        // dialogue string
        string dialogue = instructionDeliveryController.Script;
 
        DrawPropertiesExcluding(serializedObject, "MetaData");

        if (metaDataProperty != null)
        {
            EditorGUILayout.PropertyField(metaDataProperty, includeChildren: false);

            if (metaDataProperty.isExpanded)
            {
                EditorGUI.indentLevel++;
                
                //get file name field
                SerializedProperty fileNameProperty = metaDataProperty.FindPropertyRelative("fileName");

                GUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(fileNameProperty, new GUIContent("File Name"));
                
                //use button to fill in the file name field
                if (GUILayout.Button("Use Step Name"))
                {
                    fileNameProperty.stringValue = ((InstructionDeliveryController)target).gameObject.name;
                }
                GUILayout.EndHorizontal();

                //loop through the meta data fields available and draw them
                SerializedProperty property = metaDataProperty.Copy();
                int index = 0;
                while (property.NextVisible(true) & index < 10)
                {
                    if (property.name == "fileName") continue;
                    EditorGUILayout.PropertyField(property, true);
                    index++;
                }


                

                EditorGUI.indentLevel--;
            }
        }
        

        if (GUILayout.Button("Generate Audio"))
        {
            GenerateAudio(instructionDeliveryController, dialogue);
            
        }

        if (GUILayout.Button("Generate Timeline"))
        {
            TimelineAsset timeline = CreateInstance<TimelineAsset>();
            AssetDatabase.CreateAsset(timeline, instructionDeliveryMetadata.RelTimelinePath);

            AudioClip audioClip = instructionDeliveryMetadata.AudioClip;
            if(audioClip)
            {
                AudioTrack audioTrack = timeline.CreateTrack<AudioTrack>("dialogue");
                audioTrack.CreateClip(audioClip);
            }
            AnimationClip animationClip = instructionDeliveryMetadata.AnimationClip;
            if (animationClip)
            {
                AnimationTrack audioTrack = timeline.CreateTrack<AnimationTrack>("animation");
                audioTrack.CreateClip(animationClip);
            }

            //if (FileSystem.DoesFileExistAtAbsolutePath(instructionDeliveryMetadata.AbsAudioPath))
            //{
            //    AudioClip audioClip = (AudioClip)AssetDatabase.LoadAssetAtPath(instructionDeliveryMetadata.RelAudioPath, typeof(AudioClip));
            //    AudioTrack audioTrack = timeline.CreateTrack<AudioTrack>("dialogue");
            //    audioTrack.CreateClip(audioClip);
            //}
            //if (FileSystem.DoesFileExistAtAbsolutePath(instructionDeliveryMetadata.AbsAnimationPath))
            //{
            //    AnimationClip animationClip = (AnimationClip)AssetDatabase.LoadAssetAtPath(instructionDeliveryMetadata.RelAnimationPath, typeof(AnimationClip));
            //    AnimationTrack audioTrack = timeline.CreateTrack<AnimationTrack>("animation");
            //    audioTrack.CreateClip(animationClip);
            //}
            instructionDeliveryController.TimelineAsset = timeline;
        }

        if (GUILayout.Button("Tester"))
        {
            Debug.Log(instructionDeliveryController.MetaData.RelTimelinePath);
            Debug.Log(instructionDeliveryController.MetaData.AbsTimelinePath);
        }

        // Editor buttons only when in play mode
        if (Application.isPlaying)
        {
            // Plays the instruction, useful for testing in play mode. 
            if (GUILayout.Button("Play Instructions"))
            {
                instructionDeliveryController.StartInstructionDelivery();
            }
        }
        serializedObject.ApplyModifiedProperties();
    }

    public static void GenerateAudio(InstructionDeliveryController controller, string dialogue)
    {
        Debug.Log(controller.MetaData.AbsAudioPath);
        string jsonFields = JObject.FromObject(new
        {
            save_path = controller.MetaData.AbsAudioPath,
            dialogue = dialogue
        }).ToString();

        EditorCoroutineRunner.StartCoroutine(Request.post(
            "step-system/generate-instructional-audio",
            jsonFields,
            (Response response) => {
                Debug.Log(response.message);
                AssetDatabase.Refresh();
                if (FileSystem.DoesFileExistAtAbsolutePath(controller.MetaData.AbsAudioPath))
                {
                    AudioClip audioClip = (AudioClip)AssetDatabase.LoadAssetAtPath(controller.MetaData.RelAudioPath, typeof(AudioClip));
                    controller.MetaData.AudioClip = audioClip;
                }
            })
        );
    }
}