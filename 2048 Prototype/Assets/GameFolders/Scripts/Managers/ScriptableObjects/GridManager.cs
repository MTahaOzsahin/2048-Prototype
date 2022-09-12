using Prototype.Scripts.Grid;
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

        //Node prefab. Will select manually on unity inspector.
        public Node nodePrefab;

        //Block prefab. Will select manually on unity inspector.
        public Block blockPrefab;

        //Board prefabs's spriteRenderer for instantiate.
        public SpriteRenderer boardPrefab;

        //Shifting anim time.Recomended time is 0.2.
        [Range(0f, 1f)]
        public float travelTime = 0.2f;

        //The score to get win screen.
        public int winCondition = 1024;

        //Win and lose panels.
        public GameObject winPanel;
        public GameObject losePanel;



        private void Awake()
        {
            gridHeight = 4;
            gridWidth = 4;
            winCondition = 1024;
        }

        //Changing grid layout from ui grid button.
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

        //Only for if player want to keep playing after winning the game. Used in ui keep going button.
        public void WinConditionDiseabler()
        {
            winCondition = 99999;
        }
        public void TimeScaler()
        {
            Time.timeScale = 1;
        }
    }
}
