using UnityEngine;


public class ProceduralGeneration : MonoBehaviour
{
    public static ProceduralGeneration instance;

    public static MeshFilter meshFilter;
    public static MeshCollider meshCollider;
    public static MeshRenderer meshRenderer;

    static Mesh mesh;

    

    public void Init()
    {
        instance = this;

        meshFilter = GetComponent<MeshFilter>();
        meshCollider = GetComponent<MeshCollider>();
        meshRenderer = GetComponent<MeshRenderer>();

        Generate();
    }

    public void Generate() {
        mesh = new Mesh();

        Vector3[] newVertices = new Vector3[100];
        int[] newTriangles = new int[81*3];

        int i=0;
        for(int x=0; x<10; ++x) for(int y=0; y<10; ++y)
        {
            newVertices[10*y+x] = new Vector3(x, y, Random.value/2f);
            if(x<9 && y<9)
            {
                newTriangles[3*i]   = 10*y+x;
                newTriangles[3*i+1] = 10*(y+1)+x;
                newTriangles[3*i+2] = 10*y+x+1;
                ++i;
            }
        }

        mesh.vertices = newVertices;
        mesh.triangles = newTriangles;

        meshFilter.mesh = mesh;
        meshCollider.sharedMesh = mesh;
    }

}