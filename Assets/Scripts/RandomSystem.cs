using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSystem : MonoBehaviour
{
    public static System.Random random;

    // Initialize the random generator with a seed
    public void Initialize(int seed)
    {
        random = new System.Random(seed);
    }

    // Get a random integer between min (inclusive) and max (exclusive)
    public int GetRandomInt(int min, int max)
    {
        return random.Next(min, max);
    }

    // Get a random float between 0.0 (inclusive) and 1.0 (exclusive)
    public float GetRandomFloat()
    {
        return (float)random.NextDouble();
    }

    // Get a random float between min (inclusive) and max (exclusive)
    public float GetRandomFloat(float min, float max)
    {
        return (float)(min + (random.NextDouble() * (max - min)));
    }

    // Get a random boolean value
    public bool GetRandomBool()
    {
        return random.Next(0, 2) == 0;
    }

    // Get a random element from a list
    public T GetRandomElement<T>(T[] array)
    {
        int index = random.Next(0, array.Length);
        return array[index];
    }
}
