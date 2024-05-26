using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public GameObject player;

    [Header("Seeds")]
    [SerializeField] bool randomSeed;
    [SerializeField] int seed;
    RandomSystem randomSystem;

    [Header("Modules")]
    [SerializeField] GameObject[] moduleList;
    [SerializeField] GameObject finishModule;
    [SerializeField] float moduleSize;
    [SerializeField] int levelSize;
     [HideInInspector]public int progression = 0;

    [Header("QTE")]
    [SerializeField] QTEManager qte;
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


    void Start()
    {
        camStartPos = cam.transform.position;
        randomSystem = GetComponent<RandomSystem>();
        if (randomSeed) seed = Random.Range(0,1000);
        randomSystem.Initialize(seed);
        GenerateWorld();

        //First QTE
        qte.InitQTE(QTEList[0]);
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
        }

        Instantiate(finishModule, Vector3.up * levelSize * moduleSize, Quaternion.identity);
    }
    public void NextModule()
    {
        progression++;

        Vector3 endPosition = camStartPos + Vector3.up *moduleSize*progression;
        StartCoroutine(CameraLerp(cam.transform.position,endPosition));

        //qte.InitQTE(QTEList[progression]);
    }

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
        qte.InitQTE(QTEList[progression]);
        yield return null;
    }
}
