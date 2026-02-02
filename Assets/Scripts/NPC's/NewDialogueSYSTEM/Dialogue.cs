using TMPro;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue/Dialogue Asset")]
public class Dialogue : ScriptableObject
{
    [Header("Configuración Visual")]
    public TMP_FontAsset npcFont; // Fuente específica
    public Sprite npcPortrait;   // Imagen/Cara del NPC

    [Header("Nodos")]
    public DialogueNode RootNode;
}