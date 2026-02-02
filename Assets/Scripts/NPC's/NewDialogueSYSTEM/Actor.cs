using UnityEngine;

public class Actor : MonoBehaviour
{
    public string Name;
    public Dialogue Dialogue;

    [Header("Configuración Interacción")]
    public Transform player;
    public float interactionRange = 3.0f;

    private void Update()
    {
        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= interactionRange && Input.GetKeyDown(KeyCode.E))
        {
            if (!DialogueManager.Instance.IsDialogueActive())
            {
                SpeakTo();
            }
        }
    }

    public void SpeakTo()
    {
        // Pasamos nombre, nodo inicial, fuente y el retrato
        DialogueManager.Instance.StartDialogue(Name, Dialogue.RootNode, Dialogue.npcFont, Dialogue.npcPortrait);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}