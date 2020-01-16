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
        CreateVoxelMeshData();
        CreateMesh();
    }

    //Creates the data for each voxel
    void CreateVoxelMeshData()
    {
        for (int i = 0; i < HexVoxel.hexSideTris.GetLength(0); i++)
        {
            int vertexIndex = vertices.Count;
            
            vertices.Add(HexVoxel.hexVerts[HexVoxel.hexSideTris[i, 0]]);
            vertices.Add(HexVoxel.hexVerts[HexVoxel.hexSideTris[i, 1]]);
            vertices.Add(HexVoxel.hexVerts[HexVoxel.hexSideTris[i, 2]]);
            vertices.Add(HexVoxel.hexVerts[HexVoxel.hexSideTris[i, 3]]);

            uvs.Add(HexVoxel.hexUvs[0]);
            uvs.Add(HexVoxel.hexUvs[1]);
            uvs.Add(HexVoxel.hexUvs[2]);
            uvs.Add(HexVoxel.hexUvs[3]);

            triangles.Add(vertexIndex);
            triangles.Add(vertexIndex + 1);
            triangles.Add(vertexIndex + 2);
            triangles.Add(vertexIndex + 2);
            triangles.Add(vertexIndex + 1);
            triangles.Add(vertexIndex + 3);
        }

        for (int i = 0; i < HexVoxel.hexTopTris.GetLength(0); i++)
        {
            int vertexIndex = vertices.Count;
            vertices.Add(HexVoxel.hexVerts[HexVoxel.hexTopTris[i, 0]]);
            vertices.Add(HexVoxel.hexVerts[HexVoxel.hexTopTris[i, 1]]);
            vertices.Add(HexVoxel.hexVerts[HexVoxel.hexTopTris[i, 2]]);
            vertices.Add(HexVoxel.hexVerts[HexVoxel.hexTopTris[i, 3]]);
            vertices.Add(HexVoxel.hexVerts[HexVoxel.hexTopTris[i, 4]]);
            vertices.Add(HexVoxel.hexVerts[HexVoxel.hexTopTris[i, 5]]);
            
            uvs.Add(new Vector2(0f, 0.5f));
            uvs.Add(new Vector2(0.25f, 1f));
            uvs.Add(new Vector2(0.25f, 0f));
            uvs.Add(new Vector2(0.75f, 1f));
            uvs.Add(new Vector2(0.75f, 0f));
            uvs.Add(new Vector2(1f, 0.5f));

            triangles.Add(vertexIndex);
            triangles.Add(vertexIndex + 1);
            triangles.Add(vertexIndex + 2);
            triangles.Add(vertexIndex + 2);
            triangles.Add(vertexIndex + 1);
            triangles.Add(vertexIndex + 3);
            triangles.Add(vertexIndex + 2);
            triangles.Add(vertexIndex + 3);
            triangles.Add(vertexIndex + 4);
            triangles.Add(vertexIndex + 4);
            triangles.Add(vertexIndex + 3);
            triangles.Add(vertexIndex + 5);
        }
    }

    //creates the mesh from the provided voxel data
    void CreateMesh()
    {
        var mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();
        
        mesh.RecalculateNormals();

        meshFilter.mesh = mesh;
    }
}
