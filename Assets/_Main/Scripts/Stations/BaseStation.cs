using Main.Ingredient;
using Main.Items.ItemHolder;
using NUnit.Framework.Constraints;
using PrimeTween;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Main.Station
{
    public abstract class BaseStation : NetworkBehaviour, IInteractable
    {
        public Transform Transform => transform;

        public NetworkItemHolder ItemHolder;

        [SerializeField]
        private Transform view;

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

            PunchScale();
        }

        [ContextMenu("PunchRot")]
        protected void PunchRot()
        {
            var randX = Random.Range(-.5f, .5f);
            var randY = Random.Range(-.75f, .75f);
            var randZ = Random.Range(-.5f, .5f);

            Tween.PunchLocalRotation(view, new Vector3(randX, randY, randZ) * 45, .5f, easeBetweenShakes: Ease.InOutElastic);
        }

        [ContextMenu("PunchScale")]
        protected void PunchScale()
        {
            var randX = Random.Range(-.5f, .5f);
            var randY = Random.Range(-.75f, .75f);
            var randZ = Random.Range(-.5f, .5f);

            Tween.PunchScale(view, new Vector3(randX, randY, randZ) * 1.1f, .5f, easeBetweenShakes: Ease.InOutElastic);
        }

        [Rpc(SendTo.ClientsAndHost)]
        protected void ProcessPunchRpc()
        {
            PunchRot();
            PunchScale();
        }
    }
}