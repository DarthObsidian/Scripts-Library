using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexChunk : MonoBehaviour
{
    private List<Vector3> vertices = new List<Vector3>();
    private List<int> triangles = new List<int>();
    public MeshFilter meshFilter => GetComponent<MeshFilter>();

    void AddTriangle(Vector3 v1, Vector3 v2, Vector3 v3)
    {
        int vertexIndex = vertices.Count;
        for (int i = 0; i < HexVoxel.hexTris.Length; i++)
        {
            vertices.Add(HexVoxel.hexVerts[HexVoxel.hexTris[i, 0]]);
            vertices.Add(HexVoxel.hexVerts[HexVoxel.hexTris[i, 1]]);
            vertices.Add(HexVoxel.hexVerts[HexVoxel.hexTris[i, 2]]);

            triangles.Add(vertexIndex);
            triangles.Add(vertexIndex + 1);
            triangles.Add(vertexIndex + 2);
        }
    }

    void CreateMesh()
    {

    }
}
