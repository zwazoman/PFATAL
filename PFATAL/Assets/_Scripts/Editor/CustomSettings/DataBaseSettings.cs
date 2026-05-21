using UnityEditor;
using UnityEngine;

static class DataBaseUrlSettings
{
    public static string dataBaseUrl = dataBaseUrl;
}

static class DataBaseSettings
{
    public static string dataBaseUrl = "http://10.84.108.217:5000";

    [SettingsProvider]
    public static SettingsProvider CreateProvider()
    {
        var provider = new SettingsProvider(
            "Project/Custom Settings/Data Base Settings",
            SettingsScope.Project)
        {
            guiHandler = (searchContext) =>
            {
                dataBaseUrl = EditorGUILayout.TextField("Database URL", dataBaseUrl);
            }
        };

        return provider;
    }
}
