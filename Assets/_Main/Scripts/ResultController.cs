using TMPro;
using UnityEngine;

public class ResultController : MonoBehaviour
{
    [SerializeField]
    private Canvas ui;

    [SerializeField]
    private TMP_Text resultText;

    private void Show()
    {
        ui.gameObject.SetActive(true);
    }

    public void Completed()
    {
        Debug.Log($"{this}, Completed");
        resultText.text = "Completed";
        Show();
    }

    public void Failed()
    {
        Debug.Log($"{this}, Failed");
        resultText.text = "Failed";
        Show();
    }

}
