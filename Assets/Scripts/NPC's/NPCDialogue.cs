using UnityEngine;

public class NPCDialogue : MonoBehaviour
{
    [TextArea(3, 6)]
    public string dialogueText = "Hola viajero. Es peligroso andar solo por aquí...";

    [Header("Configuración")]
    public float interactionRange = 3f;

    // Método para obtener el diálogo
    public string GetDialogue()
    {
        return dialogueText;
    }
}
