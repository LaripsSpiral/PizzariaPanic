using Main.Ingredient;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

namespace Main.Recipe
{
    public class RecipeController : NetworkBehaviour
    {
        public NetworkList<FixedString32Bytes> IngredientsIDNetList = new(writePerm: NetworkVariableWritePermission.Server);

        [Rpc(SendTo.Server)]
        public void AddIngredientIDServerRPC(FixedString32Bytes ingredientId)
        {
            IngredientsIDNetList.Add(ingredientId);
        }

        [Rpc(SendTo.Server)]
        public void RemoveIngredientIDServerRPC(FixedString32Bytes ingredientId)
        {
            IngredientsIDNetList.Remove(ingredientId);
        }
    }
}