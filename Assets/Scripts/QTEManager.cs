using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.HighDefinition;


public class QTEManager : MonoBehaviour
{
    [SerializeField] LevelGenerator levelGenerator;
    //[SerializeField] GameObject inputImage;
    
    [SerializeField] List<Material> inputIconList;
    [HideInInspector] public Dictionary<QTESequence.XboxControllerInput, Material> inputIconDictionary;
    private Dictionary<QTESequence.XboxControllerInput, KeyCode> inputToKeyCode;

    List<QTESequence.XboxControllerInput> actualSequence;
    List<float> actualleftJoystickPos;
    List<float> actualrightJoystickPos;
    List<Transform> playerPosList;
    bool inQTE = false;
    //Canvas canvas;
    [SerializeField] float playerLerpDuration;

    [SerializeField] GameObject LStickPivot;
    [SerializeField] GameObject RStickPivot;
    [SerializeField] float rotationSpeed = 100f;
    [SerializeField] float angleThreshold;

    float timeToLoose;
    [SerializeField] float joystickTimer;
    bool LjoystickGood;
    bool RjoystickGood;

    [SerializeField] GameObject HUD;
    [SerializeField] GameObject GameOverCanvas;
    [SerializeField] Slider inertie;
    [SerializeField] float inertieGain;

    [Header("Particles")]
    [SerializeField] GameObject puffParticles;
    [SerializeField] GameObject rockParticles;
    void Start()
    {
        actualSequence = new List<QTESequence.XboxControllerInput>();
        timeToLoose = joystickTimer;
        InitDictionaries();
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
                    //Destroy(transform.gameObject);
                    actualSequence.RemoveAt(0);
                    inertie.value += inertieGain;
                    //move player
                    StartCoroutine(PlayerLerp(levelGenerator.player.transform.position,
                        new Vector3(playerPosList[0].position.x, playerPosList[0].position.y -1.5f, levelGenerator.player.transform.position.z), playerLerpDuration));
                    playerPosList.RemoveAt(0);

                    actualleftJoystickPos.RemoveAt(0);
                    actualrightJoystickPos.RemoveAt(0);
                }
                else if (Input.GetKeyDown(KeyCode.JoystickButton0) 
                    || Input.GetKeyDown(KeyCode.JoystickButton1)
                    || Input.GetKeyDown(KeyCode.JoystickButton2)
                    || Input.GetKeyDown(KeyCode.JoystickButton3)
                    || Input.GetKeyDown(KeyCode.JoystickButton4)
                    || Input.GetKeyDown(KeyCode.JoystickButton5))
                {
                    StartCoroutine(GameOver());
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
                StartCoroutine(GameOver());
            }
        }
        
        if(inertie.value >= 100)
        {
            if(Input.GetKey(KeyCode.JoystickButton8) && Input.GetKey(KeyCode.JoystickButton9))
            {
                levelGenerator.progression++;
                levelGenerator.NextModule();
                inertie.value = 0;
            }
        }
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

        //display sequence
        //for (int i = 0; i < actualSequence.Count; i++)
        //{
        //    Sequence.iconSockets[i].GetComponent<DecalProjector>().material = inputIconDictionary[actualSequence[i]];
        //}
    }
    void InitDictionaries()
    {
        inputIconDictionary = new Dictionary<QTESequence.XboxControllerInput, Material>();
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
            // Add other mappings as needed
        };
    }
    IEnumerator PlayerLerp(Vector3 startPosition, Vector3 endPosition,float duration)
    {
        levelGenerator.playerAnim.SetBool("Climbing", true);
        float time = 0;
        while (time < duration)
        {
            float t = time / duration;
            levelGenerator.player.transform.position = Vector3.Lerp(startPosition, endPosition, t);
            time += Time.deltaTime;
            yield return null;
        }

        levelGenerator.player.transform.position = endPosition;
        GameObject particle = Instantiate(puffParticles, levelGenerator.player.transform.position, Quaternion.identity);
        particle.transform.position += Vector3.up*1.5f;
        particle.transform.localScale *= 2;
        levelGenerator.playerAnim.SetBool("Climbing", false);
        yield return null;
    }

    bool gameOver;
    IEnumerator GameOver()
    {
        gameOver = true;
        GameObject particle = Instantiate(rockParticles, levelGenerator.player.transform.position, Quaternion.identity);
        particle.transform.position += Vector3.up; 
        particle.transform.localScale *= 10; 
        StartCoroutine(PlayerLerp(levelGenerator.player.transform.position,
                    levelGenerator.player.transform.position - new Vector3(0, 15f, 0), playerLerpDuration*5));
        yield return new WaitForSeconds(playerLerpDuration * 5);
        
        GameOverCanvas.SetActive(true);
        HUD.SetActive(false);
    }
}
