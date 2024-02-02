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
{    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        // a reference to this editor's script and metadata 
        InstructionDeliveryController instructionDeliveryController = (InstructionDeliveryController)target;
        InstructionDeliveryMetadata instructionDeliveryMetadata = instructionDeliveryController.MetaData;

        // dialogue string
        string dialogue = instructionDeliveryController.Script;
 
        if (GUILayout.Button("Generate Audio"))
        {
            Debug.Log(instructionDeliveryMetadata.AbsAudioPath);
            string jsonFields = JObject.FromObject(new
            {
                save_path = instructionDeliveryMetadata.AbsAudioPath,
                dialogue = dialogue
            }).ToString();

            EditorCoroutineRunner.StartCoroutine(Request.post(
                "step-system/generate-instructional-audio",
                jsonFields, 
                (Response response) => { 
                    Debug.Log(response.message);
                    AssetDatabase.Refresh();
                })
            );
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
    }
}