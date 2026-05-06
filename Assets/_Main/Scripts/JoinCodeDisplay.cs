using TMPro;
using UnityEngine;

public class JoinCodeDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI joinCodeText;

    void Start()
    {
        // 1. Check if we are the Host
        if (HostSingleton.Instance != null && HostSingleton.Instance.GameManager != null)
        {
            string code = HostSingleton.Instance.GameManager.JoinCode;

            if (!string.IsNullOrEmpty(code))
            {
                joinCodeText.text = $"Join Code: {code}";
            }
            else
            {
                // If there's no code (e.g. we are a client), hide the text
                joinCodeText.gameObject.SetActive(false);
            }
        }
        else
        {
            // Clients don't have the HostSingleton GameManager data
            joinCodeText.gameObject.SetActive(false);
        }
    }

    // Optional: Add a button to copy the code to the clipboard
    public void CopyCodeToClipboard()
    {
        GUIUtility.systemCopyBuffer = HostSingleton.Instance.GameManager.JoinCode;
        Debug.Log("Code copied!");
    }
}