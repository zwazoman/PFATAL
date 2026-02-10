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

    [Rpc(SendTo.Everyone)]
    public void ShowItemRpc(string meshName)
    {
        //faut trouver un moyen d'exposer le mesh par autre chose qu'un rpc. ça coute trop cher de le serializer
        //quoi que

        print("show mesh");
        _meshFilter.mesh = itemMeshes[meshName];
    }

    //[Rpc(SendTo.ClientsAndHost)]
    //public void SwapItemRpc(Mesh mesh)
    //{
    //    _meshFilter.sharedMesh = mesh;
    //}
}
