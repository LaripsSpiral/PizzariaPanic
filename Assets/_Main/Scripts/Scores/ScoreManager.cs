using System;
using Unity.Netcode;
using UnityEngine;

namespace Main.Scores
{
    public class ScoreManager : MonoBehaviour
    {
        [SerializeField]
        private int totalScores;
        public int TotalScores => totalScores;

        public void AddScore(int score)
        {
            totalScores += score;
        }
    }
}