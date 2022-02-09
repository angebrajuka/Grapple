using UnityEngine;
using Unity.Collections;
using System.Collections.Generic;
using UnityEngine.Pool;
using System.Threading.Tasks;
using System;
using System.Linq;

using static MarchingCubes;

public class ProceduralGeneration : MonoBehaviour
{
    public static ProceduralGeneration instance;

    // hierarchy
    public GameObject prefab_chunk;
    public Transform transform_chunks;
    public float chunkSize;
    public int chunkWidthCubes;
    public int chunkHeightCubes;
    public float groundThreshhold;
    public float groundScale;
    public float waveScale;
    public float waveAmount;
    public float offset;
    public byte chunksPerFrame;
    public Texture2D rainTempMap;
    public BiomeData[] biomesData;

    const int RESOLUTION = 64;
    static float cubeWidth { get { return instance.chunkSize/instance.chunkWidthCubes; } }
    static float chunkHeight { get { return cubeWidth * instance.chunkHeightCubes; } }
    // float vertexSpacing { get { return (float)chunkSize/(float)(chunkWidthVertices-1); } }

    static int diameter { get { return instance.renderDistance*2+1; } }
    static int maxChunks { get { return (int)Math.Sqr(diameter); } }
    private int renderDistance;
    public int RenderDistance {
        set {
            renderDistance = value;
            
        }
        get { return renderDistance; }
    }

    const int MAX_DECORS = 20;

    public static long seed;
    public static ushort seed_ll { get { return (ushort)((seed >> 48) & 0xFFFF); } }
    public static ushort seed_lm { get { return (ushort)((seed >> 32) & 0xFFFF); } }
    public static ushort seed_rm { get { return (ushort)((seed >> 16) & 0xFFFF); } }
    public static ushort seed_rr { get { return (ushort)((seed >>  0) & 0xFFFF); } } // TODO double check
    public static float seed_temp { get { return seed_ll+((float)seed_ll/100000.0f); } }
    public static float seed_rain { get { return seed_lm+((float)seed_lm/100000.0f); } }
    public static float seed_grnd { get { return seed_rm+((float)seed_rm/100000.0f); } }
    public static float seed_wave { get { return seed_rr+((float)seed_rr/100000.0f); } }
    public static float seed_fine { get { return (seed_rain+seed_temp)/2; } }

    static Dictionary<(int x, int z), Chunk> loadingChunks;
    public static ObjectPool<Chunk> pool_chunks;
    // public static ObjectPool<GameObject>[] pool_decor;
    public static Dictionary<(int x, int z), Chunk> loadedChunks;
    public static Vector3Int prevPos, currPos;
    static int scrollX { get { return currPos.x; } }
    static int scrollZ { get { return currPos.z; } }
    static byte[,] rain_temp_map;
    static int rain_temp_map_width;
    static Biome[] biomes;

