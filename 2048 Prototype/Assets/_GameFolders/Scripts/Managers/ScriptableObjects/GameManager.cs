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
            SaveSystem.DeleteSaveGame();
            int currentLevelIndex = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadSceneAsync(currentLevelIndex, LoadSceneMode.Single);
        }
        public void SetGridLevel(int x)
        {
            scoreManager.currentScore = 0;
            gridManager.GridSelecter(x, x);
            SaveSystem.DeleteSaveGame();
            int currentLevelIndex = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadSceneAsync(currentLevelIndex, LoadSceneMode.Single);
        }
    }
}
