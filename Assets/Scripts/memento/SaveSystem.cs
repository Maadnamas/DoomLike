using UnityEngine;
using System.IO;
using System.Collections.Generic;

public static class SaveSystem
{
    private static string savePath = Application.persistentDataPath + "/save.json";

    public static void SaveGame(GameMemento memento)
    {

        Debug.LogWarning("SaveGame con GameMemento necesita implementación actualizada");
    }

    public static GameMemento LoadGame()
    {
        if (!File.Exists(savePath))
        {
            Debug.LogWarning("No hay archivo de guardado.");
            return null;
        }

        Debug.LogWarning("LoadGame con GameMemento necesita implementación actualizada");
        return null;
    }
}