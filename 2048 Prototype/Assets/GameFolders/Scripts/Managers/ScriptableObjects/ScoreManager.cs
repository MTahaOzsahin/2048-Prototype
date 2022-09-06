using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Prototype.Scripts.Managers.ScriptableObjects
{
    [CreateAssetMenu(menuName = "Managers/ScoreManager")]
    public class ScoreManager : ScriptableObject
    {
        public int currentScore;
        public int bestScore;

        [System.NonSerialized]
        public UnityEvent<int> onScoreChangeEvent;
        [System.NonSerialized]
        public UnityEvent<int> onBestScoreChangeEvent;

        private void OnEnable()
        {
            currentScore = 0;
            if (onScoreChangeEvent == null)
            {
                onScoreChangeEvent = new UnityEvent<int>();
            }
            if (onBestScoreChangeEvent == null) 
            {
                onBestScoreChangeEvent = new UnityEvent<int>();
            }
        }
        public void HandleScore(int amoun)
        {
            currentScore += amoun * 2;
            if (currentScore >= bestScore)
            {
                bestScore = currentScore;
                onBestScoreChangeEvent?.Invoke(bestScore);
            }
            onScoreChangeEvent?.Invoke(currentScore);
        }
    }
}
