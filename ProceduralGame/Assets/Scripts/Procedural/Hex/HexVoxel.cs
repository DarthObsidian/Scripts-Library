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
        new Vector3(-innerRadius, 1f, 0.5f * outerRadius)
    };

    public static readonly int[,] hexSideTris =
    {
        { 1, 7, 0, 6 },
        { 2, 8, 1, 7 },
        { 3, 9, 2, 8 },
        { 4, 10, 3, 9 },
        { 5, 11, 4, 10 },
        { 0, 6, 5, 11 }
    };

    public static readonly int[,] hexTopTris =
    {
        { 10, 11, 9, 6, 8, 7 }, //top face
        { 4, 3, 5, 2, 0, 1 } //bottom face
    };

    public static readonly Vector2[] hexUvs =
    {
        new Vector2(0.0f, 0.0f),
        new Vector2(0.0f, 1.0f),
        new Vector2(1.0f, 0.0f),
        new Vector2(1.0f, 1.0f) 
    };
}