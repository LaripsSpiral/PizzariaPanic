using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    // Call this from a "Quit" or "Back to Menu" button
    public void BackToMenu()
    {
        // 1. Shut down the network first
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.Shutdown();
        }

        // 2. Load the Menu scene
        // Replace "MainMenu" with your actual scene name
        SceneManager.LoadScene("MainMenu");
    }
}