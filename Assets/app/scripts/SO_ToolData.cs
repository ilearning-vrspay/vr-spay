using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

[CreateAssetMenu(fileName = "Tool Data", menuName = "ScriptableObjects/Tool Data", order = 1)]
public class SO_ToolData : ScriptableObject
{
    public List<string> Bullets;
    public List<SO_ToolVideoData> Videos;
    //public List<string> ImagePaths;
    //public List<SO_ToolData> EmbededTools;

    public List<SO_ToolGripAnimation> LeftHandGrips;
    public List<SO_ToolGripAnimation> RightHandGrips;

    

    //public SO_ToolGripAnimation GetLeftToolGripAnimationAt(int index = 0)
    //{
    //    return LeftHandGrips[index];
    //}

    //public SO_ToolGripAnimation GetRightToolGripAnimationAt(int index = 0)
    //{
    //    return RightHandGrips[index];
    //}

    public SO_ToolGripAnimation GetToolGripAnimationAt(int index = 0, string handedness = "Left")
    {
        if(handedness == "Left")
        {
            return LeftHandGrips[index];
        }
        else
        {
            return RightHandGrips[index];
        }
    }
}
