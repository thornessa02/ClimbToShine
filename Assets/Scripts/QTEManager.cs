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
    List<float> actualleftJoystickPos;
    List<float> actualrightJoystickPos;
    List<Transform> playerPosList;
    bool inQTE = false;
    Canvas canvas;
    [SerializeField] float playerLerpDuration;

    [SerializeField] GameObject LStickPivot;
    [SerializeField] GameObject RStickPivot;
    [SerializeField] float rotationSpeed = 100f;
    [SerializeField] float angleThreshold;

    float timeToLoose;
    [SerializeField] float joystickTimer;
    bool LjoystickGood;
    bool RjoystickGood;
    void Start()
    {
        actualSequence = new List<QTESequence.XboxControllerInput>();
        canvas = transform.parent.GetComponent<Canvas>();
        timeToLoose = joystickTimer;
        InitDictionaries();
        //InitQTE();
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameOver && inQTE)
        {
            if (actualSequence.Count != 0)
            {
                if (Input.GetKeyDown(inputToKeyCode[actualSequence[0]]))
                {
                    Destroy(transform.GetChild(0).gameObject);
                    actualSequence.RemoveAt(0);

                    //move player
                    StartCoroutine(PlayerLerp(levelGenerator.player.transform.position,
                        playerPosList[0].position - new Vector3(0, 1.5f, 0)));
                    playerPosList.RemoveAt(0);

                    actualleftJoystickPos.RemoveAt(0);
                    actualrightJoystickPos.RemoveAt(0);
                }
                else if (Input.anyKeyDown)
                {
                    GameOver();
                }
            }
            else
            {
                inQTE = false;
                LStickPivot.SetActive(false);
                RStickPivot.SetActive(false);
                levelGenerator.NextModule();
            }

        }

        if (!gameOver)
        {
            //Left joystick
            if (actualleftJoystickPos[0] < 0)
            {
                LStickPivot.SetActive(false);
            }
            else
            {
                LStickPivot.SetActive(true);
                moveJoystick(actualleftJoystickPos[0], LStickPivot);

            }
            //check Ljoystick
            float LhorizontalInput = Input.GetAxis("Horizontal");
            float LverticalInput = Input.GetAxis("Vertical");

            LjoystickGood = checkJoystickPosition(actualleftJoystickPos[0], LhorizontalInput, LverticalInput);

            //Right joystick
            if (actualrightJoystickPos[0] < 0)
            {
                RStickPivot.SetActive(false);
            }
            else
            {
                RStickPivot.SetActive(true);
                moveJoystick(actualrightJoystickPos[0], RStickPivot);

            }

            //check Rjoystick
            float RhorizontalInput = Input.GetAxis("RHorizontal");
            float RverticalInput = Input.GetAxis("RVertical");

            RjoystickGood = checkJoystickPosition(actualrightJoystickPos[0], RhorizontalInput, RverticalInput);

            if(RjoystickGood && LjoystickGood)
            {
                timeToLoose = joystickTimer;
            }
            else
            {
                timeToLoose -= Time.deltaTime;
            }

            if (timeToLoose <= 0)
            {
                GameOver();
            }
        }
        //print(timeToLoose);
    }

    void moveJoystick(float angleNeeded, GameObject joystick)
    {
        Quaternion targetRotation = Quaternion.Euler(0, 0, angleNeeded);
        joystick.transform.rotation = Quaternion.RotateTowards(joystick.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
    bool checkJoystickPosition(float angleNeeded, float horizontalInput, float verticalInput)
    {
        float joystickangle = Mathf.Atan2(verticalInput, horizontalInput) * Mathf.Rad2Deg;
        joystickangle -= 90;
        if (angleNeeded >= 0)
        {
            if (Mathf.Abs(Mathf.DeltaAngle(joystickangle, angleNeeded)) <= angleThreshold && new Vector2(horizontalInput, verticalInput).sqrMagnitude > 0.01f)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            if (new Vector2(horizontalInput, verticalInput).sqrMagnitude < 0.01f)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
    public void InitQTE(LevelGenerator.QTEmodule Sequence)
    {
        playerPosList = Sequence.playerSockets;
        actualleftJoystickPos = Sequence.leftJoystickPos;
        actualrightJoystickPos = Sequence.rightJoystickPos;
        actualSequence = Sequence.sequence;
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

            Vector3 screenPos = Camera.main.WorldToScreenPoint(Sequence.iconSockets[i].position);
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

    bool gameOver;
    void GameOver()
    {
        gameOver = true;
        StartCoroutine(PlayerLerp(levelGenerator.player.transform.position,
                    playerPosList[0].position - new Vector3(0, 15f, 0)));
    }
}
