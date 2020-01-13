using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexChunk : MonoBehaviour
{
    private List<Vector3> vertices = new List<Vector3>();
    private List<int> triangles = new List<int>();
    private List<Vector2> uvs = new List<Vector2>();
    public MeshFilter meshFilter => GetComponent<MeshFilter>();

    private void Start()
    {
        AddTriangles();
        CreateMesh();
    }

    void AddTriangles()
    {
        //foreach (var vert in HexVoxel.hexVerts)
        //    vertices.Add(vert);

        for (int i = 0; i < HexVoxel.hexTris.GetLength(0); i++)
        {
            int vertexIndex = vertices.Count;

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
        var mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        
        mesh.RecalculateNormals();

        meshFilter.mesh = mesh;
    }
}
