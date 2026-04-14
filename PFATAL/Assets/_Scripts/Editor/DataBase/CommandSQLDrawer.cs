using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(CommandSQL))]
public class CommandSQLDrawer : PropertyDrawer
{
    private static Dictionary<string, Vector2> scrolls = new Dictionary<string, Vector2>();

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        SerializedProperty isOpen = property.FindPropertyRelative("isOpen");

        float baseHeight = 20;

        if (!isOpen.boolValue)
            return baseHeight + 5;

        float textHeight = 75;

        return baseHeight + 20 + textHeight + 25 + 10;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.DrawRect(position, new Color(0.18f, 0.18f, 0.18f));
        position = new Rect(position.x + 5, position.y + 5, position.width - 10, position.height - 10);

        SerializedProperty name = property.FindPropertyRelative("Name");
        SerializedProperty command = property.FindPropertyRelative("Command");

        float y = position.y;

        SerializedProperty isOpen = property.FindPropertyRelative("isOpen");

        Rect foldRect = new Rect(position.x, y, position.width, 18);
        isOpen.boolValue = EditorGUI.Foldout(foldRect, isOpen.boolValue, name.stringValue);
        y += 20;

        if (!isOpen.boolValue) return;

        // Nom
        Rect r0 = new Rect(position.x, y, position.width, 18);
        y += 20;

        // Zone scrollable
        float textHeight = 75;
        Rect r1 = new Rect(position.x, y, position.width, textHeight);
        y += textHeight + 5;

        // Bouton
        Rect r2 = new Rect(position.x, y, position.width, 20);

        name.stringValue = EditorGUI.TextField(r0, name.stringValue);

        GUIStyle style = new GUIStyle(EditorStyles.textArea);
        style.wordWrap = true;

        string key = property.propertyPath;
        if (!scrolls.ContainsKey(key))
            scrolls[key] = Vector2.zero;

        float contentHeight = style.CalcHeight(new GUIContent(command.stringValue), r1.width - 20);

        Rect viewRect = new Rect(0, 0, r1.width - 20, contentHeight);

        scrolls[key] = GUI.BeginScrollView(r1, scrolls[key], viewRect);

        command.stringValue = GUI.TextArea(
            new Rect(0, 0, viewRect.width, viewRect.height),
            command.stringValue,
            style
        );

        GUI.EndScrollView();

        Color old = GUI.backgroundColor;
        GUI.backgroundColor = new Color(0.3f, 0.8f, 0.3f);

        if (GUI.Button(r2, "Execute"))
        {
            SQLQuerryWindow.Instance.RunQuery(command.stringValue, name.stringValue);
        }

        GUI.backgroundColor = old;
    }
}
