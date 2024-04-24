using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoseManager : MonoBehaviour
{
    private List<KeyValuePair<AnimationClip, AnimationClip>> overrides = new List<KeyValuePair<AnimationClip, AnimationClip>>();
    private AnimatorOverrideController altController;
    public AnimatorOverrideController AltController
    {
        get => altController;
        set => altController = value;
    }
    private RuntimeAnimatorController baseAnimatorController;
    private Animator handAnimator;
    public HandInteractionSystem handInteractionSystem;


    private int poseIndex = 0;

    public int PoseIndex
    {
        get => poseIndex;
        set => poseIndex = value;
    }

    private readonly string[] animationNames = {
        "DefaultPose",
        "Pose2",
        "Pose3",
        "Pose4",
        "Pose5",
        "Pose6",
        "Pose7",
        "Pose8"
    };

    void Start()
        {
            Debug.Log("PoseManager started");

            Debug.Log(handInteractionSystem);
            handAnimator = handInteractionSystem.handAnimator;
            baseAnimatorController = handAnimator.runtimeAnimatorController;
        }


    bool HasAltState(int index, string side = "R")
    {
        foreach (var pair in overrides)
        {
            if (pair.Key.name == $"{side}_{animationNames[index]}_AltState" && pair.Value != null) //check if current animation has an alternative state
            {
                return true; // Return true if the override has an alternative state
            }
        }
        return false; // No valid override found
    }

    public void ChangeController(AnimatorOverrideController controller=null)
    {
        handAnimator.runtimeAnimatorController = altController;
        altController.GetOverrides(overrides);     
    }

    public void ResetController()
    {
        handAnimator.runtimeAnimatorController = baseAnimatorController;
    }



}
