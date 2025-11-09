using System;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    private static EventManager _instance;
    public static EventManager Instance => _instance;

    private Dictionary<string, Action<object>> eventDictionary;

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            eventDictionary = new Dictionary<string, Action<object>>();
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static void StartListening(string eventName, Action<object> listener)
    {
        if (Instance.eventDictionary.TryGetValue(eventName, out Action<object> thisEvent))
        {
            thisEvent += listener;
            Instance.eventDictionary[eventName] = thisEvent;
        }
        else
        {
            thisEvent += listener;
            Instance.eventDictionary.Add(eventName, thisEvent);
        }
    }

    public static void StopListening(string eventName, Action<object> listener)
    {
        if (_instance == null) return;
        if (Instance.eventDictionary.TryGetValue(eventName, out Action<object> thisEvent))
        {
            thisEvent -= listener;
            Instance.eventDictionary[eventName] = thisEvent;
        }
    }

    public static void TriggerEvent(string eventName, object eventParam)
    {
        if (Instance.eventDictionary.TryGetValue(eventName, out Action<object> thisEvent))
        {
            thisEvent?.Invoke(eventParam);
        }
    }
}