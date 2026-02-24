using UnityEngine;
using Unity.Netcode;
using AYellowpaper.SerializedCollections;
using System;

public class ItemVisuals : NetworkBehaviour
{
    [Header("left Hand")]
    [SerializeField] MeshFilter _leftFilter;
    [SerializeField] MeshRenderer _leftRenderer;
    Vector3 _initialLeftPos;

    [Header("Right Hand")]
    [SerializeField] MeshFilter _rightFilter;
    [SerializeField] MeshRenderer _rightRenderer;
    Vector3 _initialRightPos;

    [Header("Meshes")]

    [SerializedDictionary("Name","Mesh")]
    public SerializedDictionary<string, ItemVisual> itemMeshes = new();

    private void Start()
    {
        _initialLeftPos = _leftRenderer.transform.localPosition;
        _initialRightPos = _rightRenderer.transform.localPosition;
    }

    [Rpc(SendTo.Everyone)]
    public void ShowItemRpc(string meshName, bool leftHand = true)
    {
        print($"show {meshName}");

        if (leftHand)
        {
            _leftFilter.mesh = itemMeshes[meshName].mesh;
            _leftFilter.transform.localPosition += itemMeshes[meshName].positionOffset;
            _rightFilter.transform.localRotation = Quaternion.Euler(itemMeshes[meshName].rotation.x, itemMeshes[meshName].rotation.y, itemMeshes[meshName].rotation.z);
            _leftFilter.transform.localScale *= itemMeshes[meshName].scaleMultiplyer;
        }
        else
        {
            _rightFilter.mesh = itemMeshes[meshName].mesh;
            _rightFilter.transform.localPosition += itemMeshes[meshName].positionOffset;
            _rightFilter.transform.localRotation = Quaternion.Euler(itemMeshes[meshName].rotation.x, itemMeshes[meshName].rotation.y, itemMeshes[meshName].rotation.z);
            _rightFilter.transform.localScale *= itemMeshes[meshName].scaleMultiplyer;
        }
    }

    [Rpc(SendTo.Everyone)]
    public void HideItemRpc(bool leftHand = true)
    {
        if (leftHand)
        {
            _leftFilter.mesh = null;
            _leftFilter.transform.localPosition = _initialLeftPos;
            _leftFilter.transform.localRotation = Quaternion.Euler(0f,0f,0f);
            _leftFilter.transform.localScale = Vector3.one;
        }
        else
        {
            _rightFilter.mesh = null;
            _rightFilter.transform.localPosition = _initialLeftPos;
            _rightFilter.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
            _rightFilter.transform.localScale = Vector3.one;
        }
    }

    //public void ShowItem(string meshName, bool leftHand = true)
    //{
    //    if (leftHand)
    //    {
    //        _leftFilter.mesh = itemMeshes[meshName].mesh;
    //        _leftFilter.transform.position += itemMeshes[meshName].positionOffset;
    //        _leftFilter.transform.rotation = Quaternion.Euler(itemMeshes[meshName].rotation);
    //        _leftFilter.transform.localScale *= itemMeshes[meshName].scaleMultiplyer;
    //        ShowItemLeftRpc(meshName);
    //    }
    //    else
    //    {
    //        _rightFilter.mesh = itemMeshes[meshName].mesh;
    //        _rightFilter.transform.position += itemMeshes[meshName].positionOffset;
    //        _rightFilter.transform.rotation = Quaternion.Euler(itemMeshes[meshName].rotation);
    //        _rightFilter.transform.localScale *= itemMeshes[meshName].scaleMultiplyer;
    //        ShowItemRightRpc(meshName);
    //    }
           
    //}

    //[Rpc(SendTo.NotMe)]
    //void ShowItemRightRpc(string meshName)
    //{
    //    print("show mesh");
    //    _rightFilter.mesh = itemMeshes[meshName].mesh;
    //}

    //[Rpc(SendTo.NotMe)]
    //void ShowItemLeftRpc(string meshName)
    //{
    //    print("show mesh");
    //    _leftFilter.mesh = itemMeshes[meshName].mesh;
    //}
}

[Serializable]
public class ItemVisual
{
    public Mesh mesh;
    public Vector3 positionOffset;
    public Vector3 rotation;
    public float scaleMultiplyer = 1;
}
