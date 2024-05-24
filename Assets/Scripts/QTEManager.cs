using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QTEManager : MonoBehaviour
{
    [SerializeField] GameObject inputImage;
    List<QTESequence.XboxControllerInput> actualSequence;
    [SerializeField] List<Sprite> inputIconList;
    Dictionary<QTESequence.XboxControllerInput, Sprite> inputIconDictionary;
    private Dictionary<QTESequence.XboxControllerInput, KeyCode> inputToKeyCode;
    void Start()
    {
        
        actualSequence = GetComponent<QTESequence>().inputSequence;
        InitDictionaries();
        InitQTE();
    }

    // Update is called once per frame
    void Update()
    {
        if(actualSequence.Count != 0)
        {
            if (Input.GetKeyDown(inputToKeyCode[actualSequence[0]]))
            {
                Destroy(transform.GetChild(0).gameObject);
                actualSequence.RemoveAt(0);
            }
        }
    }

    void InitQTE()
    {
        //clean actual
        int childCount = transform.childCount;
        for (int i = childCount - 1; i >= 0; i--)
        {
            Transform child = transform.GetChild(i);
            Destroy(child.gameObject);
        }

        //display sequence
        foreach (QTESequence.XboxControllerInput input in actualSequence)
        {
            GameObject image = Instantiate(inputImage, transform);
            image.GetComponent<Image>().sprite = inputIconDictionary[input];
        }
    }
    void InitDictionaries()
    {
        inputIconDictionary = new Dictionary<QTESequence.XboxControllerInput, Sprite>();
        foreach (QTESequence.XboxControllerInput input in System.Enum.GetValues(typeof(QTESequence.XboxControllerInput)))
        {
            inputIconDictionary.Add(input, inputIconList[input.GetHashCode()]);
        }

        inputToKeyCode = new Dictionary<QTESequence.XboxControllerInput, KeyCode>
        {
            { QTESequence.XboxControllerInput.A, KeyCode.JoystickButton0 },
            { QTESequence.XboxControllerInput.B, KeyCode.JoystickButton1 },
            { QTESequence.XboxControllerInput.X, KeyCode.JoystickButton2 },
            { QTESequence.XboxControllerInput.Y, KeyCode.JoystickButton3 },
            { QTESequence.XboxControllerInput.LeftBumper, KeyCode.JoystickButton4 },
            { QTESequence.XboxControllerInput.RightBumper, KeyCode.JoystickButton5 },
            { QTESequence.XboxControllerInput.LeftStick, KeyCode.JoystickButton8 },
            { QTESequence.XboxControllerInput.RightStick, KeyCode.JoystickButton9 },
            // Add other mappings as needed
        };
    }
}
