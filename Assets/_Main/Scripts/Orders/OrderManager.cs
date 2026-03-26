using Main.Order.UI;
using Main.Recipe;
using NaughtyAttributes;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
namespace Main.Order
{
    public partial class OrderManager : NetworkBehaviour
    {
        [SerializeField]
        private OrderUI orderUI;

        public static OrderManager Instance;

        private NetworkList<FixedString32Bytes> orderList = new();

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

        [ContextMenu("Add Random Order")]
        private void AddRandomOrder()
        {
            var randIndex = Random.Range(0, recipeList.Count);
            AddOrder(recipeList[randIndex]); ;
        }

        private void AddOrder(RecipeSO recipeSO)
        {
            Debug.Log($"{this}, added {recipeSO}");
            orderList.Add(recipeSO.ID);
            Debug.Log(orderList);
        }

        public bool TryRemoveOrder(string recipeID)
        {
            if (orderList.Contains(recipeID))
            {
                orderList.Remove(recipeID);
                return true;
            }
            return false;
        }

        public RecipeSO FindOrder(RecipeController findingRecipeController)
        {
            foreach (var order in orderList)
            {
                var recipe = recipeList.First(recipe => recipe.ID == order);

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