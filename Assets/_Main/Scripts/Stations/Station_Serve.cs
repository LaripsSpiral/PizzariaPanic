using Main.Ingredient;
using Main.Order;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Main.Station
{
    public class Station_Serve : BaseStation
    {
        public override void HandleInteract(InputAction.CallbackContext inputCtx, PlayerCharacter character)
        {
            base.HandleInteract(inputCtx, character);
            PlayerPickPlaceInteract(character);

            if (ItemHolder.HoldingNetObj)
            {
                Serve();
            }
        }

        private void Serve()
        {
            if (!ItemHolder.HoldingNetObj.TryGetComponent(out IngredientController holderPizzaComponent))
                return;

            var recipeController = holderPizzaComponent.RecipeController;
            var orderedRecipe = OrderManager.Instance.FindOrder(recipeController);

            if (!orderedRecipe)
            {
                Debug.Log($"{this}, Not ordered the {recipeController}");
                return;
            }

            Debug.Log($"{this}, Sending {orderedRecipe} order");

            if (!OrderManager.Instance.TryRemoveOrder(orderedRecipe))
            {
                Debug.LogWarning($"{this}, Failed to Sending {orderedRecipe} order");
            }
            else
            {
                Debug.Log($"{this}, Sent {orderedRecipe} order");
                Destroy(ItemHolder.HoldingNetObj.gameObject);
            }
        }
    }
}