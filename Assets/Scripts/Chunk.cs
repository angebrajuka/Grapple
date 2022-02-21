using UnityEngine;
using System.Collections.Generic;

public class Chunk : MonoBehaviour
{
    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;
    public MeshCollider meshCollider;
    public List<Vector3> vertices;
    public List<int>[] triangles;
    // public Vector3[] decorPositions;
    // public int[] decors;
    // public int numOfDecors;
}