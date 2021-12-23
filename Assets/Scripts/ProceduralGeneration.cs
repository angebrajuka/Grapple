using UnityEngine;
using Unity.Collections;
using System.Collections.Generic;
using UnityEngine.Pool;
using System.Threading.Tasks;

struct ChunkLoader
{
    public int chunkX, chunkZ;
    public Mesh mesh;
    public MeshCollider collider;
    public Color[] cols;

    public ChunkLoader(int chunkX, int chunkZ, Mesh mesh, MeshCollider collider, Color[] cols)
    {
        this.chunkX = chunkX;
        this.chunkZ = chunkZ;
        this.mesh = mesh;
        this.collider = collider;
        this.cols = cols;
    }
}

public class ProceduralGeneration : MonoBehaviour
{
    public static ProceduralGeneration instance;

    // hierarchy
    public Transform transform_chunks;
    public Material chunkMaterial;
    public int renderDistance;
    public Texture2D rainTempMap;

    public const int CHUNK_SIZE = 40;
    public const int DENSITY = 1;
    public const int CHUNK_VERTECIES = DENSITY*CHUNK_SIZE+1;
    const int RESOLUTION = 32;
    static int diameter { get { return instance.renderDistance*2+1; } }
    static int maxChunks { get { return (int)Math.Sqr(diameter); } }

    static float seed;

    static Queue<ChunkLoader> chunkLoaders;
    public static ObjectPool<Chunk> pool_chunks;
    public static Dictionary<(int x, int z), Chunk> loadedChunks;
    public static Vector3Int prevPos, currPos;
    static Texture2D groundTexture;
    static int scrollX { get { return currPos.x; } }
    static int scrollZ { get { return currPos.z; } }
    static int rain_temp_map_width;
    static byte[,] rain_temp_map;

