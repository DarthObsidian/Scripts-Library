using System.Collections.Generic;
using UnityEngine;

public class HexChunk : MonoBehaviour
{
    private List<Vector3> vertices = new List<Vector3>();
    private List<int> triangles = new List<int>();
    private List<Vector2> uvs = new List<Vector2>();
    public MeshFilter meshFilter => GetComponent<MeshFilter>();

    public static readonly int chunkWidth = 5;
    public static readonly int chunkHeight = 10;

    Dictionary<HexCoordinates, byte> voxelCoords = new Dictionary<HexCoordinates, byte>();

    private void Start()
    {
        PopulateVoxelMap();

        CreateChunk();
        CreateMesh();
    }

    //creates a list containing each voxel position
    private void PopulateVoxelMap()
    {
        for (int y = 0; y < chunkHeight; y++)
        {
            for (int z = 0; z < chunkWidth; z++)
            {
                for (int x = 0; x < chunkWidth; x++)
                {
                    var coord = HexCoordinates.FromOffsetCoordinates(x,y,z);
                    voxelCoords.Add(coord, 1);
                }
            }
        }
    }

    //checks if the provided coordinates are occupied by a voxel
    private bool CheckVoxel(Vector4 pos)
    {
        var coord = new HexCoordinates((int)pos.x, (int)pos.w, (int)pos.z);
        return voxelCoords.ContainsKey(coord);
    }

    //creates a chunk
    private void CreateChunk()
    {
        for (int y = 0; y < chunkHeight; y++)
        {
            for (int z = 0; z < chunkWidth; z++)
            {
                for (int x = 0; x < chunkWidth; x++)
                {
                    float posX = (x + z * 0.5f - z / 2) * (HexVoxel.innerRadius * 2f);
                    float posZ = z * (HexVoxel.outerRadius * 1.5f);
                    var coord = HexCoordinates.FromOffsetCoordinates(x,y,z);
                    CreateVoxelMeshData(new Vector3(posX, y, posZ), new Vector4(coord.x, coord.w,  coord.z, coord.y));
                }
            }
        }
    }

    //Creates the vertex, tri, and uv data for each voxel
    private void CreateVoxelMeshData(Vector3 pos, Vector4 checkPos)
    {
        for (int i = 0; i < HexVoxel.hexSideTris.GetLength(0); i++)
        {
            //only add the face if it is visible
            if(!CheckVoxel(checkPos + HexVoxel.faceChecks[i]))
            {
                int vertexIndex = vertices.Count;
            
                vertices.Add(HexVoxel.hexVerts[HexVoxel.hexSideTris[i, 0]] + pos);
                vertices.Add(HexVoxel.hexVerts[HexVoxel.hexSideTris[i, 1]] + pos);
                vertices.Add(HexVoxel.hexVerts[HexVoxel.hexSideTris[i, 2]] + pos);
                vertices.Add(HexVoxel.hexVerts[HexVoxel.hexSideTris[i, 3]] + pos);

                //uvs for the square faces
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
        }

        for (int i = 0; i < HexVoxel.hexTopTris.GetLength(0); i++)
        {
            if(!CheckVoxel(checkPos + HexVoxel.faceChecks[i + HexVoxel.hexSideTris.GetLength(0)]))
            {
                int vertexIndex = vertices.Count;

                vertices.Add(HexVoxel.hexVerts[HexVoxel.hexTopTris[i, 0]] + pos);
                vertices.Add(HexVoxel.hexVerts[HexVoxel.hexTopTris[i, 1]] + pos);
                vertices.Add(HexVoxel.hexVerts[HexVoxel.hexTopTris[i, 2]] + pos);
                vertices.Add(HexVoxel.hexVerts[HexVoxel.hexTopTris[i, 3]] + pos);
                vertices.Add(HexVoxel.hexVerts[HexVoxel.hexTopTris[i, 4]] + pos);
                vertices.Add(HexVoxel.hexVerts[HexVoxel.hexTopTris[i, 5]] + pos);
                
                //uvs for the hex face
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
    }
    
    private void AddTexture(int textureId)
    {
        float y = textureId / HexVoxel.TextureAtlasSizeInBlocks;
        float x = textureId - (y * HexVoxel.TextureAtlasSizeInBlocks);

        x *= HexVoxel.NormalizedBlockTextureSize;
        y *= HexVoxel.NormalizedBlockTextureSize;

        y = 1f - y - HexVoxel.NormalizedBlockTextureSize;
        
        uvs.Add(new Vector2(x, y));
        uvs.Add(new Vector2(x, y + HexVoxel.NormalizedBlockTextureSize));
        uvs.Add(new Vector2(x + HexVoxel.NormalizedBlockTextureSize, y));
        uvs.Add(new Vector2(x + HexVoxel.NormalizedBlockTextureSize, y + HexVoxel.NormalizedBlockTextureSize));
    }

    //creates the mesh from the provided voxel data
    private void CreateMesh()
    {
        var mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();
        
        mesh.RecalculateNormals();

        meshFilter.mesh = mesh;
    }
}
