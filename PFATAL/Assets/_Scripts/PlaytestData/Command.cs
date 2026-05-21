using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new Command", menuName = "Scriptable Objects/CommandSQL")]
public class Command : ScriptableObject
{
    public List<CommandSQL> CommandData;
}

[Serializable]
public class CommandSQL
{
    public bool isOpen;
    public string Name;

    [TextArea(2, 10)]
    public string Command;
}
