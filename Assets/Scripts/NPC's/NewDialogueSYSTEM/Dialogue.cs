using TMPro;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue/Dialogue Asset")]
public class Dialogue : ScriptableObject
{
    [Header("General Configuration")]
    public string npcName;

    [Header("Visual Configuration")]
    public TMP_FontAsset npcFont;

    public Sprite npcPortraitClosed;
    public Sprite npcPortraitOpen;

    [Header("Nodes")]
    public DialogueNode RootNode;
}