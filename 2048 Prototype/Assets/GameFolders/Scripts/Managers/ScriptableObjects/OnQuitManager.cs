using Prototype.Scripts.Grid;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Prototype.Scripts.Managers.ScriptableObjects
{
    [CreateAssetMenu(menuName ="Managers/OnQuitManager")]
    public class OnQuitManager : ScriptableObject
    {
        public List<Block> allBlocks;
        public List<Vector2> allBlocksPos;
        public List<int> allBlocksValue;
        
        public void GettingGBlocks(List<Block> blocksOnQuit)
        {
            allBlocks = new List<Block>();
            allBlocksPos = new List<Vector2>();
            allBlocksValue = new List<int>();
            allBlocks = blocksOnQuit;
            foreach (var blocks in allBlocks)
            {
                allBlocksPos.Add(blocks.Pos);
                allBlocksValue.Add(blocks.value);
            }

            
        }
    }
}
