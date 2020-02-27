using System.Collections.Generic;
using UnityEngine;

public class HexWorld : MonoBehaviour
{
    public Material mat;
    public HexBlockType[] blockTypes;
    public Dictionary<HexCoordinates, HexChunk> chunks = new Dictionary<HexCoordinates, HexChunk>();
    List<HexCoordinates> coords = new List<HexCoordinates>();
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
                coords.Add(coord);
            }
        }
        foreach (var chunk in coords)
        {
            CreateNewChunk(chunk);
        }
    }

    //creates a new chunk
    private void CreateNewChunk(HexCoordinates coord)
    {
        chunks.Add(coord, new HexChunk(this, coord));
    }

    //calculates what type of block the voxel is
    public byte GetVoxel(HexCoordinates coord, HexCoordinates dir, HexChunk chunk)
    {
        if(!IsVoxelInWorld(coord, dir, chunk))
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
        return chunks.ContainsValue(chunk);
    }

    //checks if the chunk is in the world
    private bool IsChunkInWorld(HexCoordinates coord)
    {
        return chunks.ContainsKey(coord);
    }

    //checks if the given voxel is in the world
    private bool IsVoxelInWorld(HexCoordinates coord, HexCoordinates dir, HexChunk chunk)
    {
        //if the voxel is looking at itself then it exists
        if(dir == HexCoordinates.zero)
            return true;

        int hexMod = HexChunk.CalcHexmod(coord);
        HexCoordinates desiredHex = HexChunk.CalcCoordFromHexmod(HexChunk.CalcDesiredHexmod(hexMod, dir), coord.y + dir.y);
        Vector3 voxelPos = CalculatePos(coord) + chunk.position;
        Vector3 nextVoxelPos = CalculatePos(desiredHex);
        
        foreach(var item in chunk.neighbors)
        {
            Vector3 neighborPos;
            if(!chunks.ContainsKey(item))
                neighborPos = CalculateChunkPos(item);
            else
                neighborPos = chunks[item].position;
                
            foreach (var hexChunk in coords)
            {
                if(hexChunk == item && Vector3.Distance(voxelPos, nextVoxelPos + neighborPos) <= HexVoxel.outerRadius * 1.75)
                {
                    Debug.Log("voxel found");
                    return true;
                }
            }
        }
        return false;
    }

    //calculates the unity local position of a provided hex coord
    private Vector3 CalculatePos(HexCoordinates coord)
    {
        float posX = HexVoxel.outerRadius * (Mathf.Sqrt(3) * coord.z  +  Mathf.Sqrt(3)/2 * coord.x);
        float posZ = HexVoxel.outerRadius * 1.5f * (3 / 2 * coord.x);
        return new Vector3(posX, coord.y, posZ);
    }

    private Vector3 CalculateChunkPos(HexCoordinates coord)
    {
        //finds the number of hexvoxel.outerradius' that make up the inner radius of the chunk hex
        int num = 0;
        for (int i = 0; i <= HexChunk.chunkWidth; i++)
        {
            if (i % 2 == 0 && i != 0)
                num += 2;
            else 
                num +=1;
        }

        //finds the radius of each chunk hex
        float chunkInner;
        if(HexChunk.chunkWidth % 2 == 0)
            chunkInner = (num * HexVoxel.outerRadius) - (HexVoxel.outerRadius * 0.25f);
        else
            chunkInner = (num * HexVoxel.outerRadius) + (HexVoxel.outerRadius * 0.25f);

        float chunkOuter = (chunkInner / 0.866025404f);

        //calculates the position of the chunk
        float posX = chunkOuter * 1.5f * (3 / 2 * coord.z) + (HexVoxel.innerRadius * (coord.x + coord.z * 0.5f));
        float posZ = chunkOuter * ((Mathf.Sqrt(3) / 2) * coord.z + (Mathf.Sqrt(3) * coord.x)) + (HexVoxel.innerRadius * 0.866025404f * (coord.x + coord.w));
        
        return new Vector3(posX, 0f, posZ);
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
