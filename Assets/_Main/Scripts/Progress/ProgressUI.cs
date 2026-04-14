using UnityEngine;
using UnityEngine.UI;

public class ProgressUI : MonoBehaviour
{
    [SerializeField]
    private Image progressBar;

    public void Setup(float normalVal)
    {
        progressBar.fillAmount = normalVal;
    }
}
