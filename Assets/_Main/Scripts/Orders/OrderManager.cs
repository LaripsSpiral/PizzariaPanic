using Main.Order.UI;
using Main.Recipe;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Main.Order
{
    public partial class OrderManager : MonoBehaviour
    {
        [SerializeField]
        private OrderUI orderUI;

        public static OrderManager Instance;

        [SerializeField]
        private List<RecipeSO> recipeList;

        public void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            orderUI.Init();
        }

        public void AddOrder(RecipeSO recipeSO)
        {
            recipeList.Add(recipeSO);
        }

        public bool TryRemoveOrder(RecipeSO recipeSO)
        {
            if (recipeList.Contains(recipeSO))
            {
                recipeList.Remove(recipeSO);
                return true;
            }
            return false;
        }

        public RecipeSO FindOrder(RecipeController findingRecipeController)
        {
            foreach (var recipe in recipeList)
            {
                var ingredientsIDs = recipe.IngredientsIDList;
                var findingIngredientsIDs = findingRecipeController.IngredientsIDNetList.AsNativeArray();

                var isEqual = ingredientsIDs.Count == findingIngredientsIDs.Length
                    && !ingredientsIDs.Except(findingIngredientsIDs).Any();

                if (isEqual)
                    return recipe;
            };

            return null;
        }
    }
}