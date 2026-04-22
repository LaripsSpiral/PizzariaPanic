using Main.Ingredient;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Main.Station
{
    public class Station_IngredientContainer : BaseStation
    {
        [SerializeField]
        private IngredientSO ingredientData;

        [SerializeField]
        private IngredientController ingredientControllerPrefab;

        [SerializeField]
        private Image ingredientIcon;

        private void OnValidate()
        {
            if (ingredientData != null)
            {
                ingredientIcon.sprite = ingredientData.Icon;
            }
        }

        public override void HandleInteract(InputAction.CallbackContext inputCtx, PlayerCharacter character)
        {
            base.HandleInteract(inputCtx, character);

            PlayerPickPlaceInteract(character);

            if (ItemHolder.HoldingNetObj)
                return;

            if (character.ItemHolder.HoldingNetObj != null)
                return;

            SpawnNetworkIngredientRPC();
            character.ItemHolder.SwapItemFromHolder(ItemHolder);
        }

        [Rpc(SendTo.Server)]
        private void SpawnNetworkIngredientRPC()
        {
            var instanceNetworkObj = Instantiate(ingredientControllerPrefab).NetworkObject;
            instanceNetworkObj.Spawn();

            if (instanceNetworkObj.TryGetComponent(out IngredientController ingredient))
            {
                ingredient.SetDataRPC(ingredientData.Name);
                ItemHolder.HoldItemRPC(instanceNetworkObj);
            }
        }
    }
}