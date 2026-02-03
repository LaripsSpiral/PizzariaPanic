using NaughtyAttributes;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TextCore.Text;

public abstract class BaseStation : NetworkBehaviour, IInteractable
{
    public Transform Transform => transform;

    [SerializeField]
    private Transform holdingItemPivot;

    [SerializeField, ReadOnly]
    protected GameObject holdingItem;

    protected virtual void Start()
    {
        InteractManager.Instance.Subscribe(this);
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        InteractManager.Instance.Unsubscribe(this);
    }

    public virtual void HandleInteract(InputAction.CallbackContext inputCtx, PlayerCharacter character)
    {
        Debug.Log($"{character} Interacted {this}");
    }

    public virtual void HandleCancelInteract(InputAction.CallbackContext inputCtx, PlayerCharacter character)
    {
        Debug.Log($"{character} Canceled Interact {this}");
    }

    protected void PlayerPickPlaceInteract(PlayerCharacter character)
    {
        var characterHoldingObj = character.HoldingObject;

        if (characterHoldingObj && !holdingItem)
        {
            holdingItem = characterHoldingObj;
            character.Place(transform, holdingItemPivot);
            return;
        }
        else if (!characterHoldingObj && holdingItem)
        {
            character.Pickup(holdingItem);
            holdingItem = null;
            return;
        }
    }
}
