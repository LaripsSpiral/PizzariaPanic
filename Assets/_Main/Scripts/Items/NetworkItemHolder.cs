using System;
using Unity.Netcode;
using UnityEngine;

namespace Main.Items.ItemHolder
{
    [Serializable]
    public class NetworkItemHolder : NetworkBehaviour
    {
        public NetworkVariable<NetworkObjectReference> CurrItemRef = new();
        public NetworkObject HoldingNetObj => CurrItemRef.Value;

        [SerializeField]
        private Transform handlingPivot;

        public void InitUpdateParentTransform()
        {
            CurrItemRef.OnValueChanged += (_, _) => Debug.Log($"{this} changed holding item to {HoldingNetObj}");
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

            Debug.Log($"{this}({HoldingNetObj}) Swapping to {targetHolder}({targetHolder.HoldingNetObj})");
            (CurrItemRef.Value, targetHolder.CurrItemRef.Value) = (targetHolder.CurrItemRef.Value, CurrItemRef.Value);

        }
    }
}