using TMPro;
using UnityEngine;

namespace Main.Scores
{
    public class ScoreUI : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text scoreLabel;

        public void UpdateScore(int score)
        {
            scoreLabel.text = score.ToString();
        }
    }
}