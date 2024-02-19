using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

[CreateAssetMenu(fileName = "Tool Video", menuName = "ScriptableObjects/Tool Video Data", order = 1)]
public class SO_ToolVideoData : ScriptableObject
{
    public VideoClip VideoClip;
    public string VideoName;
}
