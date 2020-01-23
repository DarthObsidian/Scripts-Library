using UnityEngine;

public static class HexVoxel
{
    public const float outerRadius = 1f;
    public const float innerRadius = outerRadius * 0.866025404f;
    
    public static readonly int TextureAtlasSizeInBlocks = 4;
    public static float NormalizedBlockTextureSize => 1f / TextureAtlasSizeInBlocks;
    
    public static Vector3[] hexVerts = 
    {
        //bottom points
        new Vector3(0f, 0f, outerRadius),
        new Vector3(innerRadius, 0f, 0.5f * outerRadius),
        new Vector3(innerRadius, 0f, -0.5f * outerRadius),
        new Vector3(0f, 0f, -outerRadius),
        new Vector3(-innerRadius, 0f, -0.5f * outerRadius),
        new Vector3(-innerRadius, 0f, 0.5f * outerRadius),

        //top points
        new Vector3(0f, 1f, outerRadius),
        new Vector3(innerRadius, 1f, 0.5f * outerRadius),
        new Vector3(innerRadius, 1f, -0.5f * outerRadius),
        new Vector3(0f, 1f, -outerRadius),
        new Vector3(-innerRadius, 1f, -0.5f * outerRadius),
        new Vector3(-innerRadius, 1f, 0.5f * outerRadius)
    };

    //provides the vertex indices that each triangle is made up of, two triangles per list
    public static readonly int[,] hexSideTris =
    {
        { 1, 7, 0, 6 },     //top right
        { 2, 8, 1, 7 },     //right
        { 3, 9, 2, 8 },     //bottom right
        { 4, 10, 3, 9 },    //bottom left
        { 5, 11, 4, 10 },   //left
        { 0, 6, 5, 11 }     //top left
    };

    //contains the vertex indices of each triangle in the main hex faces
    public static readonly int[,] hexTopTris =
    {
        { 10, 11, 9, 6, 8, 7 }, //top face
        { 4, 3, 5, 2, 0, 1 }    //bottom face
    };

    //provides the directions to check for an existing hex
    public static readonly Vector4[] faceChecks =
    {
        //x,w,z,y
        new Vector4(1, -1, 0, 0),   //top right
        new Vector4(0, -1, 1, 0),   //right
        new Vector4(-1, 0, 1, 0),   //bottom right
        new Vector4(-1, 1, 0, 0),   //bottom left
        new Vector4(0, 1, -1, 0),   //left
        new Vector4(1, 0, -1, 0),   //top left
        new Vector4(0, 0, 0, 1),    //top
        new Vector4(0, 0, 0, -1),   //bottom
    };
}