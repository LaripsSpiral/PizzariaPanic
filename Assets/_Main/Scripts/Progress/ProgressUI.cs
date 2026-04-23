using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProgressUI : MonoBehaviour
{
    [SerializeField]
    private Image progressBar;

    [SerializeField]
    private Image[] stars;

    [SerializeField]
    private TMP_Text progessText;

    public void Setup(int currentSent, int goalSent)
    {
        progessText.text = $"Orders {currentSent} / {goalSent}";
    }

    public void UpdateStar(int starCount)
    {
        for (int i = 0; i < stars.Length; i++)
        {
            stars[i].gameObject.SetActive(i <= starCount - 1);
        }
    }
}
