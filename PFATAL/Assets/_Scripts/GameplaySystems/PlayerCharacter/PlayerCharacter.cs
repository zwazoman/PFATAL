using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _scripts.PlayerCharacter
{
    /// <summary>
    /// Contient toutes les refs de components du personnage
    /// </summary>
    public class PlayerCharacter : NetworkBehaviour
    {
        [Header("Scene References")] 
        public PlayerPhysics physics;
        public PlayerMovement movement;
        public PlayerCharacterInputs inputs;
        public PlayerStateMachine stateMachine;
        public NetworkObject networkObject;
        public DamageableObject health;

        public HUDManager HUD;
        [field: SerializeField] public Camera playerCamera { get; private set; }
        [field: SerializeField] public PlayerCameraBehaviour cameraBehaviour { get; private set; }

        [field: SerializeField]
        public PlayerInput playerInput { get; private set; }

        [field: SerializeField]
        public PlayerInteraction playerInteraction { get; private set; }

        [field: SerializeField]
        public PlayerHands playerHands { get; private set; }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            gameObject.name = gameObject.name + NetworkBehaviourId + OwnerClientId;
        }

        //todo : mettre ça dans characterInputs==
        public bool CheckActionmap(InputActionMap actionMap)
        {
            if (actionMap == playerInput.currentActionMap)
                return true;
            return false;
        }

        public void SwapActionMapToUI()
        {
            if (!playerInput.enabled)
                return;

            Cursor.lockState = CursorLockMode.Confined;
            playerInput.SwitchCurrentActionMap("UI");
        }

        public void SwapActionMapToPlayer()
        {
            if (!playerInput.enabled)
                return;

            Cursor.lockState = CursorLockMode.Locked;
            playerInput.SwitchCurrentActionMap("Player");
        }
        //========
        
        //==todo : mettre ça dans PlayerCharacterVisuals==
        
        [Rpc(SendTo.Everyone)]
        public void HidePlayerRpc()
        {
            //cacher les visuels
        }

        [Rpc(SendTo.Everyone)]
        public void ShowPlayerRpc()
        {
            //montrer les visuels
        }
        
        //===============
    }
}
