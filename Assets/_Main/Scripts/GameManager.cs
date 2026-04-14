using System;
using Unity.Netcode;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance;

    public bool IsRoundStarted = false;

    public int TotalOrder = 10;

    public NetworkVariable<int> SentOrder = new();

    [SerializeField]
    private ProgressUI progressUI;

    public void Awake()
    {
        Instance = this;
    }

    public void Start()
    {
        SentOrder.OnValueChanged += (_,_) => UpdateProgress();
        UpdateProgress();
    }

    [ContextMenu("StartGame")]
    private void StartGame()
    {
        if (!IsServer)
            return;

        Debug.Log("[GameManager] Start Game");
        IsRoundStarted = true;
    }

    public void GameOver()
    {
        if (!IsServer)
            return;

        Debug.Log("[GameManager] Game Over");
        IsRoundStarted = false;
    }

    private void UpdateProgress()
    {
        var value = (float)SentOrder.Value / (float)TotalOrder;
        progressUI.Setup(value);


        if (SentOrder.Value < TotalOrder)
            return;
        GameOver();
    }
}
