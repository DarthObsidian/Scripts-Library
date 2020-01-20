using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexWorld : MonoBehaviour
{
    public Material mat;
    public HexBlockType[] blockTypes;

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

