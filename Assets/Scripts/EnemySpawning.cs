using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class EnemyFrequency
{
    public float frequency;
    public GameObject prefab;
}

public class EnemySpawning : MonoBehaviour
{
    public static EnemySpawning instance;

    // hierarchy
    public EnemyFrequency[] naturalEnemies;
    float[] frequencies;
    GameObject[] prefabs_natural;
    public GameObject[] prefabs_bosses;
    public float minDistance, maxDistance;
    public float minDelay, maxDelay;
    public float initialDelay;

    public static float lastSpawnTime;
    public static float delayTime;
    public static int difficulty;

    public void Init()
    {
        instance = this;
        frequencies = new float[naturalEnemies.Length];
        frequencies[0] = naturalEnemies[0].frequency;
        prefabs_natural = new GameObject[naturalEnemies.Length];
        for(int i=0; i<frequencies.Length; i++)
        {
            prefabs_natural[i] = naturalEnemies[i].prefab;
            if(i == 0) continue;
            frequencies[i] += frequencies[i-1];
        }

        Reset();
    }

    public void Reset()
    {
        for(int i=0; i<transform.childCount; i++)
        {
            Destroy(transform.GetChild(i));
        }
        lastSpawnTime = Time.time;
        delayTime = maxDelay;
    }

    Vector3 GetPosition()
    {
        var pos2D = Random.insideUnitCircle;
        if(pos2D == Vector2.zero) pos2D = Vector2.right;

        var min = pos2D.normalized*minDistance;
        var max = pos2D.normalized*maxDistance;
        pos2D = (max-min)*pos2D.magnitude+min;

        pos2D.x += PlayerMovement.rb.position.x;
        pos2D.y += PlayerMovement.rb.position.z;

        return new Vector3(pos2D.x, ProceduralGeneration.Height(pos2D.x, pos2D.y)+5, pos2D.y);
    }

    public void Spawn(float frequency, Vector3 position=default(Vector3))
    {
        if(position == default(Vector3))
        {
            position = GetPosition();
        }

        int index = System.Array.BinarySearch(frequencies, frequency);

        if(index < 0) index = ~index;
        if(index >= frequencies.Length) index = frequencies.Length-1;

        Instantiate(prefabs_natural[index], position, Quaternion.identity, transform);
    }

    void Update()
    {
        
        if(Time.time >= lastSpawnTime+delayTime)
        {
            delayTime = Random.Range(minDelay, maxDelay);
            lastSpawnTime = Time.time;
            Spawn(Random.Range(0, frequencies[frequencies.Length-1])/*, Vector3.up*20*/);
        }
    }
}