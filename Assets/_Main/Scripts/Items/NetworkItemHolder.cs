using System;
using Unity.Netcode;
using UnityEngine;

[Serializable]
public class NetworkItemHolder : NetworkBehaviour
{
    public NetworkVariable<NetworkObjectReference> CurrItemRef = new();
    public NetworkObject CurrItem => CurrItemRef.Value;

    [SerializeField]
    private Transform handlingPivot;

    public void InitUpdateParentTransform()
    {
        CurrItemRef.OnValueChanged += (_, _) => Debug.Log($"{this} changed holding item to {CurrItem}");
        CurrItemRef.OnValueChanged += UpdateParentTransform;
    }

    private void UpdateParentTransform(NetworkObjectReference oldRef, NetworkObjectReference newRef)
    {
        if (!newRef.TryGet(out NetworkObject networkObject))
            return;

        networkObject.gameObject.SetParentWithTransform(transform, handlingPivot);
    }

    [Rpc(SendTo.Server)]
    public void HoldItemRPC(NetworkObjectReference spawnedItem)
    {
        Debug.Log($"{this} Picked up {spawnedItem}");
        CurrItemRef.Value = spawnedItem;
    }

    public void SwapItemFromHolder(NetworkItemHolder networkItemHolder)
    {
        SwapItemFromHolderRPC(networkItemHolder.NetworkObject);
    }

    [Rpc(SendTo.Server)]
    private void SwapItemFromHolderRPC(NetworkObjectReference targetHolderRef)
    {
        if (!targetHolderRef.TryGet(out NetworkObject networkObject))
            return;

        if (!networkObject.TryGetComponent(out NetworkItemHolder targetHolder))
            return;

        Debug.Log($"{this}({CurrItem}) Swapping to {targetHolder}({targetHolder.CurrItem})");
        (CurrItemRef.Value, targetHolder.CurrItemRef.Value) = (targetHolder.CurrItemRef.Value, CurrItemRef.Value);

    }
}
