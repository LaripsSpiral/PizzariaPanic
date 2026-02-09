using NaughtyAttributes;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TextCore.Text;

public abstract class BaseStation : NetworkBehaviour, IInteractable
{
    public Transform Transform => transform;

    public NetworkItemHolder ItemHolder;

    protected virtual void Start()
    {
        ItemHolder.InitUpdateParentTransform();
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
        // Both no holding item
        var characterHoldingItem = character.ItemHolder.CurrItem;
        if (!characterHoldingItem && !ItemHolder.CurrItem)
            return;

        ItemHolder.SwapItemFromHolder(character.ItemHolder);
    }
}