    public void Init()
    {
        RenderDistance = 7;

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
        biomesData = null;

        // pool_decor = new ObjectPool<GameObject>[Biome.s_decorations.Length];
        // foreach(var pair in Biome.s_indexes)
        // {
        //     pool_decor[pair.Value] = new ObjectPool<GameObject>(
        //         () => {
        //             // on create
        //             var decor = Instantiate(Biome.s_decorations[pair.Value], transform_decor);

        //             return decor;
        //         },
        //         (decor) => {
        //             // on get
        //             decor.SetActive(true);
        //         },
        //         (decor) => {
        //             // on return
        //             decor.SetActive(false);
        //         },
        //         (decor) => {
        //             // on destroy
        //             Destroy(decor);
        //         },
        //         false, maxChunks, maxChunks*MAX_DECORS
        //     );
        // }

        loadingChunks = new Dictionary<(int x, int z), Chunk>();

        pool_chunks = new ObjectPool<Chunk>(
            () => {
                // on create
                var go = Instantiate(prefab_chunk, transform_chunks);
                var chunk = go.GetComponent<Chunk>();
                var mesh = new Mesh();
                mesh.MarkDynamic();
                // mesh.vertices = new Vector3[(int)Math.Sqr(chunkWidthVertices)*(chunkHeightVertices+1)];
                // mesh.triangles = new int[(int)Math.Sqr(chunkWidthVertices-1)*2*3]; // 2 triangles per 4 vertices, 3 vertices per triangle
                mesh.bounds = new Bounds(new Vector3(chunkSize/2, chunkHeight/2, chunkSize/2), new Vector3(chunkSize, chunkHeight, chunkSize));
                chunk.vertices = new List<Vector3>(100);
                chunk.triangles = new List<int>(100);
                chunk.meshFilter.mesh = mesh;
                // chunk.meshRenderer.materials = new Material[biomes.Length];
                // for(int i=0; i<biomes.Length; i++) {
                //     chunk.meshRenderer.materials[i] = biomes[i].material;
                // }
                // mesh.subMeshCount = biomes.Length;

                // chunk.decorPositions = new Vector3[MAX_DECORS];
                // chunk.decors = new int[MAX_DECORS];
                // chunk.decorRefs = new GameObject[MAX_DECORS];

                return chunk;
            },
            (chunk) => {
                // on get
                chunk.gameObject.SetActive(true);
            },
            (chunk) => {
                // on return
                chunk.meshCollider.enabled = false;
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

    public static long RandomSeed()
    {
        seed = Math.RandLong();
        return seed;
    }

    static float Perlin(float seed, float x, float z, float chunkX, float chunkZ, float min=0, float max=1, float scale=1)
    {
        const int perlinOffset = 34546; // prevents mirroring
        return Math.Remap(Mathf.PerlinNoise((perlinOffset+seed+x+instance.chunkSize*chunkX)*scale, (perlinOffset+seed+z+instance.chunkSize*chunkZ)*scale), 0, 1, min, max);
    }

    public static float IsoLevel(int x, int y, int z, int chunkX=0, int chunkZ=0, int biome=0) {
        float val = Math.Perlin3D(seed_grnd, x*cubeWidth+chunkX*instance.chunkSize, y*cubeWidth, z*cubeWidth+chunkZ*instance.chunkSize, instance.groundScale);

        if(y <= 8) {
            val += Math.Remap(y, 0, 9, instance.groundThreshhold, 0);
        }
        else if(y >= 18 && y <= 26) {
            val += Math.Remap(y, 18, 26, 0, instance.groundThreshhold*0.4f);
        }
        else if(y >= 27) {
            val += Math.Remap(y, 27, 32, instance.groundThreshhold*0.4f, -instance.groundThreshhold);
        }

        val = Mathf.Clamp(val, 0, 1);

        return val;
    }

    public static bool IsGround(int x, int y, int z, int chunkX=0, int chunkZ=0, int biome=0, float isolevel=-1) {
        if(isolevel == -1) isolevel = IsoLevel(x, y, z, chunkX, chunkZ);
        return isolevel >= instance.groundThreshhold;
    }

    public static float Wavy(float x, float y, float z, int chunkX=0, int chunkZ=0) {
        return Math.Remap(Math.Perlin3D(seed_wave, x+chunkX*instance.chunkSize, y, z+chunkZ*instance.chunkSize, instance.waveScale), 0, 1, -instance.waveAmount, instance.waveAmount);
    }

    public static byte MapClamped(byte[,] map, int x, int y)
    {
        return map[Mathf.Clamp(x, 0, map.GetLength(0)-1), Mathf.Clamp(y, 0, map.GetLength(1)-1)];
    }

    public static int PerlinBiome(float x, float z, float chunkX=0, float chunkZ=0)
    {
        const float perlinScaleRain = 0.003f;
        const float perlinScaleTemp = 0.003f;

        float perlinValRain = Perlin(seed_rain, x, z, chunkX, chunkZ, 0, 1, perlinScaleRain);
        float perlinValTemp = Perlin(seed_temp, x, z, chunkX, chunkZ, 0, 1, perlinScaleTemp);

        float perlinScaleFine = 0.1f;
        float fineNoise = Perlin(seed_fine, x, z, chunkX, chunkZ, 0, 0.05f, perlinScaleFine);

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

    struct CubeInfo {
        public int[] e;
    }

    readonly sbyte[] flipx = {2, -1, -1, -1, 6, -1, -1, -1, 11, 10};
    readonly sbyte[] flipy = {4, 5, 6, 7};
    readonly sbyte[] flipz = {-1, -1, -1, 1, -1, -1, -1, 5, 9, -1, -1, 10};

    readonly Vector3Int[] corners = {
        new Vector3Int(0, 0, 0),
        new Vector3Int(0, 0, 1),
        new Vector3Int(1, 0, 1),
        new Vector3Int(1, 0 ,0),
        new Vector3Int(0, 1, 0),
        new Vector3Int(0, 1, 1),
        new Vector3Int(1, 1, 1),
        new Vector3Int(1, 1 ,0),
    };
    
    readonly Vector2Int[] interpVals = {
        new Vector2Int(0, 1),
        new Vector2Int(1, 2),
        new Vector2Int(2, 3),
        new Vector2Int(3, 0),
        new Vector2Int(4, 5),
        new Vector2Int(5, 7),
        new Vector2Int(6, 7),
        new Vector2Int(7, 4),
        new Vector2Int(0, 4),
        new Vector2Int(1, 5),
        new Vector2Int(2, 6),
        new Vector2Int(3, 7)
    };

    async void Load(int chunkX, int chunkZ)
    {
        if(loadedChunks.ContainsKey((chunkX, chunkZ))) return;

        var chunk = pool_chunks.Get();
        loadedChunks.Add((chunkX, chunkZ), chunk);
        chunk.transform.SetPositionAndRotation(new Vector3(chunkX*chunkSize, 0, chunkZ*chunkSize), Quaternion.identity);

        CubeInfo[,,] cubeInfos = new CubeInfo[chunkWidthCubes,chunkHeightCubes,chunkWidthCubes];

        int AddVertex(Vector3 v) {
            chunk.vertices.Add(v);
            return chunk.vertices.Count-1;
        }

        float[] isos = new float[12];
        Vector3 VertexInterp(int i1, int i2)
        {
            float valp1 = isos[i1];
            float valp2 = isos[i2];
            var p1 = corners[i1];
            var p2 = corners[i2];
            float mu = (groundThreshhold - valp1) / (valp2 - valp1);
            return new Vector3(p1.x + mu * (p2.x - p1.x), p1.y + mu * (p2.y - p1.y), p1.z + mu * (p2.z - p1.z))*cubeWidth;
        }

        Vector3 of = new Vector3(0, 0, 0);
        int[] vertIndices = new int[12];
        int AddVertexi(int i, int x, int y, int z) {
            of.Set(x*cubeWidth, y*cubeWidth, z*cubeWidth);
            of += VertexInterp(interpVals[i].x, interpVals[i].y);
            of.y += Wavy(of.x, of.y, of.z, chunkX, chunkZ);
            return AddVertex(of);
        }

        int GetVertex(int i, int x, int y, int z) {
            if(x > 0 && ((1 << i) & 0b001100010001) != 0) return cubeInfos[x-1, y, z].e[flipx[i]];
            if(y > 0 && ((1 << i) & 0b000000001111) != 0) return cubeInfos[x, y-1, z].e[flipy[i]];
            if(z > 0 && ((1 << i) & 0b100110001000) != 0) return cubeInfos[x, y, z-1].e[flipz[i]];
            return AddVertexi(i, x, y, z);
        }

        void AddTriangle(int a, int b, int c) {
            chunk.triangles.Add(a);
            chunk.triangles.Add(b);
            chunk.triangles.Add(c);
        }

        int biome=0;
        bool IsGround_(int x, int y, int z, float iso) {
            return IsGround(x, y, z, chunkX, chunkZ, biome, iso);
        }

        await Task.Run(() => {
            chunk.vertices.Clear();
            chunk.triangles.Clear();

            for(int x=0; x<chunkWidthCubes; ++x) for(int z=0; z<chunkWidthCubes; ++z)
            {
                biome = PerlinBiome(x*cubeWidth, z*cubeWidth, chunkX, chunkZ);

                for(int y=0; y<chunkHeightCubes; ++y)
                {
                    for(int i=0; i<8; i++) {
                        isos[i] = IsoLevel(x+corners[i].x, y+corners[i].y, z+corners[i].z, chunkX, chunkZ, biome);
                    }
                    var cubeindex = CubeIndex(isos, instance.groundThreshhold);

                    if(edgeTable[cubeindex] == 0) continue;

                    cubeInfos[x,y,z].e = new int[12];

                    for(int i=0, n=1; i<12; i++, n*=2) {
                        if((edgeTable[cubeindex] & n) != 0) {
                            vertIndices[i] = GetVertex(i, x, y, z);
                            cubeInfos[x,y,z].e[i] = vertIndices[i];
                        }
                    }

                    for(int i=0; triTable[cubeindex,i] != -1; i+=3) {
                        AddTriangle(vertIndices[triTable[cubeindex,i]], vertIndices[triTable[cubeindex,i+1]], vertIndices[triTable[cubeindex,i+2]]);
                    }
                }
            }
        });

        // numOfDecors = 1;
        // decors[0] = 0;
        // decorPositions[0].Set(chunkX*chunkSize, Height(0, 0, chunkX, chunkZ), chunkZ*chunkSize);

        // chunk.decorPositions = decorPositions;
        // chunk.decors = decors;
        // chunk.numOfDecors = numOfDecors;

        float dist = Math.Dist(currPos.x, currPos.z, chunkX, chunkZ);

        loadingChunks.Add((chunkX, chunkZ), chunk);
    }

    static void Unload(int x, int z)
    {
        if(!loadedChunks.ContainsKey((x, z)) && !loadingChunks.ContainsKey((x, z))) return;

        Chunk chunk;
        if(loadingChunks.ContainsKey((x, z))) {
            chunk = loadingChunks[(x, z)];
            loadingChunks.Remove((x, z));
        } else {
            chunk = loadedChunks[(x, z)];
            loadedChunks.Remove((x, z));
        }

        // for(int i=0; i<chunk.numOfDecors; i++)
        // {
        //     if(chunk.decorRefs[i] != null) pool_decor[chunk.decors[i]].Release(chunk.decorRefs[i]);
        // }
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

        var offset = new Vector3(chunkSize/2, 0, chunkSize/2);
        for(int i=0; loadingChunks.Count > 0 && i < chunksPerFrame; i++)
        {
            (int x, int z) closest = (0, 0);
            var closestDist = Mathf.Infinity;
            foreach(var pair in loadingChunks) {
                var dist = Vector3.Distance(PlayerMovement.rb.position, pair.Value.transform.position+offset);
                if(dist < closestDist)
                {
                    closestDist = dist;
                    closest = pair.Key;
                }
            }

            var chunk = loadingChunks[closest];
            loadingChunks.Remove(closest);
            
            chunk.meshFilter.sharedMesh.Clear();
            chunk.meshFilter.sharedMesh.SetVertices(chunk.vertices);
            chunk.meshFilter.sharedMesh.SetTriangles(chunk.triangles, 0, false);
            chunk.meshFilter.sharedMesh.UploadMeshData(false);

            chunk.meshFilter.mesh.RecalculateNormals();
            chunk.meshCollider.sharedMesh = chunk.meshFilter.sharedMesh;
            chunk.meshCollider.enabled = true;


            // for(int di=0; di<chunk.numOfDecors; di++)
            // {
            //     var decor = pool_decor[chunk.decors[di]].Get();
            //     decor.transform.position = chunk.decorPositions[di];
            //     chunk.decorRefs[di] = decor;
            // }
        }
    }

    void FixedUpdate()
    {
        
    }
}
