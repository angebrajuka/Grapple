using UnityEngine;

public class Chunk : MonoBehaviour
{
    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;
    public MeshCollider meshCollider;
    public Vector3[] decorPositions;
    public int[] decors;
    public int numOfDecors;
    public GameObject[] decorRefs;
}