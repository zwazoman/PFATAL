using _scripts.PlayerCharacter;
using NaughtyAttributes;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.Netcode;
using UnityEngine;


public class HeatMapAnalitycs : NetworkBehaviour
{
    [SerializeField] private PlayerCharacter character;

    public GameObject playerWeaponSocket;

    public bool isOnCapsule = false;

    /*private void Start()
    {
        if (isOnCapsule)
        {
            if (!character.IsServer) return;
        }

        //HeatMapServerAnalitics.instance.Players.Add(gameObject);
    }*/

    private HeatMapData _theRealHeatMap;
    private float _timer;
    private string _filePath;

    public List<GameObject> Players;
    public List<HeatMapData> heatMapsToCombine;
    public float interval = 0.1f;
    public int gridSize = 1;
    public MapBounds mapBoundsObject;

    public bool show = false;
    public bool isPlaying = false;
    public bool isFixed = false;
    public List<int> tktPllayer;

    private void Awake()
    {


        _filePath = Path.Combine(Application.persistentDataPath, "heatmap_" + System.DateTime.Now.ToString("yyyyMMddHHmmss") + ".bin");
    }

    private void Start()
    {
        TryGetComponent(out character);

        if (isOnCapsule)
        {
            if (!character.IsServer) return;
        }

        mapBoundsObject = MapBounds.instance;

        HeatMapServerAnalitics.instance.Players.Add(gameObject);

        _theRealHeatMap = new(gridSize, 0, 001, character.name[character.name.Length - 1]);
        //(int)(1f / interval * 3 * 60);
        _theRealHeatMap.points = new List<HeatPoint>((int)(1f / interval * 3 * 60));

        GameManager.Instance.EventOnGameStarted += () => isPlaying = true;
        GameManager.Instance.EventOnGameEnded += (_) => SaveHeatMap(new());

    }

    private void Update()
    {
        //if (!isFixed) return;

        if ((_timer += Time.deltaTime) >= interval)
        {
            _timer = 0f;

            Vector3 playerPos = character.transform.position;

            if (!mapBoundsObject.m_Bounds.Contains(playerPos))
            {
                //UnityEngine.Debug.Log("y a pas de bounds connard");
                return;
            }

            //search if point already exist in the list
            var point = _theRealHeatMap.points.FirstOrDefault(p =>
                p.P[0] == Mathf.Round(playerPos.x / gridSize) * gridSize &&
                p.P[1] == Mathf.Round(playerPos.y / gridSize) * gridSize &&
                p.P[2] == Mathf.Round(playerPos.z / gridSize) * gridSize);

            //if already exist, we increment the vists number, else we create a new point
            if (point != null)
            {
                //point.vG++;
                switch (GetPlayerWeaponType(character))
                {
                    case WeaponType.Without:
                        point.W++;
                        break;

                    case WeaponType.Hammer:
                        point.H++;
                        break;
                    case WeaponType.Crossbow:
                        point.C++;
                        break;
                    case WeaponType.Tomahawk:
                        point.T++;
                        break;
                }
            }
            else
            {
                HeatPoint newPoint = new HeatPoint(new List<int>()
                {
                    (int)Mathf.Round(playerPos.x / gridSize) * gridSize,
                    (int)Mathf.Round(playerPos.y / gridSize) * gridSize,
                    (int)Mathf.Round(playerPos.z / gridSize) * gridSize
                });


                switch (GetPlayerWeaponType(character))
                {
                    case WeaponType.Without:
                        newPoint.W++;
                        break;

                    case WeaponType.Hammer:
                        newPoint.H++;
                        break;
                    case WeaponType.Crossbow:
                        newPoint.C++;
                        break;
                    case WeaponType.Tomahawk:
                        newPoint.T++;
                        break;
                }
                _theRealHeatMap.points.Add(newPoint);
            }
        }

        if (character.IsOwner)
        {
            tktPllayer.Clear();

            HeatMapServerAnalitics.instance.text.text = heatMapsToCombine.Count.ToString();

            if (heatMapsToCombine.Count == 0) return;

            foreach (var heatmap in heatMapsToCombine)
            {
                tktPllayer.Add(heatmap.playerId);
            }
        }
    }

