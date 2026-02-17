using UnityEngine;
using System.IO;
using System.Collections.Generic;

public static class SaveSystem
{
    private static string savePath = Application.persistentDataPath + "/save.json";

    public static void SaveGame(GameMemento memento)
    {
        Debug.LogWarning("SaveGame with GameMemento needs updated implementation");
    }

    public static GameMemento LoadGame()
    {
        if (!File.Exists(savePath))
        {
            Debug.LogWarning("No save file found.");
            return null;
        }

        Debug.LogWarning("LoadGame with GameMemento needs updated implementation");
        return null;
    }
}