using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _scripts.PlayerCharacter
{
    /// <summary>
    /// Contient toutes les refs de components du personnage
    /// </summary>
    public class PlayerCharacter : MonoBehaviour
    {
        [Header("Scene References")] 
        public PlayerPhysics physics;
        public PlayerMovement movement;
        public PlayerCharacterInputs inputs;
        public PlayerStateMachine stateMachine;
        public NetworkObject networkObject;

        [field: SerializeField]
        public Camera playerCamera { get; private set; }

        [field: SerializeField]
        public PlayerInput playerInput { get; private set; }

        [field: SerializeField]
        public PlayerInteraction playerInteraction { get; private set; }

        [field: SerializeField]
        public PlayerHands playerHands { get; private set; }


        public bool CheckActionmap(InputActionMap actionMap)
        {
            if (actionMap == playerInput.currentActionMap)
                return true;
            return false;
        }

        public void SwapActionMapToUI()
        {
            Cursor.lockState = CursorLockMode.Confined;

            playerInput.SwitchCurrentActionMap("UI");
        }

        public void SwapActionMapToPlayer()
        {
            Cursor.lockState = CursorLockMode.Locked;

            playerInput.SwitchCurrentActionMap("Player");
        }
    }
}
