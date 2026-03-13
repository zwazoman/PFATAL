using System.Collections.Generic;
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

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.O))
            {
                TestRpc(TimeStamp.Now);
            }
        }

        [Rpc(SendTo.Everyone)]
        void TestRpc(float time)
        {
            Debug.LogError($" rpc took {TimeStamp.Now - time}s to be received");
        }

        //todo : mettre ça dans characterInputs
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

        [SerializeField] List<GameObject> _visualObjects;
        
        [Rpc(SendTo.Everyone)]
        public void HidePlayerRpc()
        {
            foreach (var obj in _visualObjects)
                obj.SetActive(false);
        }

        [Rpc(SendTo.Everyone)]
        public void ShowPlayerRpc()
        {
            foreach (var obj in _visualObjects)
                obj.SetActive(true);
        }
        
        //===============
    }

}
