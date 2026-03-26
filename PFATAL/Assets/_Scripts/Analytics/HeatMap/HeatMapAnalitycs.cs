using _scripts.PlayerCharacter;
using NaughtyAttributes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEngine;


public class HeatMapAnalitycs : MonoBehaviour
{
    private HeatMapData heatMapData;
    private float timer;
    private string filePath;

    [SerializeField] private PlayerCharacter character;

    public float UpdateInterval = 1f;
    public int gridSize = 1;
    public WeaponType weaponType;
    public Transform playerTransform;
    public MapBounds mapBoundsObject;

    [Header("Debug")]
    [SerializeField] private bool show = false;
    [SerializeField] private List<string> listeFile;
    [SerializeField] private List<string> listeHeatmapJson;

    private void Start()
    {
        filePath = Path.Combine(Application.persistentDataPath, "heatmap.json");
        LoadHeatMap();

        //GUIUtility.systemCopyBuffer = Application.persistentDataPath;

        if (character == null) return;

        character.playerHands.rightHand.OnItemDropped += ChangeWeaponType;
        character.playerHands.rightHand.OnItemSwapped += ChangeWeaponType;
        character.playerHands.rightHand.OnItemPickedUp += ChangeWeaponType;

        ChangeWeaponType();
    }

    private void Update()
    {
        //Adding point each time the time interval is reach
        if ((timer += Time.deltaTime) >= UpdateInterval)
        {
            timer = 0f;
            Vector3 playerPos = playerTransform.position;

            if (!mapBoundsObject.m_Bounds.Contains(playerPos))
            {
                return;
            }

            //search if point already exist in the list
            var point = heatMapData.points.FirstOrDefault(p =>
                p.x == Mathf.Round(playerPos.x / gridSize) * gridSize &&
                p.y == Mathf.Round(playerPos.y / gridSize) * gridSize &&
                p.z == Mathf.Round(playerPos.z / gridSize) * gridSize);

            //if already exist, we increment the vists number, else we create a new point
            if (point != null)
            {
                point.visitsGlobal++;
                switch (weaponType)
                {
                    case WeaponType.Without:
                        point.playerWithoutWeaponVisits++;
                        break;

                    case WeaponType.Hammer:
                        point.playerWithHammerVisits++;
                        break;
                    case WeaponType.Crossbow:
                        point.playerWithCrossbowVisits++;
                        break;
                    case WeaponType.Tomahawk:
                        point.playerWithTomahawkVisits++;
                        break;
                }
            }
            else
            {
                HeatPoint newPoint = new HeatPoint(
                    Mathf.Round(playerPos.x / gridSize) * gridSize,
                    Mathf.Round(playerPos.y / gridSize) * gridSize,
                    Mathf.Round(playerPos.z / gridSize) * gridSize);

                switch (weaponType)
                {
                    case WeaponType.Without:
                        newPoint.playerWithoutWeaponVisits++;
                        break;

                    case WeaponType.Hammer:
                        newPoint.playerWithHammerVisits++;
                        break;
                    case WeaponType.Crossbow:
                        newPoint.playerWithCrossbowVisits++;
                        break;
                    case WeaponType.Tomahawk:
                        newPoint.playerWithTomahawkVisits++;
                        break;
                }
                heatMapData.points.Add(newPoint);
            }

            File.WriteAllText(filePath, JsonUtility.ToJson(heatMapData));
        }

        //UnityEngine.Debug.Log($"Max Visits: {MaxVisits()}");
    }

    private void ChangeWeaponType(Item item)
    {
        ChangeWeaponType();
    }

    private void ChangeWeaponType()
    {
        UnityEngine.Debug.Log("Start changing type");

        if (character == null)
        {
            UnityEngine.Debug.LogError("ta race il est ou le player character");
        }

        if (character.playerHands.rightHand.equippedItem == null)
        {
            UnityEngine.Debug.Log("No weapon, return");
            weaponType = WeaponType.Without;
            return;
        }

        string itemName = character.playerHands.rightHand.equippedItem.name;

        itemName = itemName.ToLower();

        UnityEngine.Debug.Log(itemName);

        if (itemName.Contains("hammer"))
        {
            weaponType = WeaponType.Hammer;
        }
        else if (itemName.Contains("crossbow"))
        {
            weaponType = WeaponType.Crossbow;
        }
        else if (itemName.Contains("tomahawk"))
        {
            weaponType = WeaponType.Tomahawk;
        }
    }

    //to do, no need that later
    private HeatMapData LoadHeatMap()
    {
        heatMapData = File.Exists(filePath)
            ? JsonUtility.FromJson<HeatMapData>(File.ReadAllText(filePath))
            : new HeatMapData(gridSize);

        UnityEngine.Debug.Log(heatMapData);

        return heatMapData;
    }

    private void OnApplicationQuit()
    {
        File.WriteAllText(filePath, JsonUtility.ToJson(heatMapData));

        //to do : no save the file but send it to the DB
    }

    public void OnDrawGizmos()
    {

        if (heatMapData == null) return;


        if (!show) return;

        float size = heatMapData.heatMapCellSize;
        foreach (var point in heatMapData.points)
        {
            //Gizmos.color = Color.Lerp(Color.blue, Color.red, point.visits / MaxVisits());
            UnityEngine.Debug.Log(point.visitsGlobal / MaxVisits());
            Gizmos.color = Color.Lerp(Color.blue, Color.red, (float)point.visitsGlobal / 10);
            Gizmos.DrawWireCube(new Vector3(point.x, point.y, point.z), new Vector3(size, size, size));
        }
    }

    public int MaxVisits()
    {
        return heatMapData.points.Count > 0 ? heatMapData.points.Max(p => p.visitsGlobal) : 0;
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
}