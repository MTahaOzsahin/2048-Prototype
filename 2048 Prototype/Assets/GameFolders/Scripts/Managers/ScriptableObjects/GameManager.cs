using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Prototype.Scripts.Managers.ScriptableObjects
{
    [CreateAssetMenu(menuName ="Managers/GameManager")]
    public class GameManager : ScriptableObject
    {
        [SerializeField] SoundManager soundManager;
        [SerializeField] ScoreManager scoreManager;
        [SerializeField] GridManager gridManager;
        [SerializeField] DataManager dataManager;
        public void ExitApplication()
        {
            Application.Quit();
        }
        public void Restart()
        {
            scoreManager.currentScore = 0;
            dataManager.allActiveBlockNumber = 0;
            int currentLevelIndex = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadSceneAsync(currentLevelIndex,LoadSceneMode.Single);
        }
        public void SetGridLevel(int x)
        {
            scoreManager.currentScore = 0;
            int currentLevelIndex = SceneManager.GetActiveScene().buildIndex;
            gridManager.GridSelecter(x,x);
            SceneManager.LoadSceneAsync(currentLevelIndex, LoadSceneMode.Single);
        }
    }
}
