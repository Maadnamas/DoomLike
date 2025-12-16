using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class LarvaManager : MonoBehaviour
{
    public List<LarvaMedkit> larvas = new List<LarvaMedkit>();
    private Dictionary<string, bool> larvaCollectionState = new Dictionary<string, bool>();

    public static LarvaManager Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        InitializeLarvas();
    }

    void InitializeLarvas()
    {
        if (larvas.Count == 0)
        {
            LarvaMedkit[] foundLarvas = FindObjectsOfType<LarvaMedkit>(true);
            larvas.AddRange(foundLarvas);
        }

        foreach (LarvaMedkit larva in larvas)
        {
            if (larva != null)
            {
                string key = GetLarvaKey(larva);
                larvaCollectionState[key] = false;
                larva.gameObject.SetActive(true);
            }
        }
    }

    string GetLarvaKey(LarvaMedkit larva)
    {
        return larva.gameObject.GetInstanceID().ToString();
    }

    public void MarkLarvaAsCollected(LarvaMedkit larva)
    {
        if (larva == null) return;

        string key = GetLarvaKey(larva);
        larvaCollectionState[key] = true;
    }

    public void SaveLarvaStates(PlayerMemento memento)
    {
        List<string> collectedKeys = new List<string>();

        foreach (var kvp in larvaCollectionState)
        {
            if (kvp.Value)
            {
                collectedKeys.Add(kvp.Key);
            }
        }

        string serializedKeys = string.Join("|", collectedKeys.ToArray());
        memento.SetAdditionalData("collectedLarvas", serializedKeys);
    }

    public void LoadLarvaStates(PlayerMemento memento)
    {
        string serializedKeys = memento.GetAdditionalData<string>("collectedLarvas", "");

        // Resetear
        foreach (var key in larvaCollectionState.Keys.ToList())
        {
            larvaCollectionState[key] = false;
        }

        // Marcar recolectados
        if (!string.IsNullOrEmpty(serializedKeys))
        {
            string[] collectedKeys = serializedKeys.Split('|');
            foreach (string key in collectedKeys)
            {
                if (larvaCollectionState.ContainsKey(key))
                {
                    larvaCollectionState[key] = true;
                }
            }
        }

        // Aplicar estados
        ApplyLarvaStates();
    }

    void ApplyLarvaStates()
    {
        foreach (LarvaMedkit larva in larvas)
        {
            if (larva != null)
            {
                string key = GetLarvaKey(larva);

                if (larvaCollectionState.ContainsKey(key))
                {
                    larva.gameObject.SetActive(!larvaCollectionState[key]);
                }
            }
        }
    }
}