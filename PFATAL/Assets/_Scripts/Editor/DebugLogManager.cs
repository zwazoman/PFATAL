using UnityEditor;
using UnityEngine;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;

public class DebugLogManager : EditorWindow
{
    private string path = "Assets/_Scripts";
    private List<DebugLogEntry> foundLogs = new();
    private Vector2 scroll;
    private ActionMode selectedAction = ActionMode.Comment;
    private FilterMode filterMode = FilterMode.All;
    private Filter filter = Filter.All;
    private string scriptNameFilter = "";

    private enum ActionMode { Comment, Uncomment, Delete }
    private enum FilterMode { All, Log, Warning, Error, Assertion, Exception, Print }
    private enum Filter { All, Uncommented, Commented }

    [MenuItem("Tools/Debug Log Manager")]
    public static void ShowWindow()
    {
        GetWindow<DebugLogManager>("Debug Log Manager");
    }

    void OnGUI()
    {
        GUILayout.Label("Scanner les scripts pour les Debug.Log", EditorStyles.boldLabel);
        path = EditorGUILayout.TextField("Dossier à scanner :", path);

        if (GUILayout.Button("Scanner les Debug.Log"))
        {
            foundLogs = ScanForDebugLogs(path);
        }

        if (foundLogs.Count > 0)
        {
            GUILayout.Space(10);
            EditorGUILayout.LabelField("Lignes trouvées :", EditorStyles.boldLabel);

            // Filtres
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Tout sélectionner"))
                foreach (var entry in GetFilteredLogs()) entry.selected = true;

            if (GUILayout.Button("Tout désélectionner"))
                foreach (var entry in GetFilteredLogs()) entry.selected = false;
            EditorGUILayout.EndHorizontal();

            scriptNameFilter = EditorGUILayout.TextField("Filtrer par script :", scriptNameFilter);

            EditorGUILayout.BeginHorizontal();
            filterMode = (FilterMode)EditorGUILayout.EnumPopup("Filtrer par type :", filterMode);
            filter = (Filter)EditorGUILayout.EnumPopup("Filtrer par état :", filter);
            EditorGUILayout.EndHorizontal();

            scroll = EditorGUILayout.BeginScrollView(scroll, GUILayout.Height(300));
            foreach (var entry in GetFilteredLogs())
            {
                EditorGUILayout.BeginHorizontal();

                entry.selected = EditorGUILayout.Toggle(entry.selected, GUILayout.Width(20));

                GUIStyle style = new GUIStyle(GUI.skin.label);
                style.richText = true;

                bool isCommented = entry.lineText.TrimStart().StartsWith("//");

                string baseColor = entry.logType switch
                {
                    "LogWarning" => "#FFA500",
                    "LogError" => "#FF0000",
                    "LogAssertion" => "#00FFFF",
                    "LogException" => "#FF00FF",
                    "print" => "#FFFF00",
                    _ => "#00FF00"
                };

                // Alpha
                string finalColor = isCommented ? baseColor + "88" : baseColor + "FF";

                string colorTag = $"<color={finalColor}>";

                if (GUILayout.Button($"{colorTag} {Path.GetFileName(entry.filePath)} (Ligne {entry.lineNumber + 1}) -> {Truncate(entry.lineText.Trim(), 60)}</color>", style))
                {
                    OpenScriptAtLine(entry.filePath, entry.lineNumber + 1);
                }

                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndScrollView();

            GUILayout.Space(10);

            EditorGUILayout.BeginHorizontal();
            selectedAction = (ActionMode)EditorGUILayout.EnumPopup("Action :", selectedAction);
            if (GUILayout.Button("Appliquer", GUILayout.Height(24)))
            {
                ApplyChanges(selectedAction.ToString().ToLower());
            }
            EditorGUILayout.EndHorizontal();
        }
    }

    class DebugLogEntry
    {
        public string filePath;
        public int lineNumber;
        public string lineText;
        public bool selected = false;
        public string logType; // Log / Warning / Error / Assertion / Exception / print
    }

    List<DebugLogEntry> ScanForDebugLogs(string directory)
    {
        var entries = new List<DebugLogEntry>();
        string[] files = Directory.GetFiles(directory, "*.cs", SearchOption.AllDirectories);
        Regex regex = new(@"^\s*(//\s*)?((Debug\.(LogWarning|LogError|Log|LogAssertion|LogException))|(print))\s*\(.*?\)\s*;", RegexOptions.Multiline);
        foreach (var file in files)
        {
            string[] lines = File.ReadAllLines(file);
            for (int i = 0; i < lines.Length; i++)
            {
                var match = regex.Match(lines[i]);
                if (match.Success)
                {
                    string type = match.Groups[4].Success ? match.Groups[4].Value : "print";

                    entries.Add(new DebugLogEntry
                    {
                        filePath = file,
                        lineNumber = i,
                        lineText = lines[i],
                        logType = type
                    });
                }
            }
        }

        return entries;
    }

    void ApplyChanges(string mode)
    {
        var filesToLines = new Dictionary<string, string[]>();

        foreach (var entry in foundLogs)
        {
            if (!entry.selected) continue;

            if (!filesToLines.ContainsKey(entry.filePath))
                filesToLines[entry.filePath] = File.ReadAllLines(entry.filePath);

            var lines = filesToLines[entry.filePath];

            string originalLine = lines[entry.lineNumber];
            string indentation = Regex.Match(originalLine, @"^\s*").Value;

            switch (mode)
            {
                case "comment":
                    if (!originalLine.TrimStart().StartsWith("//"))
                        lines[entry.lineNumber] = indentation + "// " + originalLine.TrimStart();
                    break;

                case "uncomment":
                    lines[entry.lineNumber] = Regex.Replace(originalLine, @"^(\s*)//\s?", "$1");
                    break;

                case "delete":
                    lines[entry.lineNumber] = "";
                    break;
            }
        }

        foreach (var pair in filesToLines)
        {
            File.WriteAllLines(pair.Key, pair.Value);
            Debug.Log($"Fichier mis à jour : {pair.Key}");
        }

        AssetDatabase.Refresh();
        foundLogs.Clear();
    }

    void OpenScriptAtLine(string filePath, int lineNumber)
    {
        var asset = AssetDatabase.LoadAssetAtPath<MonoScript>(filePath);
        if (asset != null)
        {
            AssetDatabase.OpenAsset(asset, lineNumber);
        }
        else
        {
            Debug.LogWarning("Impossible d’ouvrir le fichier : " + filePath);
        }
    }

    string Truncate(string text, int maxLength)
    {
        if (string.IsNullOrEmpty(text)) return "";
        return text.Length > maxLength ? text.Substring(0, maxLength) + "..." : text;
    }

    List<DebugLogEntry> GetFilteredLogs()
    {
        return foundLogs.Where(e =>
            (filterMode == FilterMode.All || e.logType == GetFilterTypeName(filterMode)) &&
            (filter == Filter.All ||
             (filter == Filter.Uncommented && !e.lineText.TrimStart().StartsWith("//")) ||
             (filter == Filter.Commented && e.lineText.TrimStart().StartsWith("//"))) &&
            (string.IsNullOrEmpty(scriptNameFilter) || Path.GetFileName(e.filePath).ToLower().Contains(scriptNameFilter.ToLower()))
        ).ToList();
    }

    string GetFilterTypeName(FilterMode mode)
    {
        return mode switch
        {
            FilterMode.Warning => "LogWarning",
            FilterMode.Error => "LogError",
            FilterMode.Assertion => "LogAssertion",
            FilterMode.Exception => "LogException",
            FilterMode.Print => "print",
            _ => "Log"
        };
    }
}
