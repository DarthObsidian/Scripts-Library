using System.Collections.Generic;
using UnityEngine;

public class HexWorld : MonoBehaviour
{
    public Material mat;
    public HexBlockType[] blockTypes;
    public Dictionary<HexChunk, byte> chunks = new Dictionary<HexChunk, byte>();
    public static readonly int WorldSizeInChunks = 1;
    public static int WorldSizeInVoxels => WorldSizeInChunks * HexChunk.chunkWidth;

    private void Start()
    {
        GenerateWorld();
    }

    //generates the entire world
    private void GenerateWorld()
    {
        for (int x = -WorldSizeInChunks; x <= WorldSizeInChunks; x++)
        {
            int z1 = Mathf.Max(-WorldSizeInChunks, -x - WorldSizeInChunks);
            int z2 = Mathf.Min(WorldSizeInChunks, -x + WorldSizeInChunks);
            for (int z = z1; z <= z2; z++)
            {
                var coord = new HexCoordinates(x,0,z);
                CreateNewChunk(x, z);
            }
        }
    }

    //creates a new chunk
    private void CreateNewChunk(int x, int z)
    {
        chunks.Add(new HexChunk(this, new HexCoordinates(x, 0, z)), 0);
    }

    //calculates what type of block the voxel is
    public byte GetVoxel(HexCoordinates coord, HexCoordinates dir, HexChunk chunk, bool initialize=false)
    {
        if(!IsVoxelInWorld(coord, dir, chunk) && !initialize)
            return 0;

        if(coord.y < 1)
            return 1;
        else if (coord.y == HexChunk.chunkHeight - 1)
            return 3;
        else 
            return 2;
    }

    //checks if the provided chunk is in the world
    private bool IsChunkInWorld(HexChunk chunk)
    {
        return chunks.ContainsKey(chunk);
    }

    //checks if the given voxel is in the world
    private bool IsVoxelInWorld(HexCoordinates coord, HexCoordinates dir, HexChunk chunk)
    {
        int hexMod = HexChunk.CalcHexmod(coord);
        HexCoordinates desiredHex = HexChunk.CalcCoordFromHexmod(HexChunk.CalcDesiredHexmod(hexMod, dir), coord.w);

        var voxelPos = CalculatePos(coord) + chunk.position;
        var nextVoxelPos = CalculatePos(desiredHex);

        foreach(var item in chunk.neighbors)
        {
            foreach (var hexChunk in chunks)
            {
                Debug.Log($"{voxelPos} : {nextVoxelPos + hexChunk.Key.position}");
                if(hexChunk.Key.chunkCoord == item && Vector3.Distance(voxelPos, nextVoxelPos + hexChunk.Key.position) <= HexVoxel.outerRadius)
                {
                    Debug.Log($"voxel found: {desiredHex.x}, {desiredHex.y}, {desiredHex.z} in {hexChunk.Key} at pos {item.x}, {item.y}, {item.z}");
                    return true;
                }
            }
        }
        return false;
    }

    private Vector3 CalculatePos(HexCoordinates coord)
    {
        float posX = HexVoxel.outerRadius * (Mathf.Sqrt(3) * coord.z  +  Mathf.Sqrt(3)/2 * coord.x);
        float posZ = HexVoxel.outerRadius * 1.5f * (3 / 2 * coord.x);
        return new Vector3(posX, coord.y, posZ);
    }
}

[System.Serializable]
public class HexBlockType
{
    public string blockName;
    public bool isSolid;

    [Header("Texture Values")] 
    public int topRightTexture;
    public int rightTexture;
    public int bottomRightTexture;
    public int bottomLeftTexture;
    public int leftTexture;
    public int topLeftTexture;
    public int topFaceTexture; 
    public int bottomFaceTexture;

    public int GetTextureID(int faceIndex)
    {
        switch (faceIndex)
        {
            case 0:
                return topRightTexture;
            case 1:
                return rightTexture;
            case 2:
                return bottomRightTexture;
            case 3:
                return bottomLeftTexture;
            case 4:
                return leftTexture;
            case 5:
                return topLeftTexture;
            case 6:
                return topFaceTexture;
            case 7:
                return bottomFaceTexture;
            default:
                Debug.Log("Error in TextureID, invalid face index");
                return 0;
        }
    }
}
