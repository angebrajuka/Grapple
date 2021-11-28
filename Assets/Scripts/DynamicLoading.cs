using UnityEngine;
using System.Collections.Generic;

public class DynamicLoading : MonoBehaviour
{
    public static DynamicLoading instance;

    // hierarchy
    public GameObject prefab_chunk;
    public Transform transform_chunks;
    public int renderDistance;

    public static Queue<GameObject> unloadedChunks;
    public static Dictionary<(int x, int z), GameObject> loadedChunks;
    public const int CHUNK_SIZE = 40;
    public static Vector3Int prevPos, currPos;

    public void Init()
    {
        instance = this;

        unloadedChunks = new Queue<GameObject>();
        loadedChunks = new Dictionary<(int x, int z), GameObject>();
        prevPos = new Vector3Int(0, 0, 0);
        currPos = new Vector3Int(0, 0, 0);
    }

    public void Load(int x, int z)
    {
        if(loadedChunks.ContainsKey((x, z))) return;

        GameObject chunk = unloadedChunks.Count == 0 ? Instantiate(prefab_chunk, transform_chunks) : unloadedChunks.Dequeue();
        loadedChunks.Add((x, z), chunk);
        chunk.SetActive(true);

        Vector3 pos = chunk.transform.position;
        pos.x = x*CHUNK_SIZE;
        pos.z = z*CHUNK_SIZE;
        chunk.transform.position = pos;

        ProceduralGeneration.LoadChunk(x, z, chunk);
    }

    public void Unload(int x, int z)
    {
        if(!loadedChunks.ContainsKey((x, z))) return;

        GameObject chunk = loadedChunks[(x, z)];
        unloadedChunks.Enqueue(chunk);
        chunk.gameObject.SetActive(false);
        loadedChunks.Remove((x, z));
    }

    public void UnloadTooFar()
    {
        var toUnload = new LinkedList<(int x, int z)>();
        foreach(var chunk in loadedChunks)
        {
            if(Mathf.Abs(currPos.x - chunk.Key.x) > renderDistance+1 || Mathf.Abs(currPos.z - chunk.Key.z) > renderDistance+1)
            {
                toUnload.AddLast((chunk.Key.x, chunk.Key.z));
            }
        }
        foreach(var pos in toUnload)
        {
            Unload(pos.x, pos.z);
        }
    }

    void Update()
    {
        Vector3 p = PlayerMovement.m_rigidbody.position;
        currPos.Set((int)Mathf.Floor(p.x/CHUNK_SIZE), 0, (int)Mathf.Floor(p.z/CHUNK_SIZE));

        if(currPos != prevPos || loadedChunks.Count == 0)
        {
            UnloadTooFar();
            for(int x=-renderDistance; x<=renderDistance; ++x) for(int z=-renderDistance; z<=renderDistance; ++z)
            {
                Load(currPos.x+x, currPos.z+z);
            }
        }

        prevPos.Set(currPos.x, 0, currPos.z);
    }
}