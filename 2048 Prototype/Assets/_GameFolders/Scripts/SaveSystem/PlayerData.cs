using Prototype.Scripts.Managers.ScriptableObjects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
///Using only during saving games.
public class PlayerData
{
    public float[] xposition;
    public float[] yposition;
    public int[] value;
    public int highScore;
    public int currentScore;
    public int nodeNumber;
    public PlayerData(DataManager dataManager)
    {
        xposition = new float[dataManager.allActiveBlockNumber];
        yposition = new float[dataManager.allActiveBlockNumber];
        value = new int[dataManager.allActiveBlockNumber];
        highScore = new int();
        highScore = dataManager.highScore;
        currentScore = new int();
        currentScore = dataManager.currentScore;
        nodeNumber = new int();
        nodeNumber = dataManager.nodeNumber;
        for (int i = 0; i < dataManager.allActiveBlockNumber; i++)
        {
            value[i] = dataManager.allActiveBlocks[i].value;
            xposition[i] = dataManager.allActiveBlocks[i].Pos.x;
            yposition[i] = dataManager.allActiveBlocks[i].Pos.y;
        }
    }
}
