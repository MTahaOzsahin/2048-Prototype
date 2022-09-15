using Prototype.Scripts.Grid;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Prototype.Scripts.Managers.ScriptableObjects
{
    [CreateAssetMenu(menuName ="Managers/DataMAnager")]
    public class DataManager : ScriptableObject
    {
        public ScoreManager scoreManager;

        [HideInInspector]
        public int highScore;

        //Ordered blocks that on the grid list when we get at closing game.
        public List<Block> allBlocksBeforeSave;

        //List to GameplayManager to recreate same grid.
        public List<Vector2> savedPositions;
        public List<int> savedValues;
        
        public void GettingGBlocks(List<Block> blocksOnQuit)
        {
            highScore = scoreManager.bestScore;
            allBlocksBeforeSave = new List<Block>();
            allBlocksBeforeSave = blocksOnQuit;
            SaveGame();
        }

        public List<Vector2> GivingBlocksPos()
        {
            LoadGame();
            return savedPositions;
        }
        public List<int> GivingBlockValue()
        {
            LoadGame();
            return savedValues;
        }


        public void SaveGame()
        {
            SaveSystem.SaveGame(this);
        }
        public void LoadGame()
        {
            savedPositions = new List<Vector2>();
            savedValues = new List<int>();
            PlayerData data = SaveSystem.LoadGame();
            scoreManager.bestScore = data.highScore;
            for (int i = 0; i < allBlocksBeforeSave.Count; i++)
            {
                Vector2 vector2 = new Vector2();
                vector2.x = data.xposition[i];
                vector2.y = data.yposition[i];
                savedPositions.Add(vector2);
                int value = new int();
                value = data.value[i];
                savedValues.Add(value);
            }
        }


        
    }
}