    private WeaponType GetPlayerWeaponType(PlayerCharacter player)
    {
        if (player == null)
        {
            //UnityEngine.Debug.Log("No player, connard");

            return WeaponType.Without;
        }

        GameObject socket = player.TryGetComponent(out HeatMapAnalitycs analitycs) ? analitycs.playerWeaponSocket : null;

        if (socket == null) return WeaponType.Without;

        if (socket.transform.childCount == 0)
        {
            //UnityEngine.Debug.Log("No weapon, return");
            return WeaponType.Without;
        }

        string itemName = socket.transform.GetChild(0).name;

        itemName = itemName.ToLower();

        //UnityEngine.Debug.Log(itemName);

        if (itemName.Contains("hammer"))
        {
            return WeaponType.Hammer;
        }
        else if (itemName.Contains("crossbow"))
        {
            return WeaponType.Crossbow;
        }
        else if (itemName.Contains("tomahawk"))
        {
            return WeaponType.Tomahawk;
        }

        //UnityEngine.Debug.LogWarning("Unknown weapon type for item: " + itemName + ". En gros ça s'est chié dessus trèèèèèèèèèèèèèèèèèèèèèèèèèèèèèèèèèès très fort.");
        return WeaponType.Without;
    }

    private void OnApplicationQuit()
    {
        //UnityEngine.Debug.Log("Application quitting, saving heatmap data...");
        if (!isPlaying) return;

        if (character.IsServer)
        {
            AskHeatmapToClientRpc();
            SaveHeatMap(new());
        }

        if (character.IsClient && character.IsOwner)
        {
            SendHeatMapToServerRpc(HeatMapUtility.ConvertMapToByte(_theRealHeatMap), character.name[character.name.Length - 1]);
        }
    }

    [Rpc(SendTo.Everyone)]
    private void AskHeatmapToClientRpc()
    {
        foreach (var player in HeatMapServerAnalitics.instance.Players)
        {
            if (player.TryGetComponent(out HeatMapAnalitycs analitycs))
            {
                //if (analitycs.character.IsOwner)
                //{
                    UnityEngine.Debug.Log("Ask Heatmap from client: " + analitycs.character.name[analitycs.character.name.Length - 1]);

                    analitycs.SendHeatMapToServerRpc(HeatMapUtility.ConvertMapToByte(analitycs._theRealHeatMap), analitycs.character.name[analitycs.character.name.Length - 1]);
                //}
            }
        }

        //SendHeatMapToServerRpc(_theRealHeatMap, character.name[character.name.Length - 1]);
    }

    [Rpc(SendTo.Server)]
    private void SendHeatMapToServerRpc(byte[] _clientHeatmapByte, char playerId)
    {
        if (!character.IsOwner) return;

        HeatMapData _clientHeatmap = HeatMapUtility.ConvertByteToMap(_clientHeatmapByte);

        UnityEngine.Debug.Log("Heatmap received from client: " + playerId);

        //_theRealHeatMap = HeatMapUtility.CombineHeatMap(new() {_theRealHeatMap, _clientHeatmap});

        HeatMapData heatmap = heatMapsToCombine.FirstOrDefault(h => h.playerId == _clientHeatmap.playerId);
        if (heatmap != null)
        {
            heatMapsToCombine.Remove(heatmap);
            heatMapsToCombine.Add(_clientHeatmap);
        }
        else
        {
            heatMapsToCombine.Add(_clientHeatmap);
        }

        
    }

    public void SaveHeatMap(GameRulesBase.GameResult gameResult)
    {
        if (character.IsClient && character.IsOwner)
        {
            SendHeatMapToServerRpc(HeatMapUtility.ConvertMapToByte(_theRealHeatMap), character.name[character.name.Length - 1]);
            Debug.Log("NTM la fin de game, j'envoie le heatmap au serveur");
        }

        if (!character.IsServer) return;

        //File.WriteAllText(_filePath, JsonUtility.ToJson(_theRealHeatMap));

        //to do : get heapMap from DB to set correct gameId and version

        _theRealHeatMap.playerId = Players.Count;

        byte[] _byteHeatmap = HeatMapUtility.ConvertMapToByte(_theRealHeatMap);

        File.WriteAllBytes(_filePath, _byteHeatmap);

        //UnityEngine.Debug.Log("Heatmap data saved to: " + _filePath);
    }

    #region Debug
    public void OnDrawGizmos()
    {

        if (_theRealHeatMap == null) return;


        if (!show) return;

        float size = _theRealHeatMap.cellSize;
        foreach (var point in _theRealHeatMap.points)
        {
            //Gizmos.color = Color.Lerp(Color.blue, Color.red, point.visits / MaxVisits());
            UnityEngine.Debug.Log(point.GetGlobalVisits() / HeatMapUtility.MaxVisits(_theRealHeatMap, WeaponType.All));
            Gizmos.color = Color.Lerp(Color.blue, Color.red, (float)point.GetGlobalVisits() / 10);
            Gizmos.DrawWireCube(new Vector3(point.P[0], point.P[1], point.P[2]), new Vector3(size, size, size));
        }
    }

    [Button("Delete Current json file")]
    public void DeleteHeatMap()
    {
        File.Delete(Path.Combine(Application.persistentDataPath, "heatmap.json"));
    }

    [Button("Test rpc")]
    public void TestRpc()
    {
        AskHeatmapToClientRpc();
    }
    #endregion
}