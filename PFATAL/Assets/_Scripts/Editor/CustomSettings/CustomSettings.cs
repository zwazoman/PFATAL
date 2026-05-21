using UnityEditor;
using UnityEngine;

static class CustomSettings
{
    [SettingsProvider]
    public static SettingsProvider CreateProvider()
    {
        var provider = new SettingsProvider(
            "Project/Custom Settings",
            SettingsScope.Project)
        {
            guiHandler = (searchContext) =>
            {
                GUILayout.Label("This is a custom settings provider.", EditorStyles.boldLabel);
            }
        };

        return provider;
    }
}
