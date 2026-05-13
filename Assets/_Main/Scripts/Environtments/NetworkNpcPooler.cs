using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetworkNpcPooler : NetworkBehaviour
{
    public static NetworkNpcPooler Instance;

    [Header("Network Pool Setup")]
    public GameObject[] npcPrefabs;
    public int amountToPool = 20;

    private List<NetworkObject> _pooledNpcs = new List<NetworkObject>();

    void Awake()
    {
        Instance = this;
    }

    public override void OnNetworkSpawn()
    {
        // Only the Server spawns the initial pool
        if (!IsServer) return;

        for (int i = 0; i < amountToPool; i++)
        {
            GameObject obj = Instantiate(npcPrefabs[Random.Range(0, npcPrefabs.Length)]);
            NetworkObject netObj = obj.GetComponent<NetworkObject>();

            netObj.Spawn(); // Spawn across the network
            netObj.Despawn(false); // Hide but keep in memory (Netcode 1.0+)
            _pooledNpcs.Add(netObj);
        }
    }

    public NetworkObject GetNpcFromPool()
    {
        if (!IsServer) return null;

        foreach (var netObj in _pooledNpcs)
        {
            if (!netObj.IsSpawned) return netObj;
        }
        return null;
    }
}