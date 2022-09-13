using Prototype.Scripts.Grid;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Prototype.Scripts.Managers.ScriptableObjects
{
    [CreateAssetMenu(menuName ="Managers/OnQuitManager")]
    public class OnQuitManager : ScriptableObject
    {
        public List<Block> allBlocks;
        
        public void GettingGBlocks(List<Block> blocksOnQuit)
        {
            SaveData saveData = new SaveData();
            allBlocks = new List<Block>();
            allBlocks = blocksOnQuit;

            //foreach (var blocks in allBlocks)
            //{
            //    saveData.allBlocksPos.Add(blocks.Pos);
            //    saveData.allBlocksValue.Add(blocks.value);
            //}
            for (int i = 0; i < allBlocks.Count; i++)
            {
                saveData.allBlocksPos[i].Set(allBlocks[i].Pos.x, allBlocks[i].Pos.y);
                saveData.allBlocksValue[i] = allBlocks[i].value;
            }
           

            string saveDataJson = JsonUtility.ToJson(saveData.allBlocksPos);

            File.WriteAllText(Application.dataPath + "/saveDataJson.json", saveDataJson);

            
        }

        public void GivingBlocks(List<Vector2> positions,List<int> values)
        {
            if (File.Exists(Application.dataPath + "/saveDataJson.json"))
            {
                string jsonSavedPos = File.ReadAllText(Application.dataPath + "/savedPosFile.josn");
                string jsonSavedValue = File.ReadAllText(Application.dataPath + "/savedValueFile.json");
                List<Vector2> blocksPos = new List<Vector2>();
                blocksPos = JsonUtility.FromJson<List<Vector2>>(jsonSavedPos);
                positions = blocksPos;
                List<int> blocksValue = new List<int>();
                blocksValue = JsonUtility.FromJson<List<int>>(jsonSavedValue);
                values = blocksValue;
            }
            else
            {
                Debug.Log("There is no json file");
            }
        }

        [System.Serializable]
        public class SaveData
        {
            public Vector2[] allBlocksPos = new Vector2[8];
            public int[] allBlocksValue = new int[8];
        }
    }
}
