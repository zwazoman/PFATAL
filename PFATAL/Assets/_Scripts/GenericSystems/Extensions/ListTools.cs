using System.Collections.Generic;
using UnityEngine;

public static class ListTools
{
    public static T PickRandom<T>(this List<T> list)
    {
        return list[Random.Range(0, list.Count)];
    }

    public static T GetNextObjectWrapped<T>(this List<T> list, T currentObject)
    {
        int nextObjectIndex = list.IndexOf(currentObject) + 1;

        if (nextObjectIndex >= list.Count)
        {
            return list[0];
        }

        return list[nextObjectIndex];
    }

    public static T GetPreviousObjectWrapped<T>(this List<T> list, T currentObject)
    {
        int previousObjectIndex = list.IndexOf(currentObject) - 1;

        if (previousObjectIndex < 0)
        {
            return list[list.Count-1];
        }

        return list[previousObjectIndex];
    }

}
