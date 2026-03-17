using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new Command", menuName = "CommandSQL")]
public class Command : ScriptableObject
{
    public List<CommandSQL> CommandData;
}

[Serializable]
public class CommandSQL
{
    public string Name;

    [TextArea(2, 10)]
    public string Command;
}
