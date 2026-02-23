using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
public class PizzaComponentController : NetworkBehaviour
{
    [field: SerializeField]
    public NetworkVariable<float> currProcessTime { get; set; }

    private NetworkVariable<FixedString32Bytes> dataId = new();
    public FixedString32Bytes DataId => dataId.Value;

    private NetworkVariable<List<FixedString32Bytes>> combinedComponents = new(new());
    public List<FixedString32Bytes> CombinedComponentsId => combinedComponents.Value;

    [SerializeField]
    private PizzaComponentSO currentData;
    public PizzaComponentSO Data => currentData;

    private IngredientModel model = new();
    public IngredientModel Model => model;

    private GameObject currentView;

    protected void Start()
    {
        model.Init(this);
    }

    public override void OnNetworkSpawn()
    {
        dataId.OnValueChanged += HandleDataIDChange;

        if (!string.IsNullOrEmpty(dataId.Value.ToString()))
            HandleDataIDChange("", dataId.Value);
    }

    public override void OnNetworkDespawn()
    {
        dataId.OnValueChanged -= HandleDataIDChange;
    }

    [Rpc(SendTo.Server)]
    public void SetDataRPC(string dataID)
    {
        dataId.Value = dataID.ToString();
    }

    private void HandleDataIDChange(FixedString32Bytes previousValue, FixedString32Bytes newValue)
    {
        LoadDataAndSyncView(newValue.ToString());
    }

    private async void LoadDataAndSyncView(string key)
    {
        Debug.Log($"[Addressables] Starting load for: {key}");

        // Load the ScriptableObject
        var handle = Addressables.LoadAssetAsync<PizzaComponentSO>(key);
        await handle.Task;

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            currentData = handle.Result; // Now SO is set!
            UpdateView(currentData);
        }
        else
        {
            Debug.LogError($"[Addressables] Could not find SO with key: {key}");
        }
    }

    private void UpdateView(PizzaComponentSO data)
    {
        if (currentView != null) Destroy(currentView);
        if (data.Prefab == null) return;

        currentView = Instantiate(data.Prefab, transform, false);
        currentView.transform.localPosition = Vector3.zero;
    }
}
