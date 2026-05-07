using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class HeatMapServerAnalitics : MonoBehaviour
{
    public static HeatMapServerAnalitics instance;

    private HeatMapData _theRealHeatMap;
    private float _timer;
    private string _filePath;

    public List<GameObject> Players;
    public List<HeatMapData> HeatMaps;
    public float interval = 0.1f;
    public int gridSize = 1;
    public MapBounds mapBoundsObject;

    public TextMeshProUGUI text;

    public bool show = false;
    public bool isPlaying = false;
    public bool isFixed = false;

    public event Action<byte[]> OnHeatMapSaved;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        if (File.Exists(Path.Combine(Application.persistentDataPath, "HeatMapFolder")))
        {
            Directory.CreateDirectory(Path.Combine(Application.persistentDataPath, "HeatMapFolder"));
        }

        _filePath = Path.Combine(Application.persistentDataPath, "HeatMapFolder", "heatmap_" + System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".bin");
    }

    public void RealHeatMapSave()
    {
        HeatMapData mapToSave = HeatMapUtility.CombineHeatMap(HeatMaps);

        OnHeatMapSaved?.Invoke(HeatMapUtility.ConvertMapToByte(mapToSave));

        File.WriteAllBytes(_filePath, HeatMapUtility.ConvertMapToByte(mapToSave));
    }

    /*private void Start()
    {
        _theRealHeatMap = new(gridSize, 0, 001, Players.Count);
        //(int)(1f / interval * 3 * 60);
        _theRealHeatMap.points = new List<HeatPoint>((int)(1f / interval * 3 * 60));

        GameManager.Instance.EventOnGameStarted += () => isPlaying = true;
        GameManager.Instance.EventOnGameEnded += (_) => SaveHeatMap(new());
    }

    private void Update()
    {
        if (!isFixed) return;

        if ((_timer += Time.deltaTime) >= interval)
        {
            _timer = 0f;

            foreach (GameObject player in Players)
            {
                Vector3 playerPos = player.transform.position;
                player.TryGetComponent(out PlayerCharacter playerCharacter);



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
                    switch (GetPlayerWeaponType(playerCharacter))
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
            

                    switch (GetPlayerWeaponType(playerCharacter))
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
        if (isPlaying)
            SaveHeatMap(new());
    }

    public void SaveHeatMap(GameRulesBase.GameResult gameResult)
    {
        //File.WriteAllText(_filePath, JsonUtility.ToJson(_theRealHeatMap));

        //to do : get heapMap from DB to set correct gameId and version

        _theRealHeatMap.playerCount = Players.Count;

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

    [Button("Oppen folder")]
    public void Copy()
    {
#if UNITY_EDITOR_WIN
        Process.Start(Application.persistentDataPath);
#endif
    }
    #endregion*/
}
