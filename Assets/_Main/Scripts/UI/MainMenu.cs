using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TMP_InputField joinCodeField;

    [Header("UI State Arrays")]
    [SerializeField] private GameObject[] objectsToDisable; // e.g., Host Button, Join Button, Input Field
    [SerializeField] private GameObject[] objectsToEnable;  // e.g., Loading Spinner, "Connecting..." text

    public async void StartHost()
    {
        SetLoadingState(true);

        try
        {
            await HostSingleton.Instance.GameManager.StartHostAsync();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Host failed: {e.Message}");
            SetLoadingState(false); // Re-enable UI if it fails
        }
    }

    public async void StartClient()
    {
        SetLoadingState(true);

        try
        {
            await ClientSingleton.Instance.GameManager.StartClientAsync(joinCodeField.text);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Client failed: {e.Message}");
            SetLoadingState(false); // Re-enable UI if it fails
        }
    }

    private void SetLoadingState(bool isLoading)
    {
        // Hide/Show objects based on the state
        foreach (var obj in objectsToDisable)
        {
            if (obj != null) obj.SetActive(!isLoading);
        }

        foreach (var obj in objectsToEnable)
        {
            if (obj != null) obj.SetActive(isLoading);
        }
    }
}