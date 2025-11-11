using System.Collections.Generic;

[System.Serializable]
public class CardSaveData
{
    public string cardID;
    public bool owned;
    public bool isFoil;

    public CardSaveData(string id, bool owned, bool foil)
    {
        cardID = id;
        this.owned = owned;
        isFoil = foil;
    }
}

[System.Serializable]
public class CardSaveWrapper
{
    public List<CardSaveData> cards = new List<CardSaveData>();
}