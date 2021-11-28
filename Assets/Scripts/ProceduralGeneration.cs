using UnityEngine;


public class ProceduralGeneration : MonoBehaviour
{
    public static ProceduralGeneration instance;

    static float seed;

    public void Init()
    {
        instance = this;
    }

    public static void RandomSeed()
    {
        seed = Random.value*4586+Random.value;
    }

    public static void RelativeSeeds()
    {

    }

    public static void LoadChunk(int chunkX, int chunkZ, GameObject chunk)
    {
        var meshFilter = chunk.GetComponent<MeshFilter>();
        var meshCollider = chunk.GetComponent<MeshCollider>();

        var mesh = new Mesh();
        Vector3[] vertices = new Vector3[(int)Math.Sqr(DynamicLoading.CHUNK_SIZE+1)];
        int[] triangles = new int[(int)Math.Sqr(DynamicLoading.CHUNK_SIZE)*3];

        int i=0;
        for(int x=0; x<=DynamicLoading.CHUNK_SIZE; ++x) for(int z=0; z<=DynamicLoading.CHUNK_SIZE; ++z)
        {
            vertices[(DynamicLoading.CHUNK_SIZE+1)*x+z] = new Vector3(x, Random.value/2f, z);
            if(x<DynamicLoading.CHUNK_SIZE && z<DynamicLoading.CHUNK_SIZE)
            {
                triangles[3*i]   = (DynamicLoading.CHUNK_SIZE+1)*x+z;
                triangles[3*i+1] = (DynamicLoading.CHUNK_SIZE+1)*x+z+1;
                triangles[3*i+2] = (DynamicLoading.CHUNK_SIZE+1)*(x+1)+z;
                ++i;
            }
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;

        meshFilter.mesh = mesh;
        meshCollider.sharedMesh = mesh;
    }

}