using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(CommandSQL))]
public class CommandSQLDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return 95;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty name = property.FindPropertyRelative("Name");
        SerializedProperty command = property.FindPropertyRelative("Command");

        Rect r0 = new Rect(position.x, position.y, position.width, 18);
        Rect r1 = new Rect(position.x, position.y + 20, position.width, 40);
        Rect r2 = new Rect(position.x, position.y + 65, position.width, 20);

        name.stringValue = EditorGUI.TextField(r0, name.stringValue);
        command.stringValue = EditorGUI.TextArea(r1, command.stringValue);

        if (GUI.Button(r2, "Execute"))
        {
            SQLQuerryWindow.Instance.RunQuery(command.stringValue);
        }
    }
}