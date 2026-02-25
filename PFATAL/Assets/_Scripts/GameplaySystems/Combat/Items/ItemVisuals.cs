using UnityEngine;
using Unity.Netcode;
using AYellowpaper.SerializedCollections;
using System;

public class ItemVisuals : NetworkBehaviour
{
    [Header("left Hand")]
    [SerializeField] MeshFilter _leftFilter;
    Vector3 _initialLeftPos;

    [Header("Right Hand")]
    [SerializeField] MeshFilter _rightFilter;
    Vector3 _initialRightPos;

    [Header("Meshes")]

    [SerializedDictionary("Name", "Mesh")]
    public SerializedDictionary<string, ItemVisual> itemMeshes = new();

    private void Start()
    {
        _initialLeftPos = _leftFilter.transform.localPosition;
        _initialRightPos = _rightFilter.transform.localPosition;
    }

    [Rpc(SendTo.Everyone)]
    public void ShowItemRpc(string meshName, bool leftHand = true)
    {
        MeshFilter filter;

        if (leftHand)
            filter = _leftFilter;
        else
            filter = _rightFilter;

        filter.mesh = itemMeshes[meshName].mesh;
        filter.transform.localPosition += itemMeshes[meshName].positionOffset;
        filter.transform.localRotation = Quaternion.Euler(itemMeshes[meshName].rotation.x, itemMeshes[meshName].rotation.y, itemMeshes[meshName].rotation.z);
        filter.transform.localScale *= itemMeshes[meshName].scaleMultiplyer;
    }

    [Rpc(SendTo.Everyone)]
    public void HideItemRpc(bool leftHand = true)
    {
        MeshFilter filter;

        if (leftHand)
            filter = _leftFilter;
        else
            filter = _rightFilter;

        filter.mesh = null;
        filter.transform.localPosition = _initialLeftPos;
        filter.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
        filter.transform.localScale = Vector3.one;
    }
}

[Serializable]
public class ItemVisual
{
    public Mesh mesh;
    public Vector3 positionOffset;
    public Vector3 rotation;
    public float scaleMultiplyer = 1;
}
