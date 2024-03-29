using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;



[CreateAssetMenu(fileName = "ToolPoseData", menuName = "ScriptableObjects/ToolPoseData", order = 0)]
public class ToolPoseData : ScriptableObject {
    public AnimationClip defaultPose;
    public AnimationClip defaultAltPose;
    
    public List<AnimationClip> additionalPoses;


}

