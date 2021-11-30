using UnityEngine;
using System.Collections.Generic;

public class ProceduralGeneration : MonoBehaviour
{
    public static ProceduralGeneration instance;

    // hierarchy
    public GameObject prefab_chunk;
    public Transform transform_chunks;
    public int renderDistance;

    public const int CHUNK_SIZE = 40;

    static float seed;


    public static ObjectPool pool_chunks;
    public static Dictionary<(int x, int z), GameObject> loadedChunks;
    public static Vector3Int prevPos, currPos;

    public void Init()
    {
        instance = this;

        RandomSeed();


        pool_chunks = new ObjectPool(prefab_chunk, transform_chunks);
        loadedChunks = new Dictionary<(int x, int z), GameObject>();
        prevPos = new Vector3Int(0, 0, 0);
        currPos = new Vector3Int(0, 0, 0);
    }

    public static float RandomSeed()
    {
        seed = Random.value*4586+Random.value;
        SetRelativeSeeds();
        return seed;
    }

    public static void SetRelativeSeeds()
    {

    }

    static float Perlin(float seed, float x, float z, float chunkX, float chunkZ, float min=0, float max=1, float scale=1)
    {
        const int perlinOffset = 34546; // prevents mirroring
        return Math.Remap(Mathf.PerlinNoise((perlinOffset+seed+x+CHUNK_SIZE*chunkX)*scale, (perlinOffset+seed+z+CHUNK_SIZE*chunkZ)*scale), 0, 1, min, max);
    }

    public static void LoadChunk(int chunkX, int chunkZ, GameObject chunk)
    {
        const int density = 2;
        const int chunkSize = CHUNK_SIZE*density;

        var meshFilter = chunk.GetComponent<MeshFilter>();
        var meshCollider = chunk.GetComponent<MeshCollider>();

        var mesh = meshFilter.mesh == null ? new Mesh() : meshFilter.mesh;
        Vector3[] vertices = new Vector3[(int)Math.Sqr(chunkSize+1)];
        int[] triangles = new int[(int)Math.Sqr(chunkSize)*6];

        int i=0;
        for(int x=0; x<=chunkSize; ++x) for(int z=0; z<=chunkSize; ++z)
        {
            vertices[(chunkSize+1)*x+z].Set((float)x/(float)density, Perlin(seed, (float)x/(float)density, (float)z/(float)density, chunkX, chunkZ, 0, 0.5f, 0.2f)+Perlin(seed, (float)x/(float)density, (float)z/(float)density, chunkX, chunkZ, 0, 20, 0.04f), (float)z/(float)density);
            if(x<chunkSize && z<chunkSize)
            {
                triangles[6*i]   = (chunkSize+1)*x+z;
                triangles[6*i+1] = (chunkSize+1)*x+z+1;
                triangles[6*i+2] = (chunkSize+1)*(x+1)+z;
                triangles[6*i+3] = (chunkSize+1)*(x+1)+z;
                triangles[6*i+4] = (chunkSize+1)*x+z+1;
                triangles[6*i+5] = (chunkSize+1)*(x+1)+z+1;
                ++i;
            }
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;

        meshFilter.mesh = mesh;
        meshCollider.sharedMesh = mesh;
    }



    void Load(int x, int z)
    {
        if(loadedChunks.ContainsKey((x, z))) return;

        var chunk = pool_chunks.Get(new Vector3(x*CHUNK_SIZE, 0, z*CHUNK_SIZE), Quaternion.identity);
        loadedChunks.Add((x, z), chunk);
        chunk.SetActive(true);

        LoadChunk(x, z, chunk);
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