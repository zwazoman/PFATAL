using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static Unity.Collections.AllocatorManager;

public class SkinHandler : MonoBehaviour
{
    public event Action<int> OnSkinSwapped;

    [Header("References")]
    [SerializeField] SkinnedMeshRenderer localMeshRenderer;
    [SerializeField] SkinnedMeshRenderer outlineMeshRenderer;

    [SerializeField] SkinnedMeshRenderer leftHandRenderer;
    [SerializeField] SkinnedMeshRenderer rightHandRenderer;

    [SerializeField,Tooltip("0=classique, 1=noir, 2=jaune, 3=blanc, 4=rouge, 5=violet, 6=orange, 7=vert")] SkinData[] _skins;

    public void SwapSkin()
    {
        SwapSkin(PlayerPrefs.GetInt("skinID"));
    }

    /// <summary>
    /// appelé par les boutons de couleurs pour les skins (vont de 0 à 7)
    /// </summary>
    /// <param name="skinID"></param>
    public void SwapSkin(int skinID)
    {
        print($"skin swapped to {skinID}");

        OnSkinSwapped?.Invoke(skinID);

        SkinData skinData = _skins[skinID];
        
        //meshes
        localMeshRenderer.sharedMesh = skinData.mesh;
        if (outlineMeshRenderer != null)
            outlineMeshRenderer.sharedMesh = skinData.mesh;

        MaterialPropertyBlock propertyBlock = new();
        localMeshRenderer.GetPropertyBlock(propertyBlock);
        propertyBlock.Clear();

        // texture
        propertyBlock.SetVector("_hsv_contrast", skinData.hsv);
        //emmissive
        propertyBlock.SetVector("_EmissiveTint1", skinData.emissive);

        localMeshRenderer.SetPropertyBlock(propertyBlock);

        if(leftHandRenderer != null)
            leftHandRenderer.SetPropertyBlock(propertyBlock);
        if(rightHandRenderer != null) 
            rightHandRenderer.SetPropertyBlock(propertyBlock);

        PlayerPrefs.SetInt("skinID", skinID);
    }

    public int GetCurrentSkinID()
    {
        return PlayerPrefs.GetInt("skinID");
    }

    [Serializable]
    struct SkinData
    {
        public Mesh mesh;
        public Vector4 hsv;
        [ColorUsage(hdr: true, showAlpha:true)] public Color emissive;
    }
}


