using UnityEngine;
using Unity.Collections;
using System.Collections.Generic;
using UnityEngine.Pool;
using Unity.Jobs;

public class ProceduralGeneration : MonoBehaviour
{
    public static ProceduralGeneration instance;

    // hierarchy
    public Transform transform_chunks;
    public Material chunkMaterial;
    public int renderDistance;

    public const int CHUNK_SIZE = 40;
    public const int DENSITY = 2;
    public const int CHUNK_VERTECIES = DENSITY*CHUNK_SIZE+1;

    static float seed;


    static Queue<ChunkLoader> chunkLoaders;
    public static ObjectPool<Chunk> pool_chunks;
    public static Dictionary<(int x, int z), Chunk> loadedChunks;
    public static Vector3Int prevPos, currPos;

    struct ChunkLoader : IJob
    {
        public NativeArray<Vector3> vertices;
        public NativeArray<int> triangles;

        public ChunkLoader(Mesh mesh)
        {
            vertices = new NativeArray<Vector3>(mesh.vertices, Unity.Collections.Allocator.TempJob);
            triangles = new NativeArray<int>(mesh.triangles, Unity.Collections.Allocator.TempJob);
        }

        public void Execute()
        {

        }
    }

    public void Init()
    {
        instance = this;

        RandomSeed();


        chunkLoaders = new Queue<ChunkLoader>();

        pool_chunks = new ObjectPool<Chunk>(
            () => {
                // on create
                var go = new GameObject("chunk", typeof(Chunk), typeof(MeshFilter), typeof(MeshCollider), typeof(MeshRenderer));
                go.transform.SetParent(transform_chunks);
                var chunk = go.GetComponent<Chunk>();
                chunk.meshFilter = chunk.gameObject.GetComponent<MeshFilter>();
                chunk.meshCollider = chunk.gameObject.GetComponent<MeshCollider>();
                chunk.meshRenderer = chunk.gameObject.GetComponent<MeshRenderer>();
                chunk.meshRenderer.material = chunkMaterial;
                chunk.meshFilter.mesh = new Mesh();
                chunk.meshFilter.mesh.vertices = new Vector3[(int)Math.Sqr(CHUNK_VERTECIES)];
                chunk.meshFilter.mesh.triangles = new int[(int)Math.Sqr(CHUNK_VERTECIES-1)*2*3]; // 2 triangles per 4 vertices, 3 vertices per triangle

                return chunk;
            },
            (chunk) => {
                // on get
                chunk.gameObject.SetActive(true);
            },
            (chunk) => {
                // on return
                chunk.gameObject.SetActive(false);
            },
            (chunk) => {
                // on destroy
                Destroy(chunk.gameObject);
            },
            false, (int)Math.Sqr(renderDistance*2), (int)Math.Sqr(renderDistance*2)
        );
        loadedChunks = new Dictionary<(int x, int z), Chunk>();
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


    void Load(int chunkX, int chunkZ)
    {
        if(loadedChunks.ContainsKey((chunkX, chunkZ))) return;

        var chunk = pool_chunks.Get();
        chunk.transform.SetPositionAndRotation(new Vector3(chunkX*CHUNK_SIZE, 0, chunkZ*CHUNK_SIZE), Quaternion.identity);
        loadedChunks.Add((chunkX, chunkZ), chunk);

        var meshFilter = chunk.meshFilter;
        var meshCollider = chunk.meshCollider;

        // jobs here ish

        var mesh = meshFilter.mesh;
        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;

        int i=0;
        for(int x=0; x<CHUNK_VERTECIES; ++x) for(int z=0; z<CHUNK_VERTECIES; ++z)
        {
            vertices[CHUNK_VERTECIES*x+z].Set((float)x/(float)DENSITY, Perlin(seed, (float)x/(float)DENSITY, (float)z/(float)DENSITY, chunkX, chunkZ, 0, 0.5f, 0.2f)+Perlin(seed, (float)x/(float)DENSITY, (float)z/(float)DENSITY, chunkX, chunkZ, 0, 20, 0.04f), (float)z/(float)DENSITY);
            if(x<CHUNK_VERTECIES-1 && z<CHUNK_VERTECIES-1)
            {
                triangles[6*i]   = CHUNK_VERTECIES*x+z;
                triangles[6*i+1] = CHUNK_VERTECIES*x+z+1;
                triangles[6*i+2] = CHUNK_VERTECIES*(x+1)+z;
                triangles[6*i+3] = CHUNK_VERTECIES*(x+1)+z;
                triangles[6*i+4] = CHUNK_VERTECIES*x+z+1;
                triangles[6*i+5] = CHUNK_VERTECIES*(x+1)+z+1;
                ++i;
            }
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateBounds();

        meshFilter.mesh = mesh;
        meshCollider.sharedMesh = mesh;
    }

    void Unload(int x, int z)
    {
        if(!loadedChunks.ContainsKey((x, z))) return;

        var chunk = loadedChunks[(x, z)];
        loadedChunks.Remove((x, z));
        pool_chunks.Release(chunk);
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

        if(chunkLoaders.Count > 0)
        {
            var loader = chunkLoaders.Dequeue();
            // stuff
        }
    }
}