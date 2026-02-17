using UnityEngine;
using System.Collections.Generic;

public class GameState : MonoBehaviour
{
    public Transform player;
    public int playerHealth = 100;
    public int playerAmmo = 10;

    public string currentLevel = "Level_01";

    private Dictionary<string, object> stateData = new Dictionary<string, object>();

    public GameMemento CreateMemento()
    {
        SetValue("playerPosition", player.position);
        SetValue("playerHealth", 100);
        SetValue("playerAmmo", 10);
        SetValue("currentLevel", "Level_01");

        return new GameMemento(new Dictionary<string, object>(stateData));
    }

    public void RestoreFromMemento(GameMemento memento)
    {
        if (memento == null) return;

        if (memento.gameData.ContainsKey("playerPosition"))
            player.position = memento.GetValue<Vector3>("playerPosition");

        playerHealth = memento.GetValue<int>("playerHealth", 100);
        playerAmmo = memento.GetValue<int>("playerAmmo", 10);
        currentLevel = memento.GetValue<string>("currentLevel", "Level_01");
    }

    public void SetValue(string key, object value)
    {
        if (stateData.ContainsKey(key))
            stateData[key] = value;
        else
            stateData.Add(key, value);
    }

    public T GetValue<T>(string key, T defaultValue = default(T))
    {
        if (stateData.ContainsKey(key) && stateData[key] is T)
            return (T)stateData[key];
        return defaultValue;
    }
}