using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProgressUI : MonoBehaviour
{
    [SerializeField]
    private Image[] stars;

    [SerializeField]
    private TMP_Text progessText;

    [SerializeField]
    private TMP_Text mistakeText;

    private void OnEnable()
    {
        var stat = GameManager.Instance.Stat;
        SetProgress(stat.SentOrder.Value, stat.TotalOrder);
        SetMistake(stat.GetStarScore(), stat.MaxFailCount);
    }
    public void SetProgress(int currentSent, int goalSent)
    {
        progessText.text = $"{currentSent}/{goalSent}";
    }

    public void SetMistake(int starCount, int maxMistake)
    {
        mistakeText.text = $"{maxMistake - starCount}/{maxMistake}";

        for (int i = 0; i < stars.Length; i++)
        {
            stars[i].gameObject.SetActive(i <= starCount - 1);
        }
    }
}
