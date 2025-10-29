using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    [Header("Referencias UI")]
    public GameObject dialoguePanel;
    public Text dialogueText;

    [Header("HUD del jugador (Canvas con el arma o manos)")]
    public GameObject playerHUD; //  Asignalo desde el inspector

    private bool isDialogueActive = false;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        dialoguePanel.SetActive(false);
    }

    public void StartDialogue(string npcText)
    {
        if (isDialogueActive) return;

        isDialogueActive = true;
        dialoguePanel.SetActive(true);
        dialogueText.text = npcText;

        // Ocultar HUD del jugador si está asignado
        if (playerHUD != null)
            playerHUD.SetActive(false);

        // Pausar el tiempo
        Time.timeScale = 0f;
    }

    public void EndDialogue()
    {
        if (!isDialogueActive) return;

        isDialogueActive = false;
        dialoguePanel.SetActive(false);

        // Mostrar HUD del jugador si está asignado
        if (playerHUD != null)
            playerHUD.SetActive(true);

        // Reanudar el tiempo
        Time.timeScale = 1f;
    }

    public bool IsDialogueActive()
    {
        return isDialogueActive;
    }
}
