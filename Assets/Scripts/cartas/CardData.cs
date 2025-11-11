using UnityEngine;

[CreateAssetMenu(fileName = "NewCard", menuName = "Collectibles/Card")]
public class CardData : ScriptableObject
{
    public string cardID;
    public Sprite image;
    public bool isFoil;
}