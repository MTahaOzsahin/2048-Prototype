using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Prototype.Scripts.Managers.ScriptableObjects
{
    [CreateAssetMenu(menuName ="Managers/GridManager")]
    public class GridManager : ScriptableObject
    {
        public int gridWidth;
        public int gridHeight;

        public int winCondition = 1024;
        public void GridSelecter(int width, int height)
        {
            gridWidth = width;
            gridHeight = height;
            if (width == 4)
            {
                winCondition = 1024;
            }
            else if (width == 5)
            {
                winCondition = 2048;
            }
            else
            {
                winCondition = 4096;
            }
        }
        public void WinConditionDiseabler()
        {
            winCondition = 99999;
        }
    }
}
