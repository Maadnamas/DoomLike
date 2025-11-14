using UnityEngine;

[CreateAssetMenu(fileName = "NewCard", menuName = "Collectibles/Card")]
public class CardData : ScriptableObject
{
    public string cardID;
    public Texture2D cardTexture;
    public bool isFoil;
}