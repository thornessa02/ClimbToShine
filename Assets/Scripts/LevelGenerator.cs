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
    [SerializeField] float moduleSize;
    [SerializeField] int levelSize;
    int progression = 0;

    [Header("QTE")]
    [SerializeField] QTEManager qte;
    public struct QTEmodule
    {
        public List<QTESequence.XboxControllerInput> sequence;
        public List<Transform> iconSockets;
        public List<Transform> playerSockets;
    }
    List<QTEmodule> QTEList = new List<QTEmodule>();

    [Header("Cam")]
    [SerializeField] GameObject cam;
    [SerializeField] float camLerpDuration;


    void Start()
    {
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
            qte.iconSockets = instantiated.GetComponent<QTESequence>().iconSockets;
            qte.playerSockets = instantiated.GetComponent<QTESequence>().playerSockets;

            QTEList.Add(qte);
        }
    }
    public void NextModule()
    {
        progression++;

        Vector3 endPosition = cam.transform.position + Vector3.up *moduleSize;
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
