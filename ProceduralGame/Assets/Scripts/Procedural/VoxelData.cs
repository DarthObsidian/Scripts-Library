using UnityEngine;

public static class VoxelData
{
    public static readonly Vector2Int chunkSize = new Vector2Int(5, 10);
    public static readonly int TextureAtlasSizeInBlocks = 4;
    public static readonly int WorldSizeInChunks = 100;

    public static int WorldSizeInVoxels => WorldSizeInChunks * chunkSize.x;

    public static readonly int ViewDistanceInChunks = 5;
    
    public static float NormalizedBlockTextureSize => 1f / TextureAtlasSizeInBlocks;

    public static readonly Vector3Int[] voxelVerts = 
    {
        new Vector3Int(0, 0, 0),
        new Vector3Int(1, 0, 0),
        new Vector3Int(1, 1, 0),
        new Vector3Int(0, 1, 0),
        new Vector3Int(0, 0, 1),
        new Vector3Int(1, 0, 1),
        new Vector3Int(1, 1, 1),
        new Vector3Int(0, 1, 1),
    };

    public static readonly Vector3Int[] faceChecks =
    {
        new Vector3Int(0, 0, -1), //Back Face
        new Vector3Int(0, 0, 1), //Front Face
        new Vector3Int(0, 1, 0), //Top Face
        new Vector3Int(0, -1, 0), //Bottom Face
        new Vector3Int(-1, 0, 0), //Left Face
        new Vector3Int(1, 0, 0), //Right Face

    };
    
    public static readonly int[,] voxelTris = 
    {
        //Back, Front, Top, Bottom, Left, Right
        
        { 0, 3, 1, 2 }, //Back Face
        { 5, 6, 4, 7 }, //Front Face
        { 3, 7, 2, 6 }, //Top Face
        { 1, 5, 0, 4 }, //Bottom Face
        { 4, 7, 0, 3 }, //Left Face
        { 1, 2, 5, 6 }  //Right Face
    };
}
