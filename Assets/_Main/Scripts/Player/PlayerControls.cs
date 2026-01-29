using NaughtyAttributes;
using Unity.Netcode;
using UnityEngine;

public class PlayerControls : NetworkBehaviour
{
    [SerializeField]
    private PlayerCharacter character;

    [SerializeField, ReadOnly]
    private Vector2 moveInput;

    private Controls controls;

    private void Awake()
    {
        controls = new();
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    private void Start()
    {
        if (!IsOwner)
        {
            this.enabled = false;
            return;
        }

        controls.FindAction("Move").performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.FindAction("Move").canceled += _ => moveInput = Vector2.zero;

        controls.FindAction("Dash").performed += _ => character.DashRPC(moveInput);
    }

    private void FixedUpdate()
    {
        character.MoveRPC(moveInput);
    }
}
