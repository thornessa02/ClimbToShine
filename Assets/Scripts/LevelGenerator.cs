using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
public class LevelGenerator : MonoBehaviour
{
    public GameObject player;

    [Header("Seeds")]
    bool randomSeed;
    int seed;
    RandomSystem randomSystem;

    [Header("Modules")]
    [SerializeField] GameObject[] moduleList;
    [SerializeField] GameObject finishModule;
    [SerializeField] float moduleSize;
    [SerializeField] int levelSize;
     [HideInInspector]public int progression = 0;

    [Header("QTE")]
    [SerializeField] QTEManager qteManager;
    public struct QTEmodule
    {
        public List<QTESequence.XboxControllerInput> sequence;
        public List<float> leftJoystickPos;
        public List<float> rightJoystickPos;

        public List<Transform> iconSockets;
        public List<Transform> playerSockets;
    }
    List<QTEmodule> QTEList = new List<QTEmodule>();

    [Header("Cam")]
    [SerializeField] GameObject cam;
    Vector3 camStartPos;
    [SerializeField] float camLerpDuration;
    [SerializeField] float playerLerpDuration;
    [SerializeField] GameObject winScreen;
    [SerializeField] GameObject highScore;
    [SerializeField] TMPro.TMP_Text finalTime;
    [SerializeField] GameObject hud;

    void Start()
    {
        //seed
        if (PlayerPrefs.HasKey("UseSeed"))
        {
            if (PlayerPrefs.GetInt("UseSeed") == 1) randomSeed = false;
            else randomSeed = true;
        }

        if (PlayerPrefs.HasKey("SeedNb"))
        {
            int number;
            bool success = int.TryParse(PlayerPrefs.GetString("SeedNb"), out number);

            if (success)
            {
                seed = number;
            }
            else
            {
                randomSeed = true;
            }
        }

        camStartPos = cam.transform.position;
        randomSystem = GetComponent<RandomSystem>();
        if (randomSeed) seed = Random.Range(0,1000);
        randomSystem.Initialize(seed);
        GenerateWorld();

        //First QTE
        qteManager.InitQTE(QTEList[0]);
    }

    void GenerateWorld()
    {
        for (int i = 0; i < levelSize; i++)
        {
            GameObject module = randomSystem.GetRandomElement(moduleList);
            GameObject instantiated = Instantiate(module, Vector3.up * i * moduleSize, Quaternion.identity);

            QTEmodule qte = new QTEmodule();
            qte.sequence = instantiated.GetComponent<QTESequence>().inputSequence;
            qte.leftJoystickPos = instantiated.GetComponent<QTESequence>().leftJoystickPos;
            qte.rightJoystickPos = instantiated.GetComponent<QTESequence>().rightJoystickPos;
            qte.iconSockets = instantiated.GetComponent<QTESequence>().iconSockets;
            qte.playerSockets = instantiated.GetComponent<QTESequence>().playerSockets;

            QTEList.Add(qte);

            //display sequence
            for (int j = 0; j < qte.sequence.Count; j++)
            {
                instantiated.GetComponent<QTESequence>().iconSockets[j].GetComponent<DecalProjector>().material = qteManager.inputIconDictionary[qte.sequence[j]];
            }
        }
        
        finishModule = Instantiate(finishModule, new Vector3(2, 1 * levelSize * moduleSize, -5), Quaternion.identity);
    }
    [SerializeField] AudioSource click;
    public void NextModule()
    {
        progression++;
        click.Play();
        Vector3 endPosition = camStartPos + Vector3.up *moduleSize*progression;
        StartCoroutine(CameraLerp(cam.transform.position,endPosition));

    }

    public Animator playerAnim;
    IEnumerator CameraLerp(Vector3 startPosition, Vector3 endPosition)
    {
        float time = 0;
        while (time < camLerpDuration)
        {
            float t = time / camLerpDuration;
            cam.transform.position = Vector3.Lerp(startPosition, endPosition, t);
            time += Time.deltaTime;
            yield return null;
        }

        cam.transform.position = endPosition;
        if (progression == levelSize)
        {
            GetComponent<Timer>().StopTimer();
            hud.SetActive(false);
            finalTime.text = GetComponent<Timer>().FormatTime(GetComponent<Timer>().elapsedTime);

            if (GetComponent<Timer>().elapsedTime <= GetComponent<LeaderboardManager>().GetTopTimes(1)[0]) highScore.SetActive(true);

            StartCoroutine(PlayerLerp(player.transform.position, finishModule.GetComponent<QTESequence>().playerSockets[0].position));
            yield return new WaitForSeconds(playerLerpDuration);
            playerAnim.SetTrigger("Walk");
            StartCoroutine(PlayerLerp(player.transform.position, finishModule.GetComponent<QTESequence>().playerSockets[1].position));
            yield return new WaitForSeconds(playerLerpDuration);
            winScreen.SetActive(true);
        }
        else
            qteManager.InitQTE(QTEList[progression]);
        yield return null;
    }
    IEnumerator PlayerLerp(Vector3 startPosition, Vector3 endPosition)
    {
        float time = 0;
        while (time < playerLerpDuration)
        {
            float t = time / playerLerpDuration;
            player.transform.position = Vector3.Lerp(startPosition, endPosition, t);
            time += Time.deltaTime;
            yield return null;
        }

        player.transform.position = endPosition;
        yield return null;
    }
}
