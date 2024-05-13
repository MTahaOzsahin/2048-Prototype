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
        
        //Number of active blocks on the grid when this field called.
        public int allActiveBlockNumber = 0;

        //List of active blocks and nodes on the grid when this field called.
        public List<Block> allActiveBlocks = new List<Block>();
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


        public void SaveGame()
        {
            SaveSystem.SaveGame(this);
        }
        public void LoadGame()
        {
            savedPositions = new List<Vector2>();
            savedValues = new List<int>();
            PlayerData data = SaveSystem.LoadGame();
            if (data == null) return;
            scoreManager.bestScore = data.highScore;
            nodeNumber = data.nodeNumber;
            for (int i = 0; i < data.value.Length; i++)
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