    public void Init()
    {
        instance = this;

        RandomSeed();


        groundTexture = new Texture2D(RESOLUTION*diameter, RESOLUTION*diameter);
        chunkMaterial.mainTexture = groundTexture;

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
                var mesh = new Mesh();
                mesh.vertices = new Vector3[(int)Math.Sqr(CHUNK_VERTECIES)];
                mesh.triangles = new int[(int)Math.Sqr(CHUNK_VERTECIES-1)*2*3]; // 2 triangles per 4 vertices, 3 vertices per triangle
                chunk.meshFilter.mesh = mesh;
                chunk.meshRenderer.material = chunkMaterial;

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
            false, maxChunks, maxChunks
        );
        loadedChunks = new Dictionary<(int x, int z), Chunk>();
        prevPos = new Vector3Int(0, 0, 0);
        currPos = new Vector3Int(0, 0, 0);
    }

    public static float RandomSeed()
    {
        seed = Random.value*4586+Random.value;
        return seed;
    }

    static float Perlin(float seed, float x, float z, float chunkX, float chunkZ, float min=0, float max=1, float scale=1)
    {
        const int perlinOffset = 34546; // prevents mirroring
        return Math.Remap(Mathf.PerlinNoise((perlinOffset+seed+x+CHUNK_SIZE*chunkX)*scale, (perlinOffset+seed+z+CHUNK_SIZE*chunkZ)*scale), 0, 1, min, max);
    }

    public static float Height(float x, float z, float chunkX=0, float chunkZ=0)
    {
        return Perlin(seed, x, z, chunkX, chunkZ, 0, 0.5f, 0.2f)
                +Perlin(seed, x, z, chunkX, chunkZ, 0, 20, 0.04f);
    }

    public static byte MapClamped(byte[,] map, int x, int y)
    {
        return map[Mathf.Clamp(x, 0, map.GetLength(0)-1), Mathf.Clamp(y, 0, map.GetLength(1)-1)];
    }

    public static float Biome(float x, float z, float chunkX=0, float chunkZ=0)
    {
        const float rainSeedOffset = 21674253.165235f;
        const float tempSeedOffset = 3567567.6345678f;

        const float perlinScaleRain = 0.003f;
        const float perlinScaleTemp = 0.003f;

        float perlinValRain = Perlin(seed+rainSeedOffset, x, z, chunkX, chunkZ, 0, 1, perlinScaleRain);
        float perlinValTemp = Perlin(seed+tempSeedOffset, x, z, chunkX, chunkZ, 0, 1, perlinScaleTemp);

        float perlinScaleFine = 0.1f;
        float fineNoise = Perlin(seed, x, z, chunkX, chunkZ, 0, 0.05f, perlinScaleFine);

        perlinValTemp -= fineNoise;
        perlinValTemp = Mathf.Round(perlinValTemp * rain_temp_map_width);
        perlinValRain -= fineNoise;
        perlinValRain = Mathf.Round(perlinValRain * rain_temp_map_width);

        return MapClamped(rain_temp_map, (int)perlinValTemp, (int)perlinValRain);
    }

    void SetColor(ref Color c, float r, float g, float b, float a=1)
    {
        c.r = r;
        c.g = g;
        c.b = b;
        c.a = a;
    }

    async void Load(int chunkX, int chunkZ)
    {
        if(loadedChunks.ContainsKey((chunkX, chunkZ))) return;

        var chunk = pool_chunks.Get();
        chunk.transform.SetPositionAndRotation(new Vector3(chunkX*CHUNK_SIZE, 0, chunkZ*CHUNK_SIZE), Quaternion.identity);
        loadedChunks.Add((chunkX, chunkZ), chunk);

        var meshFilter = chunk.meshFilter;
        var meshCollider = chunk.meshCollider;
        var meshRenderer = chunk.meshRenderer;

        var mesh = meshFilter.mesh;
        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;
        Color[] cols = new Color[RESOLUTION*RESOLUTION];

        await Task.Run(() => {
            int i=0;
            for(int x=0; x<CHUNK_VERTECIES; ++x) for(int z=0; z<CHUNK_VERTECIES; ++z)
            {
                vertices[CHUNK_VERTECIES*x+z].Set(
                    (float)x/(float)DENSITY, 
                    Height((float)x/(float)DENSITY, (float)z/(float)DENSITY, chunkX, chunkZ), 
                    (float)z/(float)DENSITY);
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

            for(int x=0; x<RESOLUTION; ++x) for(int z=0; z<RESOLUTION; ++z)
            {
                cols[x*RESOLUTION+z].Set(0, Perlin(seed, x, z, chunkX, chunkZ), 0);
            }
        });

        mesh.vertices = vertices;
        mesh.triangles = triangles;

        chunkLoaders.Enqueue(new ChunkLoader(chunkX, chunkZ, mesh, meshCollider, cols));
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

    void LoadChunksZ(int x)
    {
        for(int z=0; z<=renderDistance; ++z)
        {
            Load(x, currPos.z+z);
        }
        for(int z=-1; z>=-renderDistance; --z)
        {
            Load(x, currPos.z+z);
        }
    }

    void Update()
    {
        Vector3 p = PlayerMovement.m_rigidbody.position;
        currPos.Set((int)Mathf.Floor(p.x/CHUNK_SIZE), 0, (int)Mathf.Floor(p.z/CHUNK_SIZE));

        if(currPos != prevPos || loadedChunks.Count == 0)
        {
            UnloadTooFar();
            for(int x=0; x<=renderDistance; ++x) LoadChunksZ(currPos.x+x);
            for(int x=-1; x>=-renderDistance; --x) LoadChunksZ(currPos.x+x);
        }

        prevPos.Set(currPos.x, 0, currPos.z);
    }

    void FixedUpdate()
    {
        if(chunkLoaders.Count > 0)
        {
            var chunkLoader = chunkLoaders.Dequeue();
            chunkLoader.mesh.RecalculateBounds();
            chunkLoader.collider.sharedMesh = chunkLoader.mesh;
            int beginX = (chunkLoader.chunkX%diameter)*RESOLUTION;
            int beginZ = (chunkLoader.chunkZ%diameter)*RESOLUTION;
            for(int x=0; x<RESOLUTION; ++x) for(int z=0; z<RESOLUTION; ++z)
            {
                groundTexture.SetPixel(beginX+x, beginZ+z, chunkLoader.cols[x*RESOLUTION+z]);
            }
            groundTexture.Apply();
        }
    }
}