using UnityEngine;
using Unity.Netcode;
using AYellowpaper.SerializedCollections;

public class ItemVisuals : MonoBehaviour
{
    [SerializeField] MeshFilter _meshFilter;
    [SerializeField] MeshRenderer _meshrender;

    [Header("Meshes")]

    [SerializedDictionary("Name","Mesh")]
    public SerializedDictionary<string, Mesh> itemMeshes = new();

    [Rpc(SendTo.ClientsAndHost)]
    public void ShowItemRpc(string meshName)
    {
        print("show mesh");
        _meshFilter.mesh = itemMeshes[meshName];
    }
}
