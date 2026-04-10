using FMODUnity;
using NetworkTime;
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
        [field: SerializeField] public PlayerPhysics physics  { get; private set; }
        [field: SerializeField] public PlayerMovement movement  { get; private set; }
        [field: SerializeField] public PlayerCharacterInputs inputs  { get; private set; }
        [field: SerializeField] public PlayerStateMachine stateMachine  { get; private set; }
        [field: SerializeField] public PlayerCharacterNetworkStateMachineCallback replicatedStateMachineCallbacks  { get; private set; }
        
        [field: SerializeField] public NetworkObject networkObject  { get; private set; }
        [field: SerializeField] public DamageableObject health  { get; private set; }
        [field: SerializeField] public HUDManager HUD  { get; private set; }
        [field: SerializeField] public Camera playerCamera { get; private set; }
        [field: SerializeField] public PlayerCharacterVisuals visuals { get; private set; }
        [field:SerializeField] public StudioListener listener { get; private set; }
        [field: SerializeField] public PlayerCameraBehaviour cameraBehaviour { get; private set; }
        [field: SerializeField] public PlayerInput playerInput { get; private set; }
        [field: SerializeField] public PlayerInteraction playerInteraction { get; private set; }
        [field: SerializeField] public PlayerHands playerHands { get; private set; }
        [field : SerializeField] public HandsItemVisuals HandsItemVisuals { get; private set; }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            gameObject.name = gameObject.name + NetworkBehaviourId + OwnerClientId;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.O) && IsOwner)
            {
                ServerTestRpc(TimeStamp.Now, OwnerClientId);
                Debug.LogError($"your ping is {NetworkManager.Singleton.NetworkConfig.NetworkTransport.GetCurrentRtt(NetworkManager.Singleton.NetworkConfig.NetworkTransport.ServerClientId)}");
            }
        }

        [Rpc(SendTo.Server)]
        void ServerTestRpc(float startTime, ulong askingClientID)
        {
            ClientTestRpc(startTime, RpcTarget.Single(askingClientID, RpcTargetUse.Temp));
        }

        [Rpc(SendTo.SpecifiedInParams)]
        void ClientTestRpc(float startTime, RpcParams rpcParams = default)
        {
            Debug.LogError($"the rpc delay is {(TimeStamp.Now - startTime) / 2}");
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
    }

}
