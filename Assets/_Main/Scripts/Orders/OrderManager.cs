using Main.Order.UI;
using Main.Recipe;
using NaughtyAttributes;
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

        [SerializeField, ReadOnly]
        private List<RecipeSO> orderList;

        [SerializeField]
        private List<RecipeSO> recipeList = new();

        public void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            orderUI.Init();
        }

        [Button]
        private void AddRandomOrder()
        {
            AddOrder(recipeList[Random.Range(0, recipeList.Count+1)]);
        }

        public void AddOrder(RecipeSO recipeSO)
        {
            orderList.Add(recipeSO);
        }

        public bool TryRemoveOrder(RecipeSO recipeSO)
        {
            if (orderList.Contains(recipeSO))
            {
                orderList.Remove(recipeSO);
                return true;
            }
            return false;
        }

        public RecipeSO FindOrder(RecipeController findingRecipeController)
        {
            foreach (var recipe in orderList)
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