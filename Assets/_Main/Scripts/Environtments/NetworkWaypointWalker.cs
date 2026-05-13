using Unity.Netcode;
using UnityEngine;
using System.Collections;

public class NetworkWaypointWalker : NetworkBehaviour
{
    public Transform[] waypoints;
    public float moveSpeed = 3f;
    public float spawnInterval = 3f;

    public override void OnNetworkSpawn()
    {
        GameManager.Instance.OnGameStarted += () => StartCoroutine(ServerSpawnLoop());
    }

    IEnumerator ServerSpawnLoop()
    {
        while (true)
        {
            NetworkObject npc = NetworkNpcPooler.Instance.GetNpcFromPool();
            if (npc != null)
            {
                // 1. Set the position BEFORE spawning
                npc.transform.position = waypoints[0].position;
                npc.transform.rotation = waypoints[0].rotation;

                // 2. Spawn it (This makes it visible/active for all clients)
                npc.Spawn();

                StartCoroutine(MoveNpc(npc));
            }
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    IEnumerator MoveNpc(NetworkObject npc)
    {
        int currentIndex = 0;

        // While the NPC is still spawned and valid
        while (currentIndex < waypoints.Length && npc != null && npc.IsSpawned)
        {
            Vector3 target = waypoints[currentIndex].position;

            while (Vector3.Distance(npc.transform.position, target) > 0.1f)
            {
                // Only move if the NPC hasn't been despawned mid-journey
                if (npc == null || !npc.IsSpawned) yield break;

                npc.transform.position = Vector3.MoveTowards(
                    npc.transform.position,
                    target,
                    moveSpeed * Time.deltaTime
                );

                npc.transform.LookAt(target);
                yield return null;
            }
            currentIndex++;
        }

        // 3. Return to pool by Despawning (false means don't destroy the object)
        if (npc != null && npc.IsSpawned)
        {
            npc.Despawn(false);
        }
    }
}