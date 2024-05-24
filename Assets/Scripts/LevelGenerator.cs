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
    void Start()
    {
        randomSystem = GetComponent<RandomSystem>();
        if (randomSeed) seed = Random.Range(0,1000);
        randomSystem.Initialize(seed);

        GenerateWorld();
    }

    void GenerateWorld()
    {
        for (int i = 0; i < levelSize; i++)
        {
            GameObject module = randomSystem.GetRandomElement(moduleList);
            Instantiate(module, Vector3.up * i * moduleSize, Quaternion.identity);
        }
    }
}
