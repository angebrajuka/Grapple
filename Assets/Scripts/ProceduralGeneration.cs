using UnityEngine;


public class ProceduralGeneration : MonoBehaviour
{
    public static ProceduralGeneration instance;

    static float seed;

    public void Init()
    {
        instance = this;

        RandomSeed();
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

    static float Perlin(float seed, float x, float z, float chunkX, float chunkZ, float scale=1)
    {
        const int perlinOffset = 34546; // prevents mirroring
        return Mathf.PerlinNoise((perlinOffset+seed+x+DynamicLoading.CHUNK_SIZE*chunkX)*scale, (perlinOffset+seed+z+DynamicLoading.CHUNK_SIZE*chunkZ)*scale);
    }

    public static void LoadChunk(int chunkX, int chunkZ, GameObject chunk)
    {
        var meshFilter = chunk.GetComponent<MeshFilter>();
        var meshCollider = chunk.GetComponent<MeshCollider>();

        var mesh = new Mesh();
        Vector3[] vertices = new Vector3[(int)Math.Sqr(DynamicLoading.CHUNK_SIZE+1)];
        int[] triangles = new int[(int)Math.Sqr(DynamicLoading.CHUNK_SIZE)*6];

        int i=0;
        for(int x=0; x<=DynamicLoading.CHUNK_SIZE; ++x) for(int z=0; z<=DynamicLoading.CHUNK_SIZE; ++z)
        {
            vertices[(DynamicLoading.CHUNK_SIZE+1)*x+z] = new Vector3(x, Perlin(seed, x, z, chunkX, chunkZ, 0.2f), z);
            if(x<DynamicLoading.CHUNK_SIZE && z<DynamicLoading.CHUNK_SIZE)
            {
                triangles[6*i]   = (DynamicLoading.CHUNK_SIZE+1)*x+z;
                triangles[6*i+1] = (DynamicLoading.CHUNK_SIZE+1)*x+z+1;
                triangles[6*i+2] = (DynamicLoading.CHUNK_SIZE+1)*(x+1)+z;
                triangles[6*i+3] = (DynamicLoading.CHUNK_SIZE+1)*(x+1)+z;
                triangles[6*i+4] = (DynamicLoading.CHUNK_SIZE+1)*x+z+1;
                triangles[6*i+5] = (DynamicLoading.CHUNK_SIZE+1)*(x+1)+z+1;
                ++i;
            }
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;

        meshFilter.mesh = mesh;
        meshCollider.sharedMesh = mesh;
    }

}