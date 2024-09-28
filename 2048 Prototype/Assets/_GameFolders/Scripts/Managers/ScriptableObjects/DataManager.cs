using Prototype.Scripts.Grid;
using System.Collections.Generic;
using UnityEngine;

namespace Prototype.Scripts.Managers.ScriptableObjects
{
    [CreateAssetMenu(menuName ="Managers/DataManager")]
    public class DataManager : ScriptableObject
    {
        public ScoreManager scoreManager;

        [HideInInspector]
        public int highScore;

        [HideInInspector]
        public int currentScore;
        
        //Number of active blocks on the grid when this field called.
        public int allActiveBlockNumber = 0;

        //List of active blocks and nodes on the grid when this field called.
        public List<Block> allActiveBlocks = new();
        public int nodeNumber;

        //List to GameplayManager to recreate same grid.
        public List<Vector2> savedPositions;
        public List<int> savedValues;

        private void OnEnable()
        {
            allActiveBlockNumber = 1; //We making this 1 for first time opening. After that this field will change according to game.
            nodeNumber = 16; //same.
        }
        public void GettingGBlocks(int activeBlocksOnQuit, List<Block> blocksOnQuit,int gridNodeNumber)
        {
            highScore = scoreManager.bestScore;
            currentScore = scoreManager.currentScore;
            allActiveBlockNumber = activeBlocksOnQuit;
            allActiveBlocks = new List<Block>();
            allActiveBlocks = blocksOnQuit;
            nodeNumber = gridNodeNumber;
            SaveGame();
        }

        public List<Vector2> GivingBlocksPos()
        {
            LoadGame();
            if (savedPositions.Count == 0)
            {
                return null;
            }
            return savedPositions;
        }
        public List<int> GivingBlockValue()
        {
            LoadGame();
            if (savedValues.Count == 0)
            {
                return null;
            }
            return savedValues;
        }
        public int NodeNumber()
        {
            LoadGame();
            return nodeNumber;
        }


        private void SaveGame()
        {
            SaveSystem.SaveGame(this);
        }

        private void LoadGame()
        {
            savedPositions = new List<Vector2>();
            savedValues = new List<int>();
            var data = SaveSystem.LoadGame();
            if (data == null) return;
            scoreManager.bestScore = data.highScore;
            scoreManager.currentScore = data.currentScore;
            nodeNumber = data.nodeNumber;
            for (int i = 0; i < data.value.Length; i++)
            {
                var vector2 = new Vector2
                {
                    x = data.xposition[i],
                    y = data.yposition[i]
                };
                savedPositions.Add(vector2);
                var value = data.value[i];
                savedValues.Add(value);
            }
        }
    }
}
