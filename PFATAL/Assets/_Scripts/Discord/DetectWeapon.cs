using _scripts.PlayerCharacter;
using UnityEngine;

public class DetectWeapon : MonoBehaviour
{
    [SerializeField] private PlayerCharacter _playerCharacter;
    [SerializeField] private Discord_Controller _controller;
    [SerializeField] private GameObject _weaponSocket;

    private void Start()
    {
        TryGetComponent(out _playerCharacter);
        _controller = Discord_Controller.Instance;
    }

    void Update()
    {
        if (_playerCharacter.IsOwner)
        {
            Debug.Log("checking weapon");

            if (_weaponSocket.transform.childCount == 0)
            {
                Debug.Log("no weapon");
                _controller.smallImageKey = "";
                _controller.smallText = "";
            }
            else if (_weaponSocket.transform.childCount > 0)
            {
                string weapon = _weaponSocket.transform.GetChild(0).gameObject.name.ToLower();
                if (weapon.Contains("hammer"))
                {
                    Debug.Log("hammer detected");

                    _controller.smallImageKey = "hammer";
                    _controller.smallText = "Hammer";
                }
                else if (weapon.Contains("crossbow"))
                {
                    Debug.Log("crossbow detected");

                    _controller.smallImageKey = "crossbow";
                    _controller.smallText = "Crossbow";
                }
                else if (weapon.Contains("tomahawk"))
                {
                    Debug.Log("tomahawk detected");

                    _controller.smallImageKey = "tomahawk";
                    _controller.smallText = "Tomahawk";
                }
            }
        }
    }
}
