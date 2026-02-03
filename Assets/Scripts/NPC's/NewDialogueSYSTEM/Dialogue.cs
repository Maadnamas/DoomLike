using TMPro;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue/Dialogue Asset")]
public class Dialogue : ScriptableObject
{
    [Header("Configuración Visual")]
    public TMP_FontAsset npcFont;

    public Sprite npcPortraitClosed; // Boca cerrada
    public Sprite npcPortraitOpen;   // Boca abierta

    [Header("Nodos")]
    public DialogueNode RootNode;
}