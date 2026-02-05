using TMPro;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue/Dialogue Asset")]
public class Dialogue : ScriptableObject
{
    [Header("Configuración General")]
    public string npcName; // <--- AQUÍ GUARDAMOS EL TÍTULO AHORA

    [Header("Configuración Visual")]
    public TMP_FontAsset npcFont;

    public Sprite npcPortraitClosed;
    public Sprite npcPortraitOpen;

    [Header("Nodos")]
    public DialogueNode RootNode;
}