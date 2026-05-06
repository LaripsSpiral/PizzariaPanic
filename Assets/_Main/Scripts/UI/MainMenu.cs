using System.Text.RegularExpressions;
using System.Threading.Tasks;
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
    
    private const int TimeoutMs = 8000; // 8 seconds

    public async void StartHost()
    {
        SetLoadingState(true);

        try
        {
            // Create the connection task
            var hostTask = HostSingleton.Instance.GameManager.StartHostAsync();

            // Create a timeout task
            if (await Task.WhenAny(hostTask, Task.Delay(TimeoutMs)) == hostTask)
            {
                // HostTask finished first
                await hostTask;
            }
            else
            {
                // Timeout finished first
                throw new System.TimeoutException("Host connection timed out after 8 seconds.");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Host failed: {e.Message}");
            SetLoadingState(false);
        }
    }

    public async void StartClient()
    {
        string code = joinCodeField.text.Trim().ToUpper();

        // 1. Basic Validation check
        if (string.IsNullOrEmpty(code) || code.Length < 6)
        {
            Debug.LogError("Join Code must be at least 6 characters long.");
            // Optional: Show a UI popup to the user here
            return;
        }

        // 2. Regex check (Matches Unity's Relay requirements)
        if (!Regex.IsMatch(code, @"^[6789BCDFGHJKLMNPQRTW]{6,12}$"))
        {
            Debug.LogError("Invalid characters in Join Code. Use only letters and numbers provided by the host.");
            return;
        }

        SetLoadingState(true);

        try
        {
            // Now it's safe to send
            var clientTask = ClientSingleton.Instance.GameManager.StartClientAsync(code);

            if (await Task.WhenAny(clientTask, Task.Delay(TimeoutMs)) == clientTask)
            {
                await clientTask;
            }
            else
            {
                throw new System.TimeoutException("Join connection timed out.");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Client failed: {e.Message}");
            SetLoadingState(false);
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