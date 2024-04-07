using UnityEngine;

[System.Serializable]
public class InstructionDeliveryMetadata
{
    //[UnityEngine.SerializeField] private string dataPath;
    [HideInInspector]
    [UnityEngine.SerializeField] private string fileName;
    public AudioClip AudioClip;
    public AnimationClip AnimationClip;
        

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
            return FileSystem.CombinePathFilenameExt("Assets/app/timelines", fileName, "playable");
        }
    }

    public string AbsTimelinePath
    {
        get
        {
            return FileSystem.GetAbsolutePathFromUnityAssetFilePath(RelTimelinePath);
        }
    }

    public void SetFileName(string name)
    {
        fileName = name;
    }

    // Public method to get fileName
    public string GetFileName()
    {
        return fileName;
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
