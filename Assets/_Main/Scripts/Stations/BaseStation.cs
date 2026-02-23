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
        var characterHoldingItem = character.ItemHolder.HoldingNetObj;
        if (!characterHoldingItem && !ItemHolder.HoldingNetObj)
            return;

        if (characterHoldingItem && ItemHolder.HoldingNetObj)
        {
            ItemHolder.HoldingNetObj.TryGetComponent(out PizzaComponentController holderPizzaComponent);
            characterHoldingItem.TryGetComponent(out PizzaComponentController characterPizzaComponent);

            // Combine Ingredient
            if (characterPizzaComponent.Model.TryAddIngredient(holderPizzaComponent))
                return;
        }

        ItemHolder.SwapItemFromHolder(character.ItemHolder);
    }
}
