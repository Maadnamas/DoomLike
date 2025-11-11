using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.IO;

public class CardCollectionManager : MonoBehaviour
{
    public static CardCollectionManager Instance;

    [Header("Todas las cartas disponibles en el juego (ScriptableObjects)")]
    public List<CardData> allCards = new List<CardData>();

    [Header("Progreso guardado")]
    public List<CardSaveData> ownedCards = new List<CardSaveData>();

    private string savePath;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            savePath = Path.Combine(Application.persistentDataPath, "cardsave.json");
            LoadCollection();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddCard(CardData card)
    {
        var existing = ownedCards.FirstOrDefault(c => c.cardID == card.cardID);

        if (existing == null)
        {
            ownedCards.Add(new CardSaveData(card.cardID, true, card.isFoil));
        }
        else
        {
            existing.owned = true;
            existing.isFoil = card.isFoil;
        }

        SaveCollection();
    }

    public bool HasCard(string cardID)
    {
        return ownedCards.Any(c => c.cardID == cardID && c.owned);
    }

    public void SaveCollection()
    {
        var wrapper = new CardSaveWrapper { cards = ownedCards };
        string json = JsonUtility.ToJson(wrapper, true);
        File.WriteAllText(savePath, json);
    }

    public void LoadCollection()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            var wrapper = JsonUtility.FromJson<CardSaveWrapper>(json);
            ownedCards = wrapper.cards;
        }
        else
        {
            ownedCards = new List<CardSaveData>();
        }


    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F10))
        {
            DeleteCollection();
        }
    }


    public void DeleteCollection()
    {
        ownedCards.Clear();

        if (File.Exists(savePath))
        {
            File.Delete(savePath);
        }

        Debug.Log("datos de cartas borrados correctamente.");
    }
}