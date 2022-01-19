using UnityEngine;
using Unity.Collections;
using System.Collections.Generic;
using UnityEngine.Pool;
using System.Threading.Tasks;
using System;

public class ProceduralGeneration : MonoBehaviour
{
    public static ProceduralGeneration instance;

    // hierarchy
    public Transform transform_chunks;
    public Transform transform_decor;
    public Material chunkMaterial;
    public int renderDistance;
    public float chunkSize;
    public int chunkWidthVertices;
    public float offset;
    public byte chunksPerFrame;
    public Texture2D rainTempMap;
    public BiomeData[] biomesData;

    const int RESOLUTION = 64;
    float vertexSpacing { get { return (float)chunkSize/(float)(chunkWidthVertices-1); } }
    static int diameter { get { return instance.renderDistance*2+1; } }
    static int maxChunks { get { return (int)Math.Sqr(diameter); } }

    const int MAX_DECORS = 20;

    public static float seed;

    static LinkedList<Chunk> loadingChunks;
    public static ObjectPool<Chunk> pool_chunks;
    public static ObjectPool<GameObject>[] pool_decor;
    public static Dictionary<(int x, int z), Chunk> loadedChunks;
    public static Vector3Int prevPos, currPos;
    static int scrollX { get { return currPos.x; } }
    static int scrollZ { get { return currPos.z; } }
    static byte[,] rain_temp_map;
    static int rain_temp_map_width;
    static Biome[] biomes;

    public void Init()
    {
        instance = this;

        rain_temp_map_width = rainTempMap.width;
        rain_temp_map = new byte[rain_temp_map_width,rain_temp_map_width];
        biomes = new Biome[biomesData.Length];

        for(int i=0; i<biomes.Length; i++)
        {
            biomes[i] = new Biome(biomesData[i]);
            if(ColorUtility.TryParseHtmlString(biomesData[i].rain_temp_map_color, out Color color))
            {
                var color32 = (Color32)color;
                for(int x=0; x<rain_temp_map_width; x++)
                {
                    for(int y=0; y<rain_temp_map_width; y++)
                    {
                        Color32 c = rainTempMap.GetPixel(x, y);

                        if(c.r == color32.r && c.g == color32.g && c.b == color32.b)
                        {
                            rain_temp_map[x,y] = (byte)i;
                        }
                    }
                }
            }
        }

        pool_decor = new ObjectPool<GameObject>[Biome.s_decorations.Length];
        foreach(var pair in Biome.s_indexes)
        {
            pool_decor[pair.Value] = new ObjectPool<GameObject>(
                () => {
                    // on create
                    var decor = Instantiate(Biome.s_decorations[pair.Value], transform_decor);

                    return decor;
                },
                (decor) => {
                    // on get
                    decor.SetActive(true);
                },
                (decor) => {
                    // on return
                    decor.SetActive(false);
                },
                (decor) => {
                    // on destroy
                    Destroy(decor);
                },
                false, 100, 1000
            );
        }

        loadingChunks = new LinkedList<Chunk>();

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
                mesh.vertices = new Vector3[(int)Math.Sqr(chunkWidthVertices)];
                mesh.colors = new Color[mesh.vertices.Length];
                mesh.triangles = new int[(int)Math.Sqr(chunkWidthVertices-1)*2*3]; // 2 triangles per 4 vertices, 3 vertices per triangle
                chunk.meshFilter.mesh = mesh;
                chunk.meshCollider.convex = false;
                chunk.meshRenderer.material = chunkMaterial;
                chunk.decorPositions = new Vector3[MAX_DECORS];
                chunk.decors = new int[MAX_DECORS];
                chunk.decorRefs = new GameObject[MAX_DECORS];

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
        seed = (float)UnityEngine.Random.value*(float)4586+UnityEngine.Random.value;
        return seed;
    }

