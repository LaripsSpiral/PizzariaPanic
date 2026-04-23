using Main.Ingredient;
using Main.Order;
using Unity.Netcode;
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

            // Always send serve request to the server.
            // We can't check ItemHolder.HoldingNetObj here on the client because
            // the swap RPC from PlayerPickPlaceInteract hasn't replicated back yet.
            // The server will validate whether there's actually an item to serve.
            ServeRPC();
        }

        [Rpc(SendTo.Server)]
        private void ServeRPC()
        {
            // Server-side check: is there actually an item on this station?
            if (!ItemHolder.HoldingNetObj)
                return;

            if (!ItemHolder.HoldingNetObj.TryGetComponent(out IngredientController holderPizzaComponent))
                return;

            var recipeController = holderPizzaComponent.RecipeController;
            var orderedRecipe = OrderManager.Instance.FindRecipeInOrder(recipeController);

            if (!orderedRecipe)
            {
                Debug.Log($"{this}, Not ordered the {recipeController}");
                GameManager.Instance.Stat.MistakeFail.Value++;
                Destroy(recipeController.gameObject);
                return;
            }

            Debug.Log($"{this}, Sending {orderedRecipe} order");

            if (!OrderManager.Instance.TrySentOrder(orderedRecipe.ID))
            {
                Debug.LogWarning($"{this}, Failed to Sending {orderedRecipe} order");
            }
            else
            {
                Debug.Log($"{this}, Sent {orderedRecipe} order");
                GameManager.Instance.Stat.SentOrder.Value++;
                Destroy(ItemHolder.HoldingNetObj.gameObject);
            }
        }
    }
}