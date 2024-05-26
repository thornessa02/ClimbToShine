using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QTESequence : MonoBehaviour
{
    [Header("QTE")]
    public List<XboxControllerInput> inputSequence;
    public List<float> leftJoystickPos;
    public List<float> rightJoystickPos;

    [Header("Transforms")]
    public List<Transform> iconSockets;
    public List<Transform> playerSockets;

    public enum XboxControllerInput
    {
        A,
        B,
        X,
        Y,
        LeftBumper,
        RightBumper,
        LeftTrigger,
        RightTrigger,
    }

}
