using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

public class HostSingleton : MonoBehaviour
{
    private static HostSingleton instance;

    public HostGameManager GameManager { get; private set; }
    public static HostSingleton Instance
    {
        get
        {
            if (instance != null) { return instance; }
            instance = FindFirstObjectByType<HostSingleton>();

            if (instance == null)
            {
                Debug.LogError("No HostSingleton in the scene!");
                return null;
            }
            return instance;
        }
    }
    async void Awake()
    {
        DontDestroyOnLoad(gameObject);

        try
        {
            // Initialize all Unity Services (Relay, Lobby, etc.)
            await AuthenticatePlayer();
            Debug.Log("Unity Services Initialized");

            CreateHost();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Unity Services failed to initialize: {e.Message}");
        }
    }
    public async Task<bool> AuthenticatePlayer()
    {
        try
        {
            // 1. Initialize the Core SDK
            await UnityServices.InitializeAsync();

            // 2. Check if we are already signed in
            if (!AuthenticationService.Instance.IsSignedIn)
            {
                // 3. Sign in anonymously
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
                Debug.Log($"Signed in as: {AuthenticationService.Instance.PlayerId}");
            }

            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Authentication failed: {e.Message}");
            return false;
        }
    }

    public void CreateHost()
    {
        GameManager = new HostGameManager();
    }

    private void OnDestroy()
    {
        GameManager?.Dispose();
    }
}
