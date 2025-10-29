using UnityEngine;

public class NPCDialogue : MonoBehaviour
{
    [TextArea(3, 6)]
    public string dialogueText = "Hola viajero. Es peligroso andar solo por aqu�...";

    [Header("Configuraci�n")]
    public float interactionRange = 3f;

    // M�todo para obtener el di�logo
    public string GetDialogue()
    {
        return dialogueText;
    }
}
