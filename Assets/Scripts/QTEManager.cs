using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QTEManager : MonoBehaviour
{
    [SerializeField] LevelGenerator levelGenerator;
    [SerializeField] GameObject inputImage;
    
    [SerializeField] List<Sprite> inputIconList;
    Dictionary<QTESequence.XboxControllerInput, Sprite> inputIconDictionary;
    private Dictionary<QTESequence.XboxControllerInput, KeyCode> inputToKeyCode;

    List<QTESequence.XboxControllerInput> actualSequence;
    bool inQTE = false;
    void Start()
    {
        actualSequence = new List<QTESequence.XboxControllerInput>();
        InitDictionaries();
        //InitQTE();
    }

    // Update is called once per frame
    void Update()
    {
        if (actualSequence.Count != 0 && inQTE)
        {
            if (Input.GetKeyDown(inputToKeyCode[actualSequence[0]]))
            {
                Destroy(transform.GetChild(0).gameObject);
                actualSequence.RemoveAt(0);
            }
        }
        else if (inQTE)
        {
            inQTE = false;
            levelGenerator.NextModule();
        }
    }

    public void InitQTE(List<QTESequence.XboxControllerInput> Sequence)
    {
        actualSequence = Sequence;
        //actualSequence = new List<QTESequence.XboxControllerInput>() { QTESequence.XboxControllerInput.A, QTESequence.XboxControllerInput.B, QTESequence.XboxControllerInput.X };
        inQTE = true;
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
