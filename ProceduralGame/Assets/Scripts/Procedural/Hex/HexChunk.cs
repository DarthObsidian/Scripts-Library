//https://observablehq.com/@sanderevers/hexagon-tiling-of-an-hexagonal-grid
using System.Collections.Generic;
using UnityEngine;

public class HexChunk
{
    public HexCoordinates chunkCoord;

    GameObject chunkObject;
    MeshRenderer meshRenderer;
    MeshFilter  meshFilter;

    private List<Vector3> vertices = new List<Vector3>();
    private List<int> triangles = new List<int>();
    private List<Vector2> uvs = new List<Vector2>();

    public static readonly int chunkWidth = 2;
    public static readonly int chunkHeight = 1;

    public static int chunkArea => ((3 * (int)Mathf.Pow(chunkWidth, 2)) + (3*chunkWidth) + 1);
    public static int chunkShift => ((3 * chunkWidth) + 2);

    Dictionary<HexCoordinates, byte> voxelCoords = new Dictionary<HexCoordinates, byte>();
    private HexWorld world;

    //public Vector3 position => chunkObject.transform.position; 

    public bool isActive
    {
        get => chunkObject.activeSelf;
        set => chunkObject.SetActive(value);
    }

    public HexChunk(HexWorld _world, HexCoordinates _coord)
    {
        chunkCoord = _coord;
        world = _world;
        chunkObject = new GameObject();
        meshFilter = chunkObject.AddComponent<MeshFilter>();
        meshRenderer = chunkObject.AddComponent<MeshRenderer>();
        meshRenderer.material = world.mat;
        chunkObject.transform.SetParent(world.transform);
        
        //finds the number of hexvoxel.outerradius' that make up the inner radius of the chunk hex
        int num = 0;
        for (int i = 0; i <= chunkWidth; i++)
        {
            if (i % 2 == 0 && i != 0)
                num += 2;
            else 
                num +=1;
        }

        //finds the radius of each chunk hex
        float chunkInner;
        if(chunkWidth % 2 == 0)
            chunkInner = (num * HexVoxel.outerRadius) - (HexVoxel.outerRadius * 0.25f);
        else
            chunkInner = (num * HexVoxel.outerRadius) + (HexVoxel.outerRadius * 0.25f);

        float chunkOuter = (chunkInner / 0.866025404f);

        //calculates the position of the chunk
        float posX = chunkOuter * 1.5f * (3 / 2 * chunkCoord.z) + (HexVoxel.innerRadius * (chunkCoord.x + chunkCoord.z * 0.5f));
        float posZ = chunkOuter * ((Mathf.Sqrt(3) / 2) * chunkCoord.z + (Mathf.Sqrt(3) * chunkCoord.x)) + (HexVoxel.innerRadius * 0.866025404f * (chunkCoord.x + chunkCoord.w));
        
        //moves the chunk
        chunkObject.transform.position = new Vector3(posX, 0f, posZ);
        chunkObject.name = $"Chunk {chunkCoord.x}, {chunkCoord.w}, {chunkCoord.z}";

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

    //calculates the position of the hex as a single integer
    //used to find voxels in another chunk
    public int CalcHexmod(HexCoordinates coord)
    {
        int num = coord.x + chunkShift * coord.z;
        int hexmod = Mod(num, chunkArea);
        return hexmod;
    }

    //finds the hexmod in the desired direction
    public int CalcDesiredHexmod(int currentHexmod, HexCoordinates dir)
    {
        return Mod(currentHexmod + CalcHexmod(dir), chunkArea);
    }

    //finds the original coordinates from a hexmod coord
    public HexCoordinates CalcCoordFromHexmod(int hexmod, int height)
    {
        int ms = (hexmod + chunkWidth) / chunkShift;
        int mcs = (hexmod + 2 * chunkWidth) / (chunkShift - 1);
        int z = ms * (chunkWidth + 1) + mcs * -chunkWidth;
        int x = hexmod + ms * (-2 * chunkWidth - 1) + mcs * (-chunkWidth - 1);
        int y = -hexmod + ms * chunkWidth + mcs * (2 * chunkWidth + 1);
        return new HexCoordinates(x, height, z);
    }

    //used to replaced the % operator as it does negative numbers incorrectly
    private int Mod(int a, int b)
    {
        int r = a % b;
        return r < 0 ? r + b : r;
    }

    //Creates the vertex, tri, and uv data for each voxel
    private void CreateVoxelMeshData(Vector3 pos, Vector4 checkPos)
    {
        var textObj = new GameObject();
        var text = textObj.AddComponent<TextMesh>();
        int mod = CalcHexmod(new HexCoordinates((int)checkPos.x, (int)checkPos.w, (int)checkPos.z));
        HexCoordinates myCoord = CalcCoordFromHexmod(mod, (int)checkPos.w);
        
        Debug.Log($"Original: {checkPos.x}, {checkPos.y}, {checkPos.z} New: {myCoord.x}, {myCoord.w}, {myCoord.z}");
        text.text = $"{myCoord.x}, {myCoord.w}, {myCoord.z}";
        text.anchor = TextAnchor.MiddleCenter;
        textObj.transform.position = pos;
        textObj.name = text.text;
        text.fontSize = 200;
        text.transform.localScale = new Vector3(0.02f,0.02f,0.02f);
        
        //sets visibility, uvs, vertecies, and tris for square faces
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

                AddTexture(world.blockTypes[blockId].GetTextureID(i), false);

                triangles.Add(vertexIndex);
                triangles.Add(vertexIndex + 1);
                triangles.Add(vertexIndex + 2);
                triangles.Add(vertexIndex + 2);
                triangles.Add(vertexIndex + 1);
                triangles.Add(vertexIndex + 3);
            }
        }

        //sets visibility, uvs, vertecies, and tris for hex faces
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

        //only hex faces have six uvs points
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