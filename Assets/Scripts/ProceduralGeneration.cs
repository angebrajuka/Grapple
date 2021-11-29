using UnityEngine;

public class ProceduralGeneration : MonoBehaviour
{
    public static ProceduralGeneration instance;

    public const int CHUNK_SIZE = 40;

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
            vertices[(chunkSize+1)*x+z] = new Vector3((float)x/(float)density, Perlin(seed, (float)x/(float)density, (float)z/(float)density, chunkX, chunkZ, 0, 0.5f, 0.2f), (float)z/(float)density);
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

}