using System.Collections.Generic;
using UnityEngine;

public class StartDisable : MonoBehaviour
{
    [SerializeField] List<GameObject> objectsToDisable;

    private void Start()
    {
        foreach (GameObject obj in objectsToDisable)
            obj.SetActive(false);

        gameObject.SetActive(false);
    }
}
