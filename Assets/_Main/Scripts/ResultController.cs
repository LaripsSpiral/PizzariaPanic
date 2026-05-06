using TMPro;
using UnityEngine;

public class ResultController : MonoBehaviour
{
    [SerializeField]
    private Canvas ui;

    [SerializeField]
    private Transform[] disables;

    private void Start()
    {
        ui.enabled = false;
    }

    private void Show(Stat stat)
    {
        ui.enabled = true;
    }

    public void Completed(Stat stat)
    {
        Debug.Log($"{this}, Completed");
        Show(stat);
    }

    public void Failed(Stat stat)
    {
        Debug.Log($"{this}, Failed");
        Show(stat);
    }

}
