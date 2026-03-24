using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ViewRequestWindow : EditorWindow
{
    public static ViewRequestWindow Instance;

    private List<TableData> tables = new List<TableData>();
    private Vector2 scroll;

    public static void ShowWindow()
    {
        Instance = GetWindow<ViewRequestWindow>("View Request Window");
    }

    public static void AddData(string json, string queryName)
    {
        ShowWindow();

        var parsed = MiniJSON.Deserialize(json) as List<object>;

        if (parsed == null)
        {
            Debug.LogError("JSON mal parsé");
            return;
        }

        var newTable = new TableData();
        newTable.queryName = queryName;

        foreach (var item in parsed)
        {
            var dict = item as Dictionary<string, object>;
            if (dict != null)
                newTable.rows.Add(dict);
        }

        if (newTable.rows.Count > 0)
            newTable.headers = new List<string>(newTable.rows[0].Keys);

        // Update si déjà existant
        var existing = Instance.tables.Find(t => t.queryName == queryName);
        if (existing != null)
        {
            existing.rows = newTable.rows;
            existing.headers = newTable.headers;
        }
        else
        {
            Instance.tables.Add(newTable);
        }

        Instance.Repaint();
    }

    void OnGUI()
    {
        GUILayout.Label("View Requests", EditorStyles.boldLabel);

        // Bouton Clear
        if (GUILayout.Button("Clear All"))
        {
            tables.Clear();
        }

        if (tables == null || tables.Count == 0)
        {
            GUILayout.Label("No data");
            return;
        }

        scroll = EditorGUILayout.BeginScrollView(scroll);

        int indexToRemove = -1;

        for (int i = 0; i < tables.Count; i++)
        {
            var table = tables[i];

            EditorGUILayout.BeginVertical("box");

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(table.queryName, EditorStyles.boldLabel);

            if (GUILayout.Button("Remove", GUILayout.Width(80)))
                indexToRemove = i;

            EditorGUILayout.EndHorizontal();

            DrawTable(table);

            EditorGUILayout.EndVertical();
        }

        if (indexToRemove != -1)
        {
            tables.RemoveAt(indexToRemove);
        }

        EditorGUILayout.EndScrollView();
    }

    void DrawTable(TableData table)
    {
        if (table.headers == null || table.headers.Count == 0)
            return;

        // Header
        EditorGUILayout.BeginHorizontal("box");
        foreach (var header in table.headers)
        {
            GUILayout.Label(header, EditorStyles.boldLabel, GUILayout.Width(120));
        }
        EditorGUILayout.EndHorizontal();

        // Rows
        foreach (var row in table.rows)
        {
            EditorGUILayout.BeginHorizontal("box");

            foreach (var header in table.headers)
            {
                row.TryGetValue(header, out object value);
                GUILayout.Label(value?.ToString() ?? "null", GUILayout.Width(120));
            }

            EditorGUILayout.EndHorizontal();
        }
    }
}

[Serializable]
public class TableData
{
    public string queryName;
    public List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
    public List<string> headers = new List<string>();
}