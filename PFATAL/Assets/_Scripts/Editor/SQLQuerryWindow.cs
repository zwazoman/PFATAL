using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

public class SQLQuerryWindow : EditorWindow
{
    public static SQLQuerryWindow Instance;

    private string baseURL = "http://localhost:5000";
    private Command commandData;

    public List<CommandSQL> CommandList = new List<CommandSQL>();

    Vector2 scroll;
    string search = "";

    [MenuItem("Window/SQLQuerryWindow")]
    public static void ShowWindow()
    {
        GetWindow<SQLQuerryWindow>("SQL Querry Window");
        ViewRequestWindow.ShowWindow();
    }

    private void OnEnable()
    {
        Instance = this;

        string path = "Assets/_Data/Command/Command.asset";

        commandData = AssetDatabase.LoadAssetAtPath<Command>(path);

        if (commandData != null)
        {
            for (int i = 0; i < commandData.CommandData.Count; i++)
            {
                CommandSQL cmd = commandData.CommandData[i];
                if (cmd != null)
                {
                    CommandList.Add(cmd);
                }
            }
        }
    }

    void OnGUI()
    {
        GUILayout.Label("Commandes SQL", EditorStyles.boldLabel);

        DrawToolbar();

        SerializedObject so = new SerializedObject(this);
        SerializedProperty list = so.FindProperty("CommandList");

        scroll = EditorGUILayout.BeginScrollView(scroll, GUILayout.ExpandHeight(true));

        for (int i = 0; i < list.arraySize; i++)
        {
            SerializedProperty element = list.GetArrayElementAtIndex(i);

            SerializedProperty name = element.FindPropertyRelative("Name");
            SerializedProperty command = element.FindPropertyRelative("Command");

            string lowerSearch = search.ToLower();

            if (!string.IsNullOrEmpty(search) &&
               !name.stringValue.ToLower().Contains(lowerSearch) &&
               !command.stringValue.ToLower().Contains(lowerSearch))
            {
                continue;
            }

            EditorGUILayout.PropertyField(element, true);
        }

        EditorGUILayout.EndScrollView();

        GUILayout.Space(5);

        if (GUILayout.Button("Add Command"))
        {
            CommandList.Add(new CommandSQL());
        }

        if (GUILayout.Button("Save Commands"))
        {
            commandData.CommandData = CommandList;
            EditorUtility.SetDirty(commandData);
            AssetDatabase.SaveAssets();
        }

        so.ApplyModifiedProperties();
    }

    void DrawToolbar()
    {
        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

        GUILayout.Label("Search:", GUILayout.Width(50));

        search = GUILayout.TextField(
            search,
            EditorStyles.toolbarTextField,
            GUILayout.ExpandWidth(true)
        );

        if (GUILayout.Button("Clear", EditorStyles.toolbarButton, GUILayout.Width(50)))
        {
            search = "";
        }

        if (GUILayout.Button("Sort A-Z", EditorStyles.toolbarButton, GUILayout.Width(70)))
        {
            CommandList.Sort((a, b) => string.Compare(a.Name, b.Name, StringComparison.Ordinal));
        }

        if (GUILayout.Button("Sort Z-A", EditorStyles.toolbarButton, GUILayout.Width(70)))
        {
            CommandList.Sort((a, b) => string.Compare(b.Name, a.Name, StringComparison.Ordinal));
        }

        EditorGUILayout.EndHorizontal();
    }

    /// <summary>
    /// Fonction pour exécuter une requête SQL en envoyant une requête HTTP à un serveur.
    /// </summary>
    /// <param name="command"></param>
    public async void RunQuery(string command, string name)
    {
        string encoded = UnityWebRequest.EscapeURL(command);
        string url = baseURL + "/query?cmd=" + encoded;

        UnityWebRequest request = UnityWebRequest.Get(url);

        var operation = request.SendWebRequest();

        while (!operation.isDone)
            await System.Threading.Tasks.Task.Yield();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string json = request.downloadHandler.text;

            Debug.Log("SQL RESULT : " + json);

            ViewRequestWindow.AddData(json, name);
        }
        else
        {
            Debug.LogError("SQL ERROR : " + request.error);
        }
    }
}
