using Main.Order.UI;
using Main.Recipe;
using Mono.Cecil;
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
        public static OrderManager Instance;

        [SerializeField] private OrderUI orderUI;
        [SerializeField] private List<RecipeSO> recipeList = new();

        [Header("Seeding & Limits")]
        [SerializeField] private int randomSeed = 12345;
        [SerializeField] private int maxConcurrentOrders = 5;
        private int totalOrdersForRound => GameManager.Instance.Stat.TotalOrder;

        [Header("Timings")]
        [SerializeField] private float timeBetweenOrders = 10f;

        // NetworkList synchronizes the order IDs from Server to all Clients automatically
        private NetworkList<FixedString32Bytes> orderList;

        private System.Random seededRandom;
        private float spawnTimer;
        private int ordersSpawnedSoFar = 0;

        private void Awake()
        {
            Instance = this;
            // Initialize NetworkList in Awake
            orderList = new NetworkList<FixedString32Bytes>();
        }

        public override void OnNetworkSpawn()
        {
            if (IsServer)
            {
                seededRandom = new System.Random(randomSeed * (int)Time.time);
                spawnTimer = timeBetweenOrders;
            }

            orderList.OnListChanged += HandleOrderListChanged;
        }

        public override void OnNetworkDespawn()
        {
            orderList.OnListChanged -= HandleOrderListChanged;
        }

        private void Update()
        {
            if (!IsServer)
                return;

            if (!GameManager.Instance.IsRoundStarted)
                return;

            // Exit if not the server or if we've reached the round limit
            if (ordersSpawnedSoFar >= totalOrdersForRound) 
                return;

            spawnTimer -= Time.deltaTime;

            if (spawnTimer <= 0f)
            {
                // Overcooked Logic: Only spawn if the UI isn't full
                if (orderList.Count < maxConcurrentOrders)
                {
                    AddRandomOrder();
                    spawnTimer = timeBetweenOrders;
                }
                else
                {
                    spawnTimer = 0f;
                }
            }
        }

        private void HandleOrderListChanged(NetworkListEvent<FixedString32Bytes> changeEvent)
        {
            switch (changeEvent.Type)
            {
                case NetworkListEvent<FixedString32Bytes>.EventType.Add:
                    // Only add the single new recipe to the UI
                    RecipeSO newRecipe = GetRecipeFromID(changeEvent.Value);
                    if (newRecipe != null) orderUI.Add(newRecipe);
                    break;

                case NetworkListEvent<FixedString32Bytes>.EventType.Remove:
                case NetworkListEvent<FixedString32Bytes>.EventType.RemoveAt:
                    // Only remove the specific recipe from the UI
                    orderUI.Remove(changeEvent.Value.ToString());
                    break;

                case NetworkListEvent<FixedString32Bytes>.EventType.Clear:
                    orderUI.Clear();
                    break;
            }
        }

        [ContextMenu("Add Random Order")]
        private void AddRandomOrder()
        {
            if (!IsServer || recipeList.Count == 0) return;

            // Use System.Random with seed for deterministic results
            var randIndex = seededRandom.Next(0, recipeList.Count);
            AddOrder(recipeList[randIndex]);
        }

        private void AddOrder(RecipeSO recipeSO)
        {
            if (!IsServer) return;

            orderList.Add(recipeSO.ID);
            ordersSpawnedSoFar++;
            Debug.Log($"Order Added: {recipeSO.name} ({ordersSpawnedSoFar}/{totalOrdersForRound})");
        }

        [ContextMenu("TestServe")]
        private void TestSentOrder() => TrySentOrder(orderList[0].ToString());

        public bool TrySentOrder(string recipeID)
        {
            if (!IsServer) return false;
            for (int i = 0; i < orderList.Count; i++)
            {
                if (orderList[i].ToString() == recipeID)
                {
                    orderList.RemoveAt(i);
                    return true;
                }
            }
            return false;
        }

        public RecipeSO FindRecipeInOrder(RecipeController findingRecipeController)
        {
            foreach (var orderID in orderList)
            {
                var recipe = recipeList.FirstOrDefault(r => r.ID == orderID.ToString());
                if (recipe == null) continue;

                var ingredientsIDs = recipe.IngredientsIDList;
                var findingIngredientsIDs = findingRecipeController.IngredientsIDNetList.AsNativeArray();

                var isEqual = ingredientsIDs.Count == findingIngredientsIDs.Length
                    && !ingredientsIDs.Except(findingIngredientsIDs).Any();

                if (isEqual)
                    return recipe;
            }

            return null;
        }

        private RecipeSO GetRecipeFromID(FixedString32Bytes id)
        {
            return recipeList.Find(recipe => recipe.ID == id.ToString());
        }
    }
}