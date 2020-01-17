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
        { 1, 7, 0, 6 },     //top right
        { 2, 8, 1, 7 },     //right
        { 3, 9, 2, 8 },     //bottom right
        { 4, 10, 3, 9 },    //bottom left
        { 5, 11, 4, 10 },   //left
        { 0, 6, 5, 11 }     //top left
    };

    public static readonly int[,] hexTopTris =
    {
        { 10, 11, 9, 6, 8, 7 }, //top face
        { 4, 3, 5, 2, 0, 1 }    //bottom face
    };

    public static readonly Vector4[] faceChecks =
    {
        //x,w,z,y
        new Vector4(0, -1, 1, 0),   //top right
        new Vector4(1, -1, 0, 0),   //right
        new Vector4(1, 0, -1, 0),   //bottom right
        new Vector4(0, 1, -1, 0),   //bottom left
        new Vector4(-1, 1, 0, 0),   //left
        new Vector4(-1, 0, 1, 0),   //top left
        new Vector4(0, 0, 0, 1),    //top
        new Vector4(0, 0, 0, -1),   //bottom
    };

    public static readonly Vector2[] hexUvs =
    {
        new Vector2(0.0f, 0.0f),
        new Vector2(0.0f, 1.0f),
        new Vector2(1.0f, 0.0f),
        new Vector2(1.0f, 1.0f) 
    };
}

[System.Serializable]
public struct HexCoordinates
{
    public int x {get; private set;}
    public int y {get; private set;}
    public int z {get; private set;}
    public int w {
        get
        {
            return -x -z;
        }
    }

    public HexCoordinates (int _x, int _y, int _z)
    {
        x = _x;
        y = _y;
        z = _z;
    }

    public static HexCoordinates FromOffsetCoordinates (int x, int y, int z)
    {
        return new HexCoordinates(x - z/2, y, z);
    }
}