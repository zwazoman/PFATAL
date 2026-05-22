using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public static class DetectNotStaticObjects
{
    [MenuItem("Tools/Detect Not Static Objects")]
    [System.Obsolete]
    public static void NotStaticObjects()
    {
        int notStaticCount = 0;

        GameObject[] allObjects = Object.FindObjectsOfType<GameObject>();
        List<GameObject> notStaticList = new();

        foreach (GameObject go in allObjects)
        {
            if (!go.isStatic)
            {
                notStaticList.Add(go);
                notStaticCount++;
            }
        }

        Debug.Log($"Nombre total d'objets non static : {notStaticList.Count}, {notStaticCount}");
    }
}
