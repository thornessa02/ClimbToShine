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
    List<Transform> playerPosList;
    bool inQTE = false;
    Canvas canvas;
    [SerializeField] float playerLerpDuration;
    void Start()
    {
        actualSequence = new List<QTESequence.XboxControllerInput>();
        canvas = transform.parent.GetComponent<Canvas>();
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

                //move player
                StartCoroutine(PlayerLerp(levelGenerator.player.transform.position,playerPosList[0].position));
                playerPosList.RemoveAt(0);
            }
        }
        else if (inQTE)
        {
            inQTE = false;
            levelGenerator.NextModule();
        }
    }

    public void InitQTE(LevelGenerator.QTEmodule Sequence)
    {
        actualSequence = Sequence.sequence;
        playerPosList = Sequence.sockets;
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
        for (int i = 0; i < actualSequence.Count; i++)
        {
            GameObject image = Instantiate(inputImage, transform);
            image.GetComponent<Image>().sprite = inputIconDictionary[actualSequence[i]];

            Vector3 screenPos = Camera.main.WorldToScreenPoint(Sequence.sockets[i].position);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform, screenPos, canvas.worldCamera, out Vector2 canvasPos);

            image.transform.localPosition = canvasPos;
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
    IEnumerator PlayerLerp(Vector3 startPosition, Vector3 endPosition)
    {
        float time = 0;
        while (time < playerLerpDuration)
        {
            float t = time / playerLerpDuration;
            levelGenerator.player.transform.position = Vector3.Lerp(startPosition, endPosition, t);
            time += Time.deltaTime;
            yield return null;
        }

        levelGenerator.player.transform.position = endPosition;
        yield return null;
    }
}
