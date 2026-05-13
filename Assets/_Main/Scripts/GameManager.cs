using System;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance;

    public event UnityAction OnGameStarted;

    public bool IsRoundStarted = false;

    public Stat Stat;

    [SerializeField]
    private ProgressUI progressUI;

    [SerializeField]
    private ProgressUI resultProgressUI;

    [SerializeField]
    private ResultController resultController;

    private NetworkVariable<int> playersReady = new NetworkVariable<int>(0);

    [SerializeField] private Canvas readyCanvas;
    [SerializeField] private TextMeshProUGUI readyText; // UI to show "0 / 4 Ready"

    public void Awake()
    {
        Instance = this;
        readyCanvas.enabled = true;
    }
    public override void OnNetworkSpawn()
    {
        // Update UI whenever the value changes
        playersReady.OnValueChanged += (oldVal, newVal) => UpdateReadyUI();
        UpdateReadyUI();

        NetworkManager.OnClientConnectedCallback += _ => UpdateReadyUI();
        NetworkManager.OnClientDisconnectCallback += _ => UpdateReadyUI();
    }

    // This is called by a button in your UI
    public void OnReadyButtonPressed()
    {
        SetReadyServerRpc();
    }

    [Rpc(SendTo.Server)]
    private void SetReadyServerRpc(RpcParams rpcParams = default)
    {
        // Increment ready count
        playersReady.Value++;

        // Check if everyone is ready
        // networkManager.ConnectedClients.Count gives us the total players
        if (playersReady.Value >= NetworkManager.Singleton.ConnectedClients.Count)
        {
            StartGame();
        }
    }

    private void UpdateReadyUI()
    {
        if (readyCanvas != null)
        {
            int total = NetworkManager.Singleton.ConnectedClients.Count;
            readyText.text = $"Waiting for players: {playersReady.Value} / {total}";
        }
    }
    public void Start()
    {
        Stat.SentOrder.OnValueChanged += (_,_) => UpdateProgress();
        Stat.MistakeFail.OnValueChanged += (_,_) => UpdateMistake();
        UpdateProgress();
        UpdateMistake();
    }

    [ContextMenu("StartGame")]
    private void StartGame() => StartGameRPC();

    [Rpc(SendTo.Server)]
    public void StartGameRPC()
    {
        Debug.Log("[GameManager] Start Game");

        HostSingleton.Instance.GameManager.LockLobby();

        var players = FindObjectsByType<PlayerCharacter>(sortMode: FindObjectsSortMode.InstanceID);
        foreach (var player in players)
        {
            player.RandomSpawnPointRpc();
            player.SetColorRpc();
        }

        IsRoundStarted = true; 
        StartGameClientRpc();
        OnGameStarted.Invoke();
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void StartGameClientRpc()
    {
        // Hide the waiting screen for everyone
        readyCanvas.enabled = false;
    }

    public void GameOver()
    {
        if (!IsServer)
            return;

        Debug.Log("[GameManager] Game Over");
        IsRoundStarted = false;
        ClientShowResultRpc();
    }

    [Rpc(SendTo.Everyone)]
    private void ClientShowResultRpc()
    {
        resultController.Completed(Stat);
    }

    private void UpdateProgress()
    {
        progressUI.SetProgress(Stat.SentOrder.Value, Stat.TotalOrder);
        resultProgressUI.SetProgress(Stat.SentOrder.Value, Stat.TotalOrder);

        if (Stat.SentOrder.Value < Stat.TotalOrder)
            return;

        GameOver();
    }

    private void UpdateMistake()
    {
        progressUI.SetMistake(Stat.GetStarScore(), Stat.MaxFailCount);
        resultProgressUI.SetMistake(Stat.GetStarScore(), Stat.MaxFailCount);

        if (Stat.MistakeFail.Value >= Stat.MaxFailCount)
            resultController.Failed(Stat);
    }

    public void OnLeaveGame() => LeaveGame();

    private async void LeaveGame()
    {
        // 1. Tell Netcode to shut down
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.Shutdown();
        }

        // 2. Reset the Singleton states
        if (HostSingleton.Instance != null)
        {
            await HostSingleton.Instance.ResetHost();
        }

        // 3. Now load the menu scene
        SceneManager.LoadScene("Menu");
    }
}
