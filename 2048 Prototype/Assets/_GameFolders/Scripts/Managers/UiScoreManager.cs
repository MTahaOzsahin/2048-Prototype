using Prototype.Scripts.Managers.ScriptableObjects;
using TMPro;
using UnityEngine;

namespace Prototype.Scripts.Managers
{
    public class UiScoreManager : MonoBehaviour
    {
        [SerializeField] ScoreManager scoreManager;
        [SerializeField] TextMeshProUGUI currentScoreText;
        [SerializeField] TextMeshProUGUI bestScoreText;

        private void Start()
        {
            WriteBestScores(scoreManager.bestScore);
        }
        private void OnEnable()
        {
            scoreManager.onScoreChangeEvent.AddListener(WriteCurrentScore);
            scoreManager.onBestScoreChangeEvent.AddListener(WriteBestScores);
        }
        private void OnDisable()
        {
            scoreManager.onScoreChangeEvent.RemoveListener(WriteCurrentScore);
            scoreManager.onBestScoreChangeEvent.RemoveListener(WriteBestScores);
        }
        public void WriteBestScores(int bestScore)
        {
            bestScoreText.text = bestScore.ToString();
        }
        public void WriteCurrentScore(int currentScore)
        {
            currentScoreText.text = currentScore.ToString();
        }

        public void ChangeCurrentScore(int targerScore)
        {
            currentScoreText.text = targerScore.ToString();
        }
    }
}
