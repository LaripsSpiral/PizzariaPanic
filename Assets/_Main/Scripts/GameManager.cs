using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance;

    public bool IsRoundStarted = false;

    public Stat Stat;

    [SerializeField]
    private ProgressUI progressUI;

    [SerializeField]
    private ResultController resultController;

    public void Awake()
    {
        Instance = this;
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
        IsRoundStarted = true;
    }

    public void GameOver()
    {
        if (!IsServer)
            return;

        Debug.Log("[GameManager] Game Over");
        IsRoundStarted = false;
        resultController.Completed();
    }

    private void UpdateProgress()
    {
        progressUI.SetProgress(Stat.SentOrder.Value, Stat.TotalOrder);

        if (Stat.SentOrder.Value < Stat.TotalOrder)
            return;

        GameOver();
    }

    private void UpdateMistake()
    {
        progressUI.SetMistake(Stat.GetStarScore(), Stat.MaxFailCount);

        if (Stat.MistakeFail.Value >= Stat.MaxFailCount)
            resultController.Failed();
    }
}
