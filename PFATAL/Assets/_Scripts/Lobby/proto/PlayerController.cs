using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PlayerInput))]
public class PlayerController : NetworkBehaviour
{
    [Header("Mouvement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 10f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    private Rigidbody2D rb;
    private PlayerInput playerInput;

    // Input Actions
    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction interactAction;

    // Input Values
    private float moveInput;
    private bool jumpInput;

    // States
    private bool isGrounded;
    private bool isChatOpen = false;

    // Network
    private NetworkVariable<Vector2> networkPosition = new NetworkVariable<Vector2>();
    private NetworkVariable<Vector2> networkVelocity = new NetworkVariable<Vector2>();

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerInput = GetComponent<PlayerInput>();
    }

    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
        {
            playerInput.enabled = false;
        }
        else
        {
            moveAction = playerInput.actions["Move"];
            jumpAction = playerInput.actions["Jump"];
            interactAction = playerInput.actions["Interact"];
        }

        if (!IsServer)
        {
            networkPosition.OnValueChanged += OnPositionChanged;
            networkVelocity.OnValueChanged += OnVelocityChanged;
        }

        if (IsOwner)
        {
            GetComponent<SpriteRenderer>().color = Color.green;
        }
        else
        {
            GetComponent<SpriteRenderer>().color = Color.blue;
        }
    }

    private void Update()
    {
        if (!IsOwner) return;

        ReadInputs();

        if (isChatOpen) return;

        if (!IsServer)
        {
            SendInputToServerRpc(moveInput, jumpInput);
        }
        else
        {
            ProcessMovement(moveInput, jumpInput);
        }
    }

    private void ReadInputs()
    {
        Vector2 moveVector = moveAction.ReadValue<Vector2>();
        moveInput = moveVector.x;

        jumpInput = jumpAction.WasPressedThisFrame();

    }

    public void SetChatOpen(bool open)
    {
        isChatOpen = open;
    }

    private void FixedUpdate()
    {
        CheckGroundStatus();
    }

    [ServerRpc]
    private void SendInputToServerRpc(float horizontal, bool jump)
    {
        ProcessMovement(horizontal, jump);
    }

    private void ProcessMovement(float horizontal, bool jump)
    {
        if (!IsServer) return;

        rb.linearVelocity = new Vector2(horizontal * moveSpeed, rb.linearVelocity.y);

        if (jump && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        networkPosition.Value = transform.position;
        networkVelocity.Value = rb.linearVelocity;
    }

    private void CheckGroundStatus()
    {
        if (groundCheck != null)
        {
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        }
    }

    private void OnPositionChanged(Vector2 previousValue, Vector2 newValue)
    {
        if (!IsServer)
        {
            transform.position = newValue;
        }
    }

    private void OnVelocityChanged(Vector2 previousValue, Vector2 newValue)
    {
        if (!IsServer)
        {
            rb.linearVelocity = newValue;
        }
    }

    public override void OnNetworkDespawn()
    {
        if (!IsServer)
        {
            networkPosition.OnValueChanged -= OnPositionChanged;
            networkVelocity.OnValueChanged -= OnVelocityChanged;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}