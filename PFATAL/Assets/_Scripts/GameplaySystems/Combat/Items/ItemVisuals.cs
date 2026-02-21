using UnityEngine;
using Unity.Netcode;
using AYellowpaper.SerializedCollections;

public class ItemVisuals : NetworkBehaviour
{
    [Header("left Hand")]
    [SerializeField] MeshFilter _leftFilter;
    [SerializeField] MeshRenderer _leftRenderer;

    [Header("Right Hand")]
    [SerializeField] MeshFilter _rightFilter;
    [SerializeField] MeshRenderer _rightRenderer;

    [Header("Meshes")]

    [SerializedDictionary("Name","Mesh")]
    public SerializedDictionary<string, Mesh> itemMeshes = new();

    public void ShowItem(string meshName, bool leftHand = true)
    {
        if (leftHand)
        {
            _leftFilter.mesh = itemMeshes[meshName];
            ShowItemLeftRpc(meshName);
        }
        else
        {
            _rightFilter.mesh = itemMeshes[meshName];
            ShowItemRightRpc(meshName);
        }
           
    }

    [Rpc(SendTo.NotMe)]
    void ShowItemRightRpc(string meshName)
    {
        print("show mesh");
        _rightFilter.mesh = itemMeshes[meshName];
    }

    [Rpc(SendTo.NotMe)]
    void ShowItemLeftRpc(string meshName)
    {
        print("show mesh");
        _leftFilter.mesh = itemMeshes[meshName];
    }
}
