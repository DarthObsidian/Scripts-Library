//https://observablehq.com/@sanderevers/hexagon-tiling-of-an-hexagonal-grid
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.iOS;

public class HexChunk
{
    public HexChunkCoord chunkCoord;

    GameObject chunkObject;
    MeshRenderer meshRenderer;
    MeshFilter  meshFilter;

    private List<Vector3> vertices = new List<Vector3>();
    private List<int> triangles = new List<int>();
    private List<Vector2> uvs = new List<Vector2>();

    public static readonly int chunkWidth = 3;
    public static readonly int chunkHeight = 5;

    Dictionary<HexCoordinates, byte> voxelCoords = new Dictionary<HexCoordinates, byte>();
    private HexWorld world;

    public Vector3 position => chunkObject.transform.position; 

    public bool isActive
    {
        get => chunkObject.activeSelf;
        set => chunkObject.SetActive(value);
    }

    public HexChunk(HexWorld _world, HexChunkCoord _coord)
    {
        chunkCoord = _coord;
        world = _world;
        chunkObject = new GameObject();
        meshFilter = chunkObject.AddComponent<MeshFilter>();
        meshRenderer = chunkObject.AddComponent<MeshRenderer>();
        meshRenderer.material = world.mat;
        chunkObject.transform.SetParent(world.transform);
        
        //sets the offsets for each chunk
        float posX = (chunkCoord.x + chunkCoord.z * 0.5f - chunkCoord.z / 2) * (HexVoxel.innerRadius * 2f);
        float posZ = chunkCoord.z * (HexVoxel.outerRadius * 1.5f);

        //moves the chunk
        chunkObject.transform.position = new Vector3(chunkWidth * posX, 0f, chunkWidth * posZ);
        chunkObject.name = "Chunk " + chunkCoord.x + " " + chunkCoord.z;

        PopulateVoxelMap();
        CreateChunk();
        CreateMesh();
    }

    //creates a list containing each voxel position
    private void PopulateVoxelMap()
    {
        for (int y = 0; y < chunkHeight; y++)
        {
            for (int x = -chunkWidth; x <= chunkWidth; x++)
            {
                int z1 = Mathf.Max(-chunkWidth, -x - chunkWidth);
                int z2 = Mathf.Min(chunkWidth, -x + chunkWidth);
                for (int z = z1; z <= z2; z++)
                {
                    var coord = new HexCoordinates(x,y,z);
                    voxelCoords.Add(coord, 1);
                }
            }
        }
    }

    //checks if the given voxel is in this chunk
    private bool IsVoxelInChunk(HexCoordinates coord)
    {
        return voxelCoords.ContainsKey(coord);
    }

    //checks if the voxel at the provided coordinates is solid
    private bool CheckVoxel(Vector4 pos)
    {
        var coord = new HexCoordinates((int)pos.x, (int)pos.w, (int)pos.z);

        //if(!IsVoxelInChunk(coord))
            //return world.blockTypes[world.GetVoxel(new Vector3(pos.x, pos.y, pos.z) + position)].isSolid;
        return IsVoxelInChunk(coord) /*&& world.blockTypes[voxelCoords[coord]].isSolid*/;
    }

    //creates a chunk
    private void CreateChunk()
    {
        foreach (var item in voxelCoords)
        {
            float posX = HexVoxel.outerRadius * (Mathf.Sqrt(3) * item.Key.z  +  Mathf.Sqrt(3)/2 * item.Key.x);
            float posZ = HexVoxel.outerRadius * 1.5f * (3 / 2 * item.Key.x);
            CreateVoxelMeshData(new Vector3(posX, item.Key.y, posZ), new Vector4(item.Key.x, item.Key.w, item.Key.z, item.Key.y));
        }
    }

    //Creates the vertex, tri, and uv data for each voxel
    private void CreateVoxelMeshData(Vector3 pos, Vector4 checkPos)
    {
        // var textObj = new GameObject();
        // var text = textObj.AddComponent<TextMesh>();
        // text.text = $"{checkPos.x}, {checkPos.z}, {checkPos.y}";
        // text.anchor = TextAnchor.MiddleCenter;
        // textObj.transform.position = pos;
        // textObj.name = text.text;
        // text.fontSize = 200;
        // text.transform.localScale = new Vector3(0.02f,0.02f,0.02f);
        
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
                AddTexture(world.blockTypes[blockId].GetTextureID(i + HexVoxel.hexSideTris.GetLength(0)), true);

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
    
    //adds a texture and uv data to voxel
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

public class HexChunkCoord
{
    public int x;
    public int z;

    public HexChunkCoord(int _x, int _z)
    {
        x = _x;
        z = _z;
    }
}