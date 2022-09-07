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
        public void GridSelecter(int width, int height)
        {
            gridWidth = width;
            gridHeight = height;
        }
    }
}
