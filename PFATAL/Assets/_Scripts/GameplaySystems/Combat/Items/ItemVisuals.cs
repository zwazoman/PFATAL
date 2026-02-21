using UnityEngine;
using Unity.Netcode;
using AYellowpaper.SerializedCollections;

public class ItemVisuals : NetworkBehaviour
{
    [SerializeField] MeshFilter _meshFilter;
    [SerializeField] MeshRenderer _meshrender;

    [Header("Meshes")]

    [SerializedDictionary("Name","Mesh")]
    public SerializedDictionary<string, Mesh> itemMeshes = new();

    public void ShowItem(string meshName)
    {
        print("show mesh");
        _meshFilter.mesh = itemMeshes[meshName];
        ShowItemRpc(meshName);
    }

    [Rpc(SendTo.NotMe)]
    void ShowItemRpc(string meshName)
    {
        print("show mesh");
        _meshFilter.mesh = itemMeshes[meshName];
    }
}
