using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class GameMemento
{
    public Dictionary<string, object> gameData = new Dictionary<string, object>();

    public GameMemento() { }

    public GameMemento(Dictionary<string, object> data)
    {
        gameData = data;
    }

    public void SetValue(string key, object value)
    {
        if (gameData.ContainsKey(key))
            gameData[key] = value;
        else
            gameData.Add(key, value);
    }

    public T GetValue<T>(string key, T defaultValue = default(T))
    {
        if (gameData.ContainsKey(key) && gameData[key] is T)
            return (T)gameData[key];
        return defaultValue;
    }
}