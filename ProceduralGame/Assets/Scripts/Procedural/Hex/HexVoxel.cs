using UnityEngine;

public static class HexVoxel
{
    public const float outerRadius = 1f;
    public const float innerRadius = outerRadius * 0.866025404f;

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
        new Vector3(-innerRadius, 1f, 0.5f * outerRadius),

        //center points
        new Vector3(0f, 0f, 0f),
        new Vector3(0f, 1f, 0f)
    };

    public static readonly int[,] hexTris =
    {
        { 1, 7, 0 },
        { 0, 7, 6 },
        { 2, 8, 1 },
        { 1, 8, 7 },
        { 3, 9, 2 },
        { 2, 9, 8 },
        { 4, 10, 3 },
        { 3, 10, 9 },
        { 5, 11, 4 },
        { 4, 11, 10 },
        { 0, 6, 5 },
        { 5, 6, 11 },
        { 0, 13, 1 },
        { 1, 13, 2 },
        { 2, 13, 3 },
        { 3, 13, 4 },
        { 4, 13, 5 },
        { 5, 13, 0 },
        { 5, 14, 8 },
        { 8, 14, 7 },
        { 7, 14, 6 },
        { 6, 14, 11 },
        { 11, 14, 10 },
        { 10, 14, 9 }
    };
}