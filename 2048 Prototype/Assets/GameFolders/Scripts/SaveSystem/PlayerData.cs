using Prototype.Scripts.Managers.ScriptableObjects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public float[] xposition;
    public float[] yposition;
    public int[] value;
    public int highScore;
    public PlayerData(DataManager dataManager)
    {
        xposition = new float[36];
        yposition = new float[36];
        value = new int[36];
        highScore = new int();
        highScore = dataManager.highScore;
        for (int i = 0; i < dataManager.allBlocksBeforeSave.Count; i++)
        {
            value[i] = dataManager.allBlocksBeforeSave[i].value;
            xposition[i] = dataManager.allBlocksBeforeSave[i].Pos.x;
            yposition[i] = dataManager.allBlocksBeforeSave[i].Pos.y;
        }
    }
}
