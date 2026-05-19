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
    public List<HeatMapData> heatMapsToCombine = new List<HeatMapData>();
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
        GameManager.Instance.EventOnGameEnded += (_) => SaveHeatMap(new(), false);

        if (!character.IsOwner)
        {
            this.enabled = false;
        }
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

                    case WeaponType.Sword:
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

                    case WeaponType.Sword:
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

        if (itemName.Contains("sword"))
        {
            return WeaponType.Sword;
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
            SaveHeatMap(new(), true);
        }

        if (character.IsClient && character.IsOwner)
        {
            SendHeatMapToServerRpc(HeatMapUtility.ConvertMapToByte(_theRealHeatMap), character.name[character.name.Length - 1]);
        }
    }

    [Rpc(SendTo.Everyone)]
    private void AskHeatmapToClientRpc()
    {
        //Debug.Log("Ask Heatmap to clients");

        foreach (var theCharacter in HeatMapServerAnalitics.instance.Players)
        {
            theCharacter.TryGetComponent(out HeatMapAnalitycs analitycs);

            //Debug.Log(theCharacter.name[theCharacter.name.Length - 1]);
            
            if (analitycs.character.IsOwner)
            {
                //Debug.Log("This client is owner " + analitycs.character.name[analitycs.character.name.Length - 1]);
                SendHeatMapToServerRpc(HeatMapUtility.ConvertMapToByte(analitycs._theRealHeatMap), analitycs.character.name[analitycs.character.name.Length - 1]);
            }
        }

        //SendHeatMapToServerRpc(_theRealHeatMap, character.name[character.name.Length - 1]);
    }

    [Rpc(SendTo.Server)]
    private void SendHeatMapToServerRpc(byte[] _clientHeatmapByte, char playerId)
    {
        //Debug.Log($"Client {playerId} sent heatmap to server");

        //if (!character.IsOwner) return;

        HeatMapData _clientHeatmap = HeatMapUtility.ConvertByteToMap(_clientHeatmapByte);
        heatMapsToCombine = HeatMapServerAnalitics.instance.HeatMaps;

        //UnityEngine.Debug.Log($"Heatmap received from client: {playerId}, Map id : {_clientHeatmap.playerId}, " +
        //    $"maps to combine count: {heatMapsToCombine.Count}, Script Owner : {this.gameObject.name}, Count : {_clientHeatmap.points.Count}");

        //File.WriteAllBytes(Path.Combine(Application.persistentDataPath, $"heatmap_received_from_player_{playerId}.bin"), _clientHeatmapByte);

        HeatMapData heatmap = heatMapsToCombine.FirstOrDefault(h => h.playerId == _clientHeatmap.playerId);
        if (heatmap != null)
        {
            //Debug.Log("Trouvé");
            heatMapsToCombine.Remove(heatmap);
            heatMapsToCombine.Add(_clientHeatmap);
        }
        else
        {
            //Debug.Log("pas trouvé");
            heatMapsToCombine.Add(_clientHeatmap);
        }
        
        HeatMapServerAnalitics.instance.HeatMaps = heatMapsToCombine;

        //Debug.Log($"Player : {HeatMapServerAnalitics.instance.Players.Count}, heatMap count : {heatMapsToCombine.Count}");

        if (heatMapsToCombine.Count == HeatMapServerAnalitics.instance.Players.Count)
        {
            _theRealHeatMap = HeatMapUtility.CombineHeatMap(new() {_theRealHeatMap, _clientHeatmap});
            //Debug.Log($"All heatmaps received, combining and saving... {HeatMapServerAnalitics.instance.Players.Count}");
            SaveHeatMap(new(), true);
        }
    }

    public void SaveHeatMap(GameRulesBase.GameResult gameResult, bool triggerByServer)
    {
        if (character.IsClient && character.IsOwner)
        {
            SendHeatMapToServerRpc(HeatMapUtility.ConvertMapToByte(_theRealHeatMap), character.name[character.name.Length - 1]);
            //Debug.Log("NTM la fin de game, j'envoie le heatmap au serveur");
        }

        if (character.IsServer && triggerByServer)
        {
            //File.WriteAllText(_filePath, JsonUtility.ToJson(_theRealHeatMap));

            //to do : get heapMap from DB to set correct gameId and version

            //HeatMapData map = HeatMapUtility.CombineHeatMap(heatMapsToCombine);

            byte[] _byteHeatmap = HeatMapUtility.ConvertMapToByte(_theRealHeatMap);

            //File.WriteAllBytes(_filePath, _byteHeatmap);
            HeatMapServerAnalitics.instance.RealHeatMapSave();

            //UnityEngine.Debug.Log("Heatmap data saved to: " + _filePath);
        }
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