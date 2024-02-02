using UnityEngine;

[System.Serializable]
public class InstructionDeliveryMetadata
{
    [UnityEngine.SerializeField] private string dataPath;
    [UnityEngine.SerializeField] private string fileName;
    [UnityEngine.SerializeField] private AudioClip audioClip;
    [UnityEngine.SerializeField] private AnimationClip animationClip;

    public AudioClip AudioClip
    {
        get
        {
            return audioClip;
        }
    }

    public AnimationClip AnimationClip
    {
        get
        {
            return animationClip;
        }
    }

    

    public string RelAudioPath
    {
        get
        {
            return FileSystem.CombinePathFilenameExt("Assets/app/audio-clips", fileName, "mp3"); 
        }
    }

    public string AbsAudioPath
    {
        get
        {
            return FileSystem.GetAbsolutePathFromUnityAssetFilePath(RelAudioPath);
        }
    }

    //public string RelAnimationPath
    //{
    //    get
    //    {
    //        string animDir = FileSystem.CombinePathFilename(dataPath, "Animation");
    //        return FileSystem.CombinePathFilenameExt(animDir, fileName, "anim");
    //    }
    //}

    //public string AbsAnimationPath
    //{
    //    get
    //    {
    //        return FileSystem.GetAbsolutePathFromUnityAssetFilePath(RelAnimationPath);
    //    }
    //}

    public string RelTimelinePath
    {
        get
        {
            string timelineDir = FileSystem.CombinePathFilename(dataPath, "Timeline");
            return FileSystem.CombinePathFilenameExt(timelineDir, fileName, "playable");
        }
    }

    public string AbsTimelinePath
    {
        get
        {
            return FileSystem.GetAbsolutePathFromUnityAssetFilePath(RelTimelinePath);
        }
    }
}


//// relative dirs
//string relativeAudioDir = instructionDeliveryController.RelativeAudioDirectory;
//string relativeTimelineDir = instructionDeliveryController.RelativeTimelineDirectory;

//// abs dirs
//string absAudioDir = FileSystem.GetAbsolutePathFromUnityAssetFilePath(relativeAudioDir);

//// step name becomes our file name
//string stepName = instructionDeliveryController.name;

//// save paths
//string relAudioSavePath = FileSystem.CombinePathFilenameExt(relativeAudioDir, stepName, "mp3");
//string absAudioSavePath = FileSystem.CombinePathFilenameExt(absAudioDir, stepName, "mp3");
//string relTimelineSavePath = FileSystem.CombinePathFilenameExt(relativeTimelineDir, stepName, "playable");
