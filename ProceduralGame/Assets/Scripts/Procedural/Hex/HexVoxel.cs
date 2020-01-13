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
        { 0, 12, 1 },
        { 1, 12, 2 },
        { 2, 12, 3 },
        { 3, 12, 4 },
        { 4, 12, 5 },
        { 5, 12, 0 },
        { 9, 13, 8 },
        { 8, 13, 7 },
        { 7, 13, 6 },
        { 6, 13, 11 },
        { 11, 13, 10 },
        { 10, 13, 9 }
    };
}