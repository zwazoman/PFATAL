using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SchemaViewWindow : EditorWindow
{
    private Dictionary<string, TableSchema> schema = new Dictionary<string, TableSchema>();
    private Dictionary<string, List<ForeignKey>> foreignKeys = new Dictionary<string, List<ForeignKey>>();
    private Vector2 scroll;
    private Dictionary<string, bool> foldouts = new Dictionary<string, bool>();

    public static void ShowSchema(string json)
    {
        var window = GetWindow<SchemaViewWindow>("DB Schema");

        var root = MiniJSON.Deserialize(json) as Dictionary<string, object>;
        if (root == null) return;

        // Parse tables
        window.schema.Clear();
        if (root.TryGetValue("tables", out var tablesRaw))
        {
            var tables = tablesRaw as Dictionary<string, object>;
            foreach (var kv in tables)
            {
                var cols = kv.Value as List<object>;
                var tableSchema = new TableSchema();
                foreach (var colRaw in cols)
                {
                    var col = colRaw as Dictionary<string, object>;
                    tableSchema.columns.Add(new ColumnInfo
                    {
                        name = col["name"]?.ToString(),
                        type = col["type"]?.ToString(),
                        notNull = col["notNull"]?.ToString() == "1",
                        primaryKey = col["primaryKey"]?.ToString() == "1",
                        autoIncrement = col["autoIncrement"]?.ToString() == "1",
                        unique = col["unique"]?.ToString() == "1"
                    });
                }
                window.schema[kv.Key] = tableSchema;
            }
        }

        // Parse foreign keys
        window.foreignKeys.Clear();
        if (root.TryGetValue("foreignKeys", out var fkRaw))
        {
            var fkTables = fkRaw as Dictionary<string, object>;
            foreach (var kv in fkTables)
            {
                var fkList = kv.Value as List<object>;
                var list = new List<ForeignKey>();
                foreach (var fkItem in fkList)
                {
                    var fk = fkItem as Dictionary<string, object>;
                    list.Add(new ForeignKey
                    {
                        from = fk["from"]?.ToString(),
                        table = fk["table"]?.ToString(),
                        to = fk["to"]?.ToString()
                    });
                }
                window.foreignKeys[kv.Key] = list;
            }
        }

        window.Repaint();
    }

    void OnGUI()
    {
        GUILayout.Label("Database Schema", EditorStyles.boldLabel);
        GUILayout.Space(5);

        if (schema == null || schema.Count == 0)
        {
            GUILayout.Label("No schema loaded.");
            return;
        }

        scroll = EditorGUILayout.BeginScrollView(scroll);

        foreach (var kv in schema)
        {
            string tableName = kv.Key;
            TableSchema tableSchema = kv.Value;

            if (!foldouts.ContainsKey(tableName))
                foldouts[tableName] = true;

            // Header de table
            Rect rect = EditorGUILayout.BeginVertical("box");
            EditorGUI.DrawRect(new Rect(rect.x, rect.y, rect.width, 22), new Color(0.2f, 0.35f, 0.55f));

            EditorGUILayout.BeginHorizontal();
            GUIStyle titleStyle = new GUIStyle(EditorStyles.foldout)
            {
                fontStyle = FontStyle.Bold,
                normal = { textColor = Color.white },
                onNormal = { textColor = Color.white }
            };

            foldouts[tableName] = EditorGUILayout.Foldout(foldouts[tableName], tableName, true, titleStyle);
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(2);

            if (foldouts[tableName])
            {
                // Header colonnes
                EditorGUILayout.BeginHorizontal("box");
                GUILayout.Label("Column", EditorStyles.miniLabel, GUILayout.Width(130));
                GUILayout.Label("Type", EditorStyles.miniLabel, GUILayout.Width(80));
                GUILayout.Label("NOT NULL", EditorStyles.miniLabel, GUILayout.Width(65));
                GUILayout.Label("PK", EditorStyles.miniLabel, GUILayout.Width(25));
                GUILayout.Label("AI", EditorStyles.miniLabel, GUILayout.Width(25));
                GUILayout.Label("Unique", EditorStyles.miniLabel, GUILayout.Width(50));
                GUILayout.Label("FK", EditorStyles.miniLabel, GUILayout.ExpandWidth(true));
                EditorGUILayout.EndHorizontal();

                foreach (var col in tableSchema.columns)
                {
                    // Cherche si cette colonne a une FK
                    string fkLabel = "";
                    if (foreignKeys.TryGetValue(tableName, out var fks))
                    {
                        var fk = fks.Find(f => f.from == col.name);
                        if (fk != null)
                            fkLabel = $"-> {fk.table}.{fk.to}";
                    }

                    Color rowColor = col.primaryKey
                        ? new Color(0.25f, 0.45f, 0.25f)
                        : new Color(0.22f, 0.22f, 0.22f);

                    Rect rowRect = EditorGUILayout.BeginHorizontal();
                    EditorGUI.DrawRect(rowRect, rowColor);

                    GUILayout.Label(col.name, GUILayout.Width(130));
                    GUILayout.Label(col.type, GUILayout.Width(80));
                    GUILayout.Label(col.notNull ? "X" : "", GUILayout.Width(65));
                    GUILayout.Label(col.primaryKey ? "X" : "", GUILayout.Width(25));
                    GUILayout.Label(col.autoIncrement ? "X" : "", GUILayout.Width(25));
                    GUILayout.Label(col.unique ? "X" : "", GUILayout.Width(50));
                    GUILayout.Label(fkLabel, GUILayout.ExpandWidth(true));

                    EditorGUILayout.EndHorizontal();
                }
            }

            EditorGUILayout.EndVertical();
            GUILayout.Space(5);
        }

        EditorGUILayout.EndScrollView();
    }
}

public class TableSchema
{
    public List<ColumnInfo> columns = new List<ColumnInfo>();
}

public class ColumnInfo
{
    public string name;
    public string type;
    public bool notNull;
    public bool primaryKey;
    public bool autoIncrement;
    public bool unique;
}

public class ForeignKey
{
    public string from;
    public string table;
    public string to;
}