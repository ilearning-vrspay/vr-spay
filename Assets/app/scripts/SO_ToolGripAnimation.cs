using UnityEngine;

[CreateAssetMenu(fileName = "Tool Grip Animation", menuName = "ScriptableObjects/Tool Grip Animation", order = 1)]
public class SO_ToolGripAnimation : ScriptableObject
{
    //public AnimationClip GripAnimationClip;
    //public AnimationClip UseAnimationClip;
    //public string GripAnimationKey;
    //public string UseAnimationKey;
    public bool isUsable;

    public string GetGripAnimationKey()
    {
        return name;
    }

    public string GetUseAnimationKey()
    {
        return $"{name} Use";
    }
}
