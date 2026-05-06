using Main.Recipe;
using System;
using System.Threading.Tasks;
using Unity.Collections;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using static UnityEngine.UI.GridLayoutGroup;

namespace Main.Ingredient
{
    public class IngredientController : NetworkBehaviour
    {
        [field: SerializeField]
        public NetworkVariable<float> currProcessTime { get; set; }

        private NetworkVariable<FixedString32Bytes> dataId = new();
        public FixedString32Bytes DataId => dataId.Value;

        [SerializeField]
        private RecipeController recipeController;
        public RecipeController RecipeController => recipeController;

        [SerializeField]
        private IngredientView view;
        public IngredientView View => view;

        [SerializeField]
        private IngredientSO currentData;
        public IngredientSO Data => currentData;

        [SerializeField]
        private IngredientModel model = new();
        public IngredientModel Model => model;

        [SerializeField]
        private bool isCooked;
        public bool IsCooked => isCooked;

        [SerializeField]
        private ProcessProgressUI progressUI;

        protected void Start()
        {
            model.Init(this);
        }

        public override void OnNetworkSpawn()
        {
            dataId.OnValueChanged += HandleDataIDChange;

            currProcessTime.OnValueChanged += HandleProcessTime;

            var currentKey = dataId.Value.ToString();
            if (!string.IsNullOrEmpty(currentKey))
                HandleDataIDChange(default, dataId.Value);
        }

        public override void OnNetworkDespawn()
        {
            dataId.OnValueChanged -= HandleDataIDChange;
        }

        public void SetCooked()
        {
            if (isCooked)
                return;

            Debug.Log($"{this}: Set Cooked");

            isCooked = true;
            model.ChangedToProcessedData();

            // Broadcast cooked view update to all clients (SetCooked only runs on server)
            UpdateCookedViewClientRPC();
        }

        [Rpc(SendTo.ClientsAndHost)]
        private void UpdateCookedViewClientRPC()
        {
            view.UpdateCookedView();
        }

        [Rpc(SendTo.Server)]
        public void SetDataRPC(string dataID)
        {
            dataId.Value = dataID ?? string.Empty;
        }

        /// <summary>
        /// Server RPC: Adds an ingredient into this ingredient (e.g. topping onto dough).
        /// Called from IngredientModel.TryAddIngredient via the controller since
        /// IngredientModel is not a NetworkBehaviour and cannot send RPCs.
        /// </summary>
        [Rpc(SendTo.Server)]
        public void AddIngredientServerRPC(NetworkObjectReference addingIngredientRef)
        {
            if (!addingIngredientRef.TryGet(out NetworkObject netObj))
                return;

            if (!netObj.TryGetComponent(out IngredientController addingIngredient))
                return;

            var addingID = addingIngredient.Data.Name;

            // We're on the server — write directly to the NetworkList
            recipeController.IngredientsIDNetList.Add(addingID);

            // Tell all clients to load the visual for this ingredient
            AddViewClientRPC(addingID);

            // Properly despawn the added ingredient's NetworkObject (server-authoritative)
            netObj.Despawn();
        }

        /// <summary>
        /// Client RPC: Each client loads the ingredient view from Addressables independently.
        /// </summary>
        [Rpc(SendTo.ClientsAndHost)]
        private void AddViewClientRPC(string ingredientId)
        {
            LoadAndAddView(ingredientId);
        }

        private async void LoadAndAddView(string ingredientId)
        {
            var data = await LoadData(ingredientId);
            if (data == null)
            {
                Debug.LogError($"[IngredientController] LoadAndAddView: failed to load data for '{ingredientId}'");
                return;
            }

            var go = view.LoadView(data);
            view.AddView(ingredientId, go);
        }

        private async void HandleDataIDChange(FixedString32Bytes previousValue, FixedString32Bytes newValue)
        {
            var key = newValue.ToString();
            if (string.IsNullOrEmpty(key))
            {
                currentData = null;
                return;
            }

            try
            {
                var data = await LoadData(newValue);
                currentData = Instantiate(data);
                currProcessTime.Value = 0;

                // Only modify the recipe list on the server to avoid duplicates.
                // HandleDataIDChange fires on ALL clients via OnValueChanged,
                // so without this guard each client would add/remove the ID,
                // causing the recipe list to have N copies (one per player).
                if (IsServer)
                {
                    recipeController.IngredientsIDNetList.Add(newValue);

                    if (!string.IsNullOrEmpty(previousValue.ToString()))
                        recipeController.IngredientsIDNetList.Remove(previousValue);
                }

                // Visual updates run on every client
                view.LoadView(data);
                view.RemoveView(previousValue);
            }
            catch (Exception ex)
            {
                Debug.LogError($"[IngredientController] Failed to load data for key '{key}': {ex}");
                currentData = null;
            }
        }

        public static async Task<IngredientSO> LoadData(FixedString32Bytes key)
        {
            var keyString = key.ToString();
            Debug.Log($"[Addressables] Starting load for: {keyString}");

            if (string.IsNullOrEmpty(keyString))
            {
                Debug.LogWarning("[Addressables] LoadData called with empty key.");
                return null;
            }

            AsyncOperationHandle<IngredientSO> handle = default;
            try
            {
                handle = Addressables.LoadAssetAsync<IngredientSO>(keyString);
                await handle.Task;

                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    return handle.Result;
                }
                else
                {
                    Debug.LogError($"[Addressables] Could not find SO with key: {keyString}. Status: {handle.Status}");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[Addressables] Exception while loading key '{keyString}': {ex}");
            }

            return null;
        }

        public void HandleProcessTime(float previousValue, float newValue)
        {
            progressUI.Setup(newValue, Data.ProcessData.ProcessTime);
        }
    }
}