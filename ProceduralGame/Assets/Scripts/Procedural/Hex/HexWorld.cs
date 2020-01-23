using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexWorld : MonoBehaviour
{
    public Material mat;
    public HexBlockType[] blockTypes;
    public HexChunk[,] chunks = new HexChunk[WorldSizeInChunks, WorldSizeInChunks];
    public static readonly int WorldSizeInChunks = 1;
    public static int WorldSizeInVoxels => WorldSizeInChunks * HexChunk.chunkWidth;

    private void Start()
    {
        GenerateWorld();
    }

    //generates the entire world
    private void GenerateWorld()
    {
        for (int x = 0; x < WorldSizeInChunks; x++)
        {
            for (int z = 0; z < WorldSizeInChunks; z++)
            {
                CreateNewChunk(x, z);
            }
        }
    }

    //creates a new chunk
    private void CreateNewChunk(int x, int z)
    {
        chunks[x, z] = new HexChunk(this, new HexChunkCoord(x, z));
    }

    //calculates what type of block the voxel is
    public byte GetVoxel (Vector3 pos)
    {
        if(!IsVoxelInWorld(pos))
            return 0;

        if(pos.y < 1)
            return 1;
        else if (pos.y == HexChunk.chunkHeight - 1)
            return 3;
        else 
            return 2;
    }

    //checks if the provided chunk is in the world
    private bool IsChunkInWorld(HexChunkCoord coord)
    {
        if(coord.x > 0 && coord.x < WorldSizeInChunks - 1 && coord.z > 0 && coord.z < WorldSizeInChunks - 1)
            return true;
        return false;
    }

    //checks if the given voxel is in the world
    private bool IsVoxelInWorld(Vector3 pos)
    {
        if(pos.x >= 0 && pos.x < WorldSizeInVoxels && pos.y >= 0 && pos.y < HexChunk.chunkHeight && pos.z >= 0 && pos.z < WorldSizeInVoxels)
            return true;
        return false;
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
