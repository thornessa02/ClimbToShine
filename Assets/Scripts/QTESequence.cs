using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QTESequence : MonoBehaviour
{
    public List<XboxControllerInput> inputSequence;

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
        LeftStick,
        RightStick,
        DPadUp,
        DPadDown,
        DPadLeft,
        DPadRight,
        //LeftStickUp,
        //LeftStickDown,
        //LeftStickLeft,
        //LeftStickRight,
        //RightStickUp,
        //RightStickDown,
        //RightStickLeft,
        //RightStickRight
    }

}
