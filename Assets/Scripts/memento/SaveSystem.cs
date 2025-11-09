using UnityEngine;
using System.IO;

public static class SaveSystem
{
    private static string savePath = Application.persistentDataPath + "/save.json";

    public static void SaveGame(GameMemento memento)
    {
        string json = JsonUtility.ToJson(memento, true);
        File.WriteAllText(savePath, json);
        Debug.Log("Partida guardada en: " + savePath);
    }

    public static GameMemento LoadGame()
    {
        if (!File.Exists(savePath))
        {
            Debug.LogWarning("No hay archivo de guardado.");
            return null;
        }

        string json = File.ReadAllText(savePath);
        GameMemento memento = JsonUtility.FromJson<GameMemento>(json);
        Debug.Log("Partida cargada desde: " + savePath);
        return memento;
    }
}
