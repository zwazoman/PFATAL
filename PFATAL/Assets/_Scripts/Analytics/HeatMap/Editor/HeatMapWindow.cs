using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class HeatMapWindow : EditorWindow
{
    //heatMap loading parameters
    string fileStartText = "heatmap_";
    bool isFileOnThisComputer = true;
    bool heatMapHasFilters = false;
    bool texture3DHasFilters = false;
    string fileNameToSave;
    string fileNameForTexture3D;
    static GameObject mapBound;
    static Material rayMarchingMat;

    //heatMap generaton pparameters
    int gameVersion;
    int gameId;
    int playerNumber;

    //Texture3D generation parameters
    WeaponType weaponType;
    string attenuation;

    [MenuItem("Window/HeatMap")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(HeatMapWindow));
        mapBound = FindFirstObjectByType<MapBounds>()?.gameObject;
        rayMarchingMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/_Graph/Surfaces/Materials/Raymarching/Mat_Texture3DVisualiizer.mat");
    }

    private void OnGUI()
    {
        GUILayout.Label("Laissez la souris 3 secondes sur un élément pour voir les détails", EditorStyles.boldLabel);

        GUILayout.Space(17);

        GUILayout.Label("Base Settings", EditorStyles.boldLabel);

        mapBound = (GameObject)EditorGUILayout.ObjectField(
            new GUIContent(
                "Map Bounds",
                "The limits of the map, must be an object with MapBounds component, one should be in the game scene already. Bounds value define the area where positions can be saved."),
            mapBound,
            typeof(GameObject),
            true);
        if (mapBound != null)
        {
            GUILayout.Label("Objet sélectionné : " + mapBound.name);
        }


        rayMarchingMat = (Material)EditorGUILayout.ObjectField(
            new GUIContent(
                "Ray Marching Material",
                "Material used for ray marching. Normally called : Mat_Texture3D_visalizer"),
            rayMarchingMat,
            typeof(Material),
            false);
        if (rayMarchingMat != null)
        {
            EditorGUILayout.LabelField("Shader:", rayMarchingMat.shader.name);
        }


        GUILayout.Space(25);


        //Filters options for the heapmap
        GUILayout.Label("HeatMaps Filter", EditorStyles.boldLabel);
        fileStartText = EditorGUILayout.TextField(
            new GUIContent(
                "File start name",
                "Start of the files name use to generate the heatmap."),
            fileStartText);
        isFileOnThisComputer = EditorGUILayout.Toggle(
            new GUIContent(
                "Is Files On This Computer",
                "Check true if the file used to generate the heatmap are in the persistent data path on this computer."),
            isFileOnThisComputer);
        fileNameToSave = EditorGUILayout.TextField(
            new GUIContent(
                "New Heatmap File",
                "Name for the heatmap that will be generate."),
            fileNameToSave);

        heatMapHasFilters = EditorGUILayout.BeginToggleGroup("Apply heatMap Filters", heatMapHasFilters);
        gameVersion = EditorGUILayout.IntField("Game Version", gameVersion);
        gameId = EditorGUILayout.IntField("Game Id", gameId);
        playerNumber = EditorGUILayout.IntField("Player Number", playerNumber);
        EditorGUILayout.EndToggleGroup();
        EditorGUILayout.Space(10);

        if (GUILayout.Button("Generate Heat Map"))
        {
            GenerateHeatMap();
        }

        GUILayout.Space(25);

        //Filters options for the textre 3D
        GUILayout.Label("Texture3D Data To Analyse Parameters (W.I.P.)", EditorStyles.boldLabel);
        fileNameForTexture3D = EditorGUILayout.TextField(
            new GUIContent(
                "File Name For Texture3D",
                "File name use to generate the texture3D, locate ine the folder HeatMapFolder in the persistent data path"),
            fileNameForTexture3D);

        attenuation = EditorGUILayout.TextField(
            new GUIContent(
                "Visits attenuation",
                "Value that describe what's the max visits number. Write 'max' for the visits max of the heatmap."
                ),
            attenuation);

        texture3DHasFilters = EditorGUILayout.BeginToggleGroup(
            new GUIContent(
                "Apply texture3D Filters",
                "Apply or not filters for the creation of the texture3D"
                ),
            texture3DHasFilters);

        weaponType = (WeaponType)EditorGUILayout.EnumPopup(
            new GUIContent(
                "Weapon Type",
                "Weapon use by players"), weaponType);

        EditorGUILayout.EndToggleGroup();

        EditorGUILayout.Space(10);
        if (GUILayout.Button("Generate Texture3D"))
        {
            GenerateTexture3D();
        }

        EditorGUILayout.Space(25);

        if (GUILayout.Button("Open Persistant Data Path Folder"))
        {
            //GUIUtility.systemCopyBuffer = Application.persistentDataPath;
            //EditorUtility.RevealInFinder(Application.persistentDataPath);
#if UNITY_EDITOR_WIN
            Process.Start(Application.persistentDataPath);
#endif
        }

        if (GUILayout.Button("Open script"))
        {
            string scriptPath = AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(this));
            AssetDatabase.OpenAsset(AssetDatabase.LoadAssetAtPath<MonoScript>(scriptPath));
        }
    }

    private void GenerateHeatMap()
    {
        List<HeatMapData> allMaps = new List<HeatMapData>();
        List<HeatMapData> finalMapsToCombine = new List<HeatMapData>();

        //foce to put a file name
        if (fileNameToSave == "")
        {
            UnityEngine.Debug.LogWarning("No file name, AutoGeneratedHeatMap was put by default");
            fileNameToSave = "AutoGeneratedHeatMap";
        }

        //searching files on this computer in the persistent data path
        if (isFileOnThisComputer)
        {
            UnityEngine.Debug.Log("Loading heat map data from a file on this computer.");

            //Get all the json files that start with the designated text
            List<string> heatMapJsonList = Directory.GetFiles(Path.Combine(Application.persistentDataPath, "HeatMapFolder"), $"{fileStartText}*.bin").ToList(); //fileStartText + "*.json"

            //convert the json file to HeatMapData class
            foreach (string file in heatMapJsonList)
                allMaps.Add(HeatMapUtility.ConvertByteToMap(File.ReadAllBytes(file)));

            //checking if there's sorting conditions, if yes check all HeatMapData to find which accord to the conditions
            if (heatMapHasFilters && (gameVersion != 0 || playerNumber != 0))
            {
                //UnityEngine.Debug.Log("Start filtering.");
                List<HeatMapData> filteredMaps = new List<HeatMapData>();
                int i = 0;
                foreach (var map in allMaps)
                {
                    i++;
                    //UnityEngine.Debug.Log(
                    //    $"Iterating HeatMapData list, list number : {i}, GameVersion : {map.heatMapGameVersion} {gameVersion == "0" || gameVersion == map.heatMapGameVersion}" +
                    //    $", Player Number : {map.heatMapPlayerNumber} {playerNumber == 0 || playerNumber == map.heatMapPlayerNumber}.");
                    if (!(gameVersion == 0 || gameVersion == map.gameVersion))
                    {
                        //UnityEngine.Debug.Log($"Condition gameVersion : {gameVersion == "0" || gameVersion == map.heatMapGameVersion}");
                        continue;
                    }
                    if (!(playerNumber == 0 || playerNumber == map.playerId))
                    {
                        //UnityEngine.Debug.Log($"Condition playerNumber : {playerNumber == 0 || playerNumber == map.heatMapPlayerNumber}");
                        continue;
                    }
                    if (!(gameId == 0 || gameId == map.gameId))
                    {
                        //UnityEngine.Debug.Log($"Condition game id : {gameId == 0 || gameId == map.heatMapGameId}");
                        continue;
                    }

                    filteredMaps.Add(map);
                }
                finalMapsToCombine = filteredMaps;
            }
            else
            {
                finalMapsToCombine = allMaps;
            }
        }
        else
        {
            UnityEngine.Debug.Log("Loading heat map data from a remote source.");

            // to do Connard
            // Load the heat map data from a remote source or another location
            // Example: heatPoints = LoadHeatMapDataFromRemoteSource(fileStartText);
        }

        //check if list empty, if yes that means no HeatMap was matching the conditions
        if (finalMapsToCombine.Count == 0)
        {
            UnityEngine.Debug.Log("No matching heatMap for the selected parameters.");
            return;
        }

        //UnityEngine.Debug.Log($"Create/Editing file");
        //UnityEngine.Debug.Log(finalMapsToCombine.Count);

        //Save the a new file with all the HeatMapCombined
        HeatMapData combinedHeatMap = HeatMapUtility.CombineHeatMap(finalMapsToCombine);

        File.WriteAllBytes(Application.persistentDataPath + "/HeatMapFolder/" + fileNameToSave.Replace(" ", "_") + ".bin", HeatMapUtility.ConvertMapToByte(combinedHeatMap));
    }

    private void GenerateTexture3D()
    {
        //UnityEngine.Debug.Log(File.Exists(Path.Combine(Application.persistentDataPath + "/HeatMapFolder/" + fileNameForTexture3D)));

        //check if mapbounds is not null
        if (!mapBound.TryGetComponent(out MapBounds bounds))
        {
            UnityEngine.Debug.LogError("mapBounds object has no MapBounds on it");
            return;
        }
        //check for existing heatmap files, auto generate one with the actual parameters if no files exist
        if (fileNameForTexture3D == null || fileNameForTexture3D == "")
        {
            return;
            UnityEngine.Debug.LogWarning("Name of file to search is empty, creating new file with name : AutoGeneratedHeatmap");
            fileNameToSave = "AutogeneratedHeatmap";
            GenerateHeatMap();
            fileNameForTexture3D = "AutoGeneratedHeatmap.json";
        }
        else if (File.Exists(Path.Combine(Application.persistentDataPath + "/HeatMapFolder/" + fileNameForTexture3D)))
        {
            UnityEngine.Debug.Log("File founded, creating texture3D");
            //GenerateHeatMap();
        }
        else
        {
            UnityEngine.Debug.LogError("Y a un truc qui s'est chié dessus très très fort");
            return;
        }   

        string textureName = null;
        Vector3 boundsSize = bounds.m_Bounds.size;

        UnityEngine.Debug.Log(boundsSize);
        
        //get the heatmap
        HeatMapData baseHeatMapUseToGenerate =
            HeatMapUtility.ConvertByteToMap(File.ReadAllBytes(Path.Combine(Application.persistentDataPath + "/HeatMapFolder/" + fileNameForTexture3D)));

        //get the size of the heatMap
        int size = baseHeatMapUseToGenerate.cellSize;

        //set up textureSize, divide it by box Size
        Texture3D texture3D = new((int)boundsSize.x, (int)boundsSize.y, (int)boundsSize.z, TextureFormat.RFloat, false);
        texture3D.wrapMode = TextureWrapMode.Clamp;
        texture3D.filterMode = FilterMode.Point;
        texture3D.anisoLevel = 1;
        
        Color[] colors = new Color[(int)boundsSize.x * (int)boundsSize.y * (int)boundsSize.z];

        for (int i = 0; i < colors.Length; i++)
        {
            colors[i] = Color.black;
        }
        texture3D.SetPixels(colors);


        //convert the targeted heatmap to a heatmap at a size of one, that avoid having texture3D with aberrant storage size
        HeatMapData heatMapAtCellSizeOfOne = new(1);
        foreach (HeatPoint point in baseHeatMapUseToGenerate.points)
        {
            HeatPoint newPoint = new HeatPoint(new List<int>
                    {
                        (point.P[0] != 0 ? point.P[0] / size : 0),
                        (point.P[1] != 0 ? point.P[1] / size : 0),
                        (point.P[2] != 0 ? point.P[2] / size : 0),
                    },
                    //point.G,
                    point.W,
                    point.H,
                    point.C,
                    point.T
            );

            /*UnityEngine.Debug.Log($"Point X : {point.x}, Y : {point.y}, Z : {point.z}," +
                $" Global : {point.visitsGlobal}, " +
                $"Without : {point.playerWithoutWeaponVisits}, " +
                $"Hammer : {point.playerWithHammerVisits}, " +
                $"Crossbow : {point.playerWithCrossbowVisits}, " +
                $"Tomahawk : {point.playerWithTomahawkVisits}");

            UnityEngine.Debug.Log($"New Point X : {newPoint.x}, Y : {newPoint.y}, Z : {newPoint.z}, " +
                $"Global : {newPoint.visitsGlobal}, " +
                $"Without : {newPoint.playerWithoutWeaponVisits}, " +
                $"Hammer : {newPoint.playerWithHammerVisits}, " +
                $"Crossbow : {newPoint.playerWithCrossbowVisits}, " +
                $"Tomahawk : {newPoint.playerWithTomahawkVisits}");*/


            heatMapAtCellSizeOfOne.points.Add(newPoint);
        }

        int maxGlobal = HeatMapUtility.MaxVisits(heatMapAtCellSizeOfOne, WeaponType.All);
        int maxWithout = HeatMapUtility.MaxVisits(heatMapAtCellSizeOfOne, WeaponType.Without);
        int maxHammer = HeatMapUtility.MaxVisits(heatMapAtCellSizeOfOne, WeaponType.Hammer);
        int maxCrossbow = HeatMapUtility.MaxVisits(heatMapAtCellSizeOfOne, WeaponType.Crossbow);
        int maxTomahawk = HeatMapUtility.MaxVisits(heatMapAtCellSizeOfOne, WeaponType.Tomahawk);

        //set pixel colors for each position in the heatmap
        foreach (HeatPoint point in heatMapAtCellSizeOfOne.points)
        {
            Color pixelColor = new(0, 0, 0, 0);

            int attenuationInt = 1;

            attenuation = attenuation.ToLower();
            if (attenuation == "max")
                attenuationInt = HeatMapUtility.MaxVisits(heatMapAtCellSizeOfOne, texture3DHasFilters ? weaponType : WeaponType.All);
            else
                int.TryParse(attenuation, out attenuationInt);


            if (!texture3DHasFilters)
            {
                pixelColor = Color.Lerp(Color.black, Color.white, (float)point.GetGlobalVisits() / attenuationInt);
                textureName = "AllPlayerType";
            }
            else
            {
                switch (weaponType)
                {
                    case WeaponType.All:
                        UnityEngine.Debug.LogWarning("Tu t'es chié dessus frérot mais tkt ça marche quand même");
                        pixelColor = Color.Lerp(Color.black, Color.white, (float)point.GetGlobalVisits() / attenuationInt);
                        textureName = "AllPlayerType";
                        break;
                    case WeaponType.Without:
                        pixelColor = Color.Lerp(Color.black, Color.white, (float)point.W / attenuationInt);
                        textureName = "NoWeapons";
                        break;
                    case WeaponType.Hammer:
                        pixelColor = Color.Lerp(Color.black, Color.white, (float)point.H / attenuationInt);
                        textureName = "HammerPlayer";
                        break;
                    case WeaponType.Crossbow:
                        pixelColor = Color.Lerp(Color.black, Color.white, (float)point.C / attenuationInt);
                        textureName = "CrossbowPlayer";
                        break;
                    case WeaponType.Tomahawk:
                        pixelColor = Color.Lerp(Color.black, Color.white, (float)point.T / attenuationInt);
                        textureName = "TomahawkPlayer";
                        break;
                }
            }

            //UnityEngine.Debug.Log($"{Color.Lerp(Color.black, Color.white, (float)point.visitsGlobal / 6)}, {(float)point.visitsGlobal / 6}");

            //Adding offset base on half the bounds size, so all position modify a positive value, cause pixels texture3D only have positive value,
            //plus adding an offset to have the center of the map as the center of the texture3D

            /*texture3D.SetPixel(
                    ((int)point.x + (int)boundsSize.x / 2) - (int)bounds.m_Bounds.center.x -1,
                    ((int)point.y + (int)boundsSize.y / 2) - (int)bounds.m_Bounds.center.y,
                    ((int)point.z + (int)boundsSize.z / 2) - (int)bounds.m_Bounds.center.z -1,
                    pixelColor);*/


            Vector3 boxMin = bounds.m_Bounds.center - boundsSize / 2f;
            Vector3 size2 = boundsSize;

            int x1 = Mathf.FloorToInt((point.P[0] - boxMin.x) / size2.x * texture3D.width) ;
            int y1 = Mathf.FloorToInt((point.P[1] - boxMin.y) / size2.y * texture3D.height);
            int z1 = Mathf.FloorToInt((point.P[2] - boxMin.z) / size2.z * texture3D.depth) ;

            int x = Mathf.Clamp(x1, 0, texture3D.width - 1) ;
            int y = Mathf.Clamp(y1, 0, texture3D.height - 1);
            int z = Mathf.Clamp(z1, 0, texture3D.depth - 1) ;

            texture3D.SetPixel(x, y, z, pixelColor);

            /*UnityEngine.Debug.Log($"Point : {point.x}, {point.y}, {point.z}, texture coordinate : {x1}, {y1}, {z1} : {x}, {y}, {z}, ancient calcul : " +
                $"{((int)point.x + (int)boundsSize.x / 2) - (int)bounds.m_Bounds.center.x}, " +
                $"{((int)point.y + (int)boundsSize.y / 2) - (int)bounds.m_Bounds.center.y}, " +
                $"{((int)point.z + (int)boundsSize.z / 2) - (int)bounds.m_Bounds.center.z - 1}");*/

        }

        texture3D.Apply(false);

        //save json file of the heatmap size one, used to debug
        File.WriteAllText(Path.Combine(Application.persistentDataPath + "/HeatMapFolder/heatMapAtCellSizeOne.json"), HeatMapUtility.ConvertHeatMapDataToJson(heatMapAtCellSizeOfOne));

        //check if texture 
        if (textureName == null)
        {
            UnityEngine.Debug.LogError("The textureName is null, which means the texture 3D has not been updated.");
            return;
        }



        string path = "Assets/_Data/Texture3D/Texture3D_" + textureName + ".asset";

        Texture3D existing = AssetDatabase.LoadAssetAtPath<Texture3D>(path);

        if (existing == null)
        {
            AssetDatabase.CreateAsset(texture3D, path);
            existing.name = "Texture3D_" + textureName;
            existing = AssetDatabase.LoadAssetAtPath<Texture3D>(path);
            existing.anisoLevel = 1;
        }
        else
        {
            EditorUtility.CopySerialized(texture3D, existing);
            existing.name = "Texture3D_" + textureName;
            existing.anisoLevel = 1;
            Object.DestroyImmediate(texture3D);
        }

        rayMarchingMat.SetTexture("_densityField", existing);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}