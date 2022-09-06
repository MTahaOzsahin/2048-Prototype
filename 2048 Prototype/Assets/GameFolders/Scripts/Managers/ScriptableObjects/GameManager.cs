using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Prototype.Scripts.Managers.ScriptableObjects
{
    [CreateAssetMenu(menuName ="Managers/GameManager")]
    public class GameManager : ScriptableObject
    {
        public void ExitApplication()
        {
            Application.Quit();
        }
        public void Restart()
        {
            int currentLevelIndex = SceneManager.GetActiveScene().buildIndex;
            SceneManager.UnloadSceneAsync(currentLevelIndex);
            SceneManager.LoadSceneAsync(currentLevelIndex);
        }
    }
}
