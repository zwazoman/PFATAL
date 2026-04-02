using _scripts.PlayerCharacter;
using FMOD.Studio;
using NaughtyAttributes;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class HeatMapServerAnalitics : MonoBehaviour
{
    public static HeatMapServerAnalitics instance;

    private HeatMapData _theRealHeatMap;
    private float _timer;
    private string _filePath;

    public List<GameObject> Players;
    public float interval = 0.1f;
    public int gridSize = 1;
    public MapBounds mapBoundsObject;

    public bool show = false;

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

        _filePath = Path.Combine(Application.persistentDataPath, "heatmap.json");
    }

    private void Start()
    {
        _theRealHeatMap = new(gridSize);
    }

    private void Update()
    {
        if ((_timer += Time.deltaTime) >= interval)
        {
            _timer = 0f;

            foreach (GameObject player in Players)
            {
                Vector3 playerPos = player.transform.position;
                player.TryGetComponent(out PlayerCharacter playerCharacter);



                if (!mapBoundsObject.m_Bounds.Contains(playerPos))
                {
                    UnityEngine.Debug.Log("y a pas de bounds connard");
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
            UnityEngine.Debug.Log("No player connard");

            return WeaponType.Without;
        }

        GameObject socket = player.TryGetComponent(out HeatMapAnalitycs analitycs) ? analitycs.playerWeaponSocket : null;

        if (socket == null) return WeaponType.Without;

        if (socket.transform.childCount == 0)
        {
            UnityEngine.Debug.Log("No weapon, return");
            return WeaponType.Without;
        }

        string itemName = socket.transform.GetChild(0).name;

        itemName = itemName.ToLower();

        UnityEngine.Debug.Log(itemName);

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

        UnityEngine.Debug.LogWarning("Unknown weapon type for item: " + itemName + ". En gros ça s'est chié dessus trèèèèèèèèèèèèèèèèèèèèèèèèèèèèèèèèèès très fort.");
        return WeaponType.Without;
    }

    private void OnApplicationQuit()
    {
        UnityEngine.Debug.Log("Application quitting, saving heatmap data...");
        SaveHeatMap();
    }

    public void SaveHeatMap()
    {
        File.WriteAllText(_filePath, JsonUtility.ToJson(_theRealHeatMap));
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
    #endregion
}
