using UnityEngine;
using System.Collections.Generic;

public class DynamicLoading : MonoBehaviour
{
    public static DynamicLoading instance;

    // hierarchy
    public GameObject prefab_chunk;
    public Transform transform_chunks;
    public int renderDistance;

    public static ObjectPool pool_chunks;
    public static Dictionary<(int x, int z), GameObject> loadedChunks;
    public static Vector3Int prevPos, currPos;

    public void Init()
    {
        instance = this;

        pool_chunks = new ObjectPool(prefab_chunk, transform_chunks);
        loadedChunks = new Dictionary<(int x, int z), GameObject>();
        prevPos = new Vector3Int(0, 0, 0);
        currPos = new Vector3Int(0, 0, 0);
    }

    void Load(int x, int z)
    {
        if(loadedChunks.ContainsKey((x, z))) return;

        var chunk = pool_chunks.Get(new Vector3(x*ProceduralGeneration.CHUNK_SIZE, 0, z*ProceduralGeneration.CHUNK_SIZE), Quaternion.identity);
        loadedChunks.Add((x, z), chunk);
        chunk.SetActive(true);

        ProceduralGeneration.LoadChunk(x, z, chunk);
    }

    void Unload(int x, int z)
    {
        if(!loadedChunks.ContainsKey((x, z))) return;

        GameObject chunk = loadedChunks[(x, z)];
        loadedChunks.Remove((x, z));
        pool_chunks.Return(chunk);
    }

    void UnloadTooFar()
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
        currPos.Set((int)Mathf.Floor(p.x/ProceduralGeneration.CHUNK_SIZE), 0, (int)Mathf.Floor(p.z/ProceduralGeneration.CHUNK_SIZE));

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