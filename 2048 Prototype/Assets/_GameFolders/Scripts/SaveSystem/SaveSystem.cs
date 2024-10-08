using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using Prototype.Scripts.Managers.ScriptableObjects;

public static class SaveSystem
{
    public static void SaveGame(DataManager dataManager)
    {
        BinaryFormatter formatter = new BinaryFormatter();

        string  filePath = Application.persistentDataPath + "/save.data";

        FileStream stream = new FileStream(filePath, FileMode.Create);

        PlayerData data = new PlayerData(dataManager);

        formatter.Serialize(stream, data);

        stream.Close();
    }

    public static PlayerData LoadGame ()
    {
        string filePath = Application.persistentDataPath + "/save.data";
        if (File.Exists(filePath))
        {
            BinaryFormatter formatter = new BinaryFormatter();

            FileStream stream = new FileStream(filePath, FileMode.Open);

            PlayerData data =  formatter.Deserialize(stream) as PlayerData;

            stream.Close();

            return data;
        }

        return null;
    }
    /// <summary>
    /// Deleting saved game datas.
    /// </summary>
    public static void DeleteSaveGame()
    {
        string filePath = Application.persistentDataPath + "/save.data";
        try
        {
            File.Delete(filePath);
        }
        catch (System.Exception ex)
        {
            Debug.LogException(ex);
        }
    }
    
}
