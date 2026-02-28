using Main.Ingredient;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Main.Station
{
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
                ItemHolder.HoldingNetObj.TryGetComponent(out IngredientController holderPizzaComponent);
                characterHoldingItem.TryGetComponent(out IngredientController characterPizzaComponent);

                // Combine Ingredient
                if (holderPizzaComponent.Model.TryAddIngredient(addingIngredient: characterPizzaComponent))
                    return;
            }

            ItemHolder.SwapItemFromHolder(character.ItemHolder);
        }
    }
}