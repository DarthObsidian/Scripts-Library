[System.Serializable]
public struct HexCoordinates
{
    public int x {get;}
    public int y {get;}
    public int z {get;}
    public int w => -x -z;

    public HexCoordinates (int _x, int _y, int _z)
    {
        x = _x;
        y = _y;
        z = _z;
    }

    //sets the hex coordinates based on standard xyz coords
    public static HexCoordinates FromOffsetCoordinates (int x, int y, int z)
    {
        return new HexCoordinates(x - z/2, y, z);
    }
    
    public static bool operator == (HexCoordinates a, HexCoordinates b)
    {
        return a.x == b.x && a.y == b.y && a.z == b.z && a.w == b.w;
    }

    public static bool operator != (HexCoordinates a, HexCoordinates b)
    {
        return !(a==b);
    }

    public static HexCoordinates operator + (HexCoordinates a, HexCoordinates b)
    {
        return new HexCoordinates(a.x + b.x, a.y + b.y, a.z + b.z);
    }

    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
            return false;
        var hexCoord = (HexCoordinates) obj;
        return (this == hexCoord);
    }

    public override int GetHashCode()
    {
        return x ^ y ^ z ^ w;
    }
}
