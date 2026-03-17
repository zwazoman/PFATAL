using UnityEngine;

[ExecuteAlways]
public class RaymarchRenderer : MonoBehaviour
{
    [Header("Asset References")]
    [SerializeField] Texture3D _densityField;


    [Header("SceneReferences")]
    [SerializeField] Transform _boundingBox;
    [SerializeField] MeshRenderer _MeshRenderer;

    void Awake()
    {
        UpdateMaterialValues();
    }

    void Update()
    {
        UpdateMaterialValues();
    }
    
    private void OnValidate()
    {
        UpdateMaterialValues();
    }

    void UpdateMaterialValues()
    {
        //_MeshRenderer.sharedMaterial.SetTexture("_densityField", _densityField); // pas propre si plusieurs renderers avec des textures / materials differents dans la scene
        _MeshRenderer.sharedMaterial.SetVector("_InvertResolution", new Vector3(1f/(float)_densityField.width, 1f / (float)_densityField.height, 1f / (float)_densityField.depth));
        Debug.Log(_boundingBox.position);
        _MeshRenderer.sharedMaterial.SetVector("_boundingBoxCenter", _boundingBox.position);
        _MeshRenderer.sharedMaterial.SetVector("_boundingBoxSize", _boundingBox.localScale);
        _MeshRenderer.sharedMaterial.SetInteger("_pixelsPerUnit", 10);
        
    }

    
}
