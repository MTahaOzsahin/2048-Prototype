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
        public void ExitApplication()
        {
            Application.Quit();
        }
        public void Restart()
        {
            scoreManager.currentScore = 0;
            int currentLevelIndex = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadSceneAsync(currentLevelIndex,LoadSceneMode.Single);
        }
    }
}
