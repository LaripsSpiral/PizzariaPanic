using Main.Recipe;
using System;
using System.Threading.Tasks;
using Unity.Collections;
using Unity.Netcode;
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

        private IngredientModel model = new();
        public IngredientModel Model => model;

        [SerializeField]
        private bool isCooked;
        public bool IsCooked => isCooked;

        protected void Start()
        {
            model.Init(this);
        }

        public override void OnNetworkSpawn()
        {
            dataId.OnValueChanged += HandleDataIDChange;

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
            view.UpdateCookedView();
        }

        [Rpc(SendTo.Server)]
        public void SetDataRPC(string dataID)
        {
            dataId.Value = dataID ?? string.Empty;
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
                currentData = data;

                // Apply New
                recipeController.AddIngredientIDServerRPC(newValue);
                view.LoadView(data);

                // Remove Old
                recipeController.RemoveIngredientIDServerRPC(previousValue);
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
    }
}