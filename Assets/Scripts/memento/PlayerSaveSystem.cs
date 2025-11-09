using UnityEngine;
using System.IO;

public static class PlayerSaveSystem
{
    private static string path = Application.persistentDataPath + "/player_save.json";

    public static void Save(PlayerMemento memento)
    {
        string json = JsonUtility.ToJson(memento, true);
        File.WriteAllText(path, json);
        Debug.Log("Partida guardada en: " + path);
    }

    public static PlayerMemento Load()
    {
        if (!File.Exists(path))
        {
            Debug.LogWarning("No se encontró guardado en " + path);
            return null;
        }

        string json = File.ReadAllText(path);
        return JsonUtility.FromJson<PlayerMemento>(json);
    }
}
