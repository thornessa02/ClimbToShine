using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
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
    List<QTESequence.XboxControllerInput>[] QteSequences;

    [Header("Cam")]
    [SerializeField] GameObject cam;
    [SerializeField] float camLerpDuration;
    void Start()
    {
        randomSystem = GetComponent<RandomSystem>();
        if (randomSeed) seed = Random.Range(0,1000);
        randomSystem.Initialize(seed);

        QteSequences = new List<QTESequence.XboxControllerInput>[levelSize];
        for (int i = 0; i < levelSize; i++)
        {
            QteSequences[i] = new List<QTESequence.XboxControllerInput>();
        }
        GenerateWorld();

        //First QTE
        qte.InitQTE(QteSequences[0]);
    }

    void GenerateWorld()
    {
        for (int i = 0; i < levelSize; i++)
        {
            GameObject module = randomSystem.GetRandomElement(moduleList);
            GameObject instantiated = Instantiate(module, Vector3.up * i * moduleSize, Quaternion.identity);
            QteSequences[i] = instantiated.GetComponent<QTESequence>().inputSequence;
        }
    }
    public void NextModule()
    {
        progression++;

        Vector3 endPosition = cam.transform.position + Vector3.up *moduleSize;
        StartCoroutine(CameraLerp(cam.transform.position,endPosition));

        qte.InitQTE(QteSequences[progression]);
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
        yield return null;
    }
}
