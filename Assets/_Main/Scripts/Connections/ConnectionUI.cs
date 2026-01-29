using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class ConnectionUI : MonoBehaviour
{
    [SerializeField]
    private Button hostButton;

    [SerializeField]
    private Button clientButton;

    [SerializeField]
    private Button stopButton;

    private void Start()
    {
        hostButton.onClick.AddListener(StartHost);
        clientButton.onClick.AddListener(StartClient);
        stopButton.onClick.AddListener(Stop);
    }

    private void StartHost()
    {
        NetworkManager.Singleton.StartHost();
    }

    private void StartClient()
    {
        NetworkManager.Singleton.StartClient();
    }
    private void Stop()
    {
        NetworkManager.Singleton.Shutdown();
    }
}
