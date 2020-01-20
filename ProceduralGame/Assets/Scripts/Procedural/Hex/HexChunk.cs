using System.Collections.Concurrent;
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
    private HexWorld world;

    private void Start()
    {
        world = GameObject.Find("HexWorld").GetComponent<HexWorld>();
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
                    if(y < 1)
                        voxelCoords.Add(coord, 0);
                    else if (y == chunkHeight - 1)
                        voxelCoords.Add(coord, 2);
                    else 
                        voxelCoords.Add(coord, 1);
                }
            }
        }
    }

    //checks if the provided coordinates are occupied by a voxel
    private bool CheckVoxel(Vector4 pos)
    {
        var coord = new HexCoordinates((int)pos.x, (int)pos.w, (int)pos.z);
        return voxelCoords.ContainsKey(coord) && world.blockTypes[voxelCoords[coord]].isSolid;
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
                var coord = new HexCoordinates((int)checkPos.x, (int)checkPos.w, (int)checkPos.z);
                byte blockId = voxelCoords[coord];
                int vertexIndex = vertices.Count;
            
                vertices.Add(HexVoxel.hexVerts[HexVoxel.hexSideTris[i, 0]] + pos);
                vertices.Add(HexVoxel.hexVerts[HexVoxel.hexSideTris[i, 1]] + pos);
                vertices.Add(HexVoxel.hexVerts[HexVoxel.hexSideTris[i, 2]] + pos);
                vertices.Add(HexVoxel.hexVerts[HexVoxel.hexSideTris[i, 3]] + pos);

                //uvs for the square faces
                AddTexture(world.blockTypes[blockId].GetTextureID(i), false);

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
                var coord = new HexCoordinates((int)checkPos.x, (int)checkPos.w, (int)checkPos.z);
                byte blockId = voxelCoords[coord];
                int vertexIndex = vertices.Count;

                vertices.Add(HexVoxel.hexVerts[HexVoxel.hexTopTris[i, 0]] + pos);
                vertices.Add(HexVoxel.hexVerts[HexVoxel.hexTopTris[i, 1]] + pos);
                vertices.Add(HexVoxel.hexVerts[HexVoxel.hexTopTris[i, 2]] + pos);
                vertices.Add(HexVoxel.hexVerts[HexVoxel.hexTopTris[i, 3]] + pos);
                vertices.Add(HexVoxel.hexVerts[HexVoxel.hexTopTris[i, 4]] + pos);
                vertices.Add(HexVoxel.hexVerts[HexVoxel.hexTopTris[i, 5]] + pos);
                
                //uvs for the hex face
                AddTexture(world.blockTypes[blockId].GetTextureID(i + 6), true);

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
    
    private void AddTexture(int textureId, bool isHexFace)
    {
        float y = textureId / HexVoxel.TextureAtlasSizeInBlocks;
        float x = textureId - (y * HexVoxel.TextureAtlasSizeInBlocks);

        x *= HexVoxel.NormalizedBlockTextureSize;
        y *= HexVoxel.NormalizedBlockTextureSize;

        //makes texture ids start at the top instead of the bottom
        y = 1f - y - HexVoxel.NormalizedBlockTextureSize;

        if (!isHexFace)
        {
            uvs.Add(new Vector2(x, y));
            uvs.Add(new Vector2(x, y + HexVoxel.NormalizedBlockTextureSize));
            uvs.Add(new Vector2(x + HexVoxel.NormalizedBlockTextureSize, y));
            uvs.Add(new Vector2(x + HexVoxel.NormalizedBlockTextureSize, y + HexVoxel.NormalizedBlockTextureSize));
        }
        else
        {
            uvs.Add(new Vector2(x, y + (HexVoxel.NormalizedBlockTextureSize / 2)));
            uvs.Add( new Vector2(x + (HexVoxel.NormalizedBlockTextureSize / 4), y + HexVoxel.NormalizedBlockTextureSize));
            uvs.Add(new Vector2(x + (HexVoxel.NormalizedBlockTextureSize / 4), y));
            uvs.Add(new Vector2(x + (HexVoxel.NormalizedBlockTextureSize * 0.75f), y + HexVoxel.NormalizedBlockTextureSize));
            uvs.Add(new Vector2(x + (HexVoxel.NormalizedBlockTextureSize * 0.75f), y));
            uvs.Add(new Vector2(x + HexVoxel.NormalizedBlockTextureSize, y + (HexVoxel.NormalizedBlockTextureSize / 2)));

        }
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
