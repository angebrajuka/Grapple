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
    [Tooltip("time until next spawn, also initial delay before enemies start spawning")]
    public float timer;

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
    }

    Vector3 GetPosition()
    {
        var pos2D = Random.insideUnitCircle;
        if(pos2D == Vector2.zero) pos2D = Vector2.right;

        var min = pos2D.normalized*minDistance;
        var max = pos2D.normalized*maxDistance;
        pos2D = (max-min)*pos2D.magnitude+min;

        pos2D.x += PlayerMovement.m_rigidbody.position.x;
        pos2D.y += PlayerMovement.m_rigidbody.position.z;

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
        timer -= Time.deltaTime;
        if(timer <= 0)
        {
            timer = Random.Range(minDelay, maxDelay);
            Spawn(Random.Range(0, frequencies[frequencies.Length-1])/*, Vector3.up*20*/);
        }
    }
}