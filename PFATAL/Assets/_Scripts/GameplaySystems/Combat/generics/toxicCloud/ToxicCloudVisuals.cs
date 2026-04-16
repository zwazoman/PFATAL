using System;
using _scripts.PlayerCharacter;
using UnityEngine;

public class ToxicCloudVisuals : MonoBehaviour
{
    [Header("Scene references")]
    [SerializeField] private Transform _quad;
    [SerializeField] private ToxicCloud _cloud;

    void Start()
    {
        _quad.gameObject.SetActive(false);
    }
    
    private void Update()
    {
        Camera cam = PlayerCharacter.LocalPlayerCharacter.cameraBehaviour._cam;
        if ((transform.position - cam.transform.position).sqrMagnitude < _cloud.radius * _cloud.radius)
        {
            _quad.gameObject.SetActive(true);
            _quad.transform.position = cam.transform.position + cam.transform.forward * (cam.nearClipPlane * 1.1f);
            _quad.transform.rotation = cam.transform.rotation;
        }
        else 
            _quad.gameObject.SetActive(false);
    }
}
