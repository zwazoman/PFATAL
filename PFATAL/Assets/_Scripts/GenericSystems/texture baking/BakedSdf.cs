using UnityEngine;

[CreateAssetMenu(fileName = "newSdfData",menuName = "Raymarching/SdfData")]
public class BakedSdf : ScriptableObject
{
    public Bounds worldBounds;
    public Bounds localBounds;
    public Texture3D texture;

}