    static float Perlin(float seed, float x, float z, float chunkX, float chunkZ, float min=0, float max=1, float scale=1)
    {
        const int perlinOffset = 34546; // prevents mirroring
        return Math.Remap(Mathf.PerlinNoise((perlinOffset+seed+x+instance.chunkSize*chunkX)*scale, (perlinOffset+seed+z+instance.chunkSize*chunkZ)*scale), 0, 1, min, max);
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

    public static int PerlinBiome(float x, float z, float chunkX=0, float chunkZ=0)
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
        loadedChunks.Add((chunkX, chunkZ), chunk);
        chunk.transform.SetPositionAndRotation(new Vector3(chunkX*chunkSize, 0, chunkZ*chunkSize), Quaternion.identity);

        var meshFilter = chunk.meshFilter;
        var meshCollider = chunk.meshCollider;
        var meshRenderer = chunk.meshRenderer;

        var mesh = meshFilter.mesh;
        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;
        Color[] cols = mesh.colors;
        Vector3[] decorPositions = chunk.decorPositions;
        int[] decors = chunk.decors;
        int numOfDecors = 0;

        await Task.Run(() => {
            int i=0;
            for(int x=0; x<chunkWidthVertices; ++x) for(int z=0; z<chunkWidthVertices; ++z)
            {
                vertices[chunkWidthVertices*x+z].Set(
                    (float)x*vertexSpacing+Perlin(154.2643f, x*vertexSpacing, z*vertexSpacing, chunkX, chunkZ, -offset, offset), 
                    Height((float)x*vertexSpacing, (float)z*vertexSpacing, chunkX, chunkZ), 
                    (float)z*vertexSpacing+Perlin(56743.2534525f, x*vertexSpacing, z*vertexSpacing, chunkX, chunkZ, -offset, offset));

                cols[chunkWidthVertices*x+z] = biomes[PerlinBiome(x*vertexSpacing, z*vertexSpacing, chunkX, chunkZ)].color;

                if(x<chunkWidthVertices-1 && z<chunkWidthVertices-1)
                {
                    triangles[6*i+0] = chunkWidthVertices*x+z;
                    triangles[6*i+1] = chunkWidthVertices*x+z+1;
                    triangles[6*i+2] = chunkWidthVertices*(x+1)+z;
                    triangles[6*i+3] = chunkWidthVertices*(x+1)+z;
                    triangles[6*i+4] = chunkWidthVertices*x+z+1;
                    triangles[6*i+5] = chunkWidthVertices*(x+1)+z+1;
                    ++i;
                }
            }
        });

        numOfDecors = 1;
        decors[0] = 0;
        decorPositions[0].Set(chunkX*chunkSize, Height(0, 0, chunkX, chunkZ), chunkZ*chunkSize);

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.colors = cols;

        chunk.decorPositions = decorPositions;
        chunk.decors = decors;
        chunk.numOfDecors = numOfDecors;

        float dist = Math.Dist(currPos.x, currPos.z, chunkX, chunkZ);

        loadingChunks.AddLast(chunk);
    }

    static void Unload(int x, int z)
    {
        if(!loadedChunks.ContainsKey((x, z))) return;

        var chunk = loadedChunks[(x, z)];
        loadedChunks.Remove((x, z));
        for(int i=0; i<chunk.numOfDecors; i++)
        {
            if(chunk.decorRefs[i] != null) pool_decor[chunk.decors[i]].Release(chunk.decorRefs[i]);
        }
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

    void AlternatingLoop(Action<int> func, int max)
    {
        for(int i=0; i<max; i++)
        {
            func(i);
            if(i != 0) func(-i);
        }
    }

    public static void UnloadAll()
    {
        loadingChunks.Clear();
        var toUnload = new (int x, int z)[loadedChunks.Count];
        int i=0;
        foreach(var pair in loadedChunks)
        {
            toUnload[i] = pair.Key;
            i++;
        }
        foreach(var key in toUnload)
        {
            Unload(key.x, key.z);
        }
    }

    void Update()
    {
        Vector3 p = PlayerMovement.rb.position;
        currPos.Set((int)Mathf.Floor(p.x/chunkSize), 0, (int)Mathf.Floor(p.z/chunkSize));

        if(currPos != prevPos || loadedChunks.Count == 0)
        {
            UnloadTooFar();
            AlternatingLoop((x) => {
                AlternatingLoop((z) => {
                    Load(currPos.x+x, currPos.z+z);
                }, renderDistance);
            }, renderDistance);
        }

        prevPos.Set(currPos.x, 0, currPos.z);
    }

    void FixedUpdate()
    {
        var offset = new Vector3(chunkSize/2, 0, chunkSize/2);
        for(int i=0; loadingChunks.Count > 0 && i < chunksPerFrame; i++)
        {
            var closest = loadingChunks.First;
            var closestDist = Mathf.Infinity;
            for(var node = loadingChunks.First; node != null; node = node.Next)
            {
                var dist = Vector3.Distance(PlayerMovement.rb.position, node.Value.transform.position+offset);
                if(dist < closestDist)
                {
                    closestDist = dist;
                    closest = node;
                }
            }

            var chunk = closest.Value;
            loadingChunks.Remove(closest);

            chunk.meshFilter.mesh.RecalculateBounds();
            chunk.meshFilter.mesh.RecalculateNormals();
            chunk.meshCollider.sharedMesh = chunk.meshFilter.mesh;

            for(int di=0; di<chunk.numOfDecors; di++)
            {
                var decor = pool_decor[chunk.decors[di]].Get();
                decor.transform.position = chunk.decorPositions[di];
                chunk.decorRefs[di] = decor;
            }
        }
    }
}