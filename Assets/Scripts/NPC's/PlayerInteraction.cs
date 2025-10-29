using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Configuración")]
    public KeyCode interactKey = KeyCode.E;
    public float detectionRadius = 3f;
    public LayerMask npcLayer;

    private NPCDialogue currentNPC;

    void Update()
    {
        // Si ya estás en diálogo
        if (DialogueManager.Instance.IsDialogueActive())
        {
            if (Input.GetKeyDown(interactKey))
            {
                DialogueManager.Instance.EndDialogue();
            }
            return;
        }

        DetectNearbyNPC();

        // Si hay un NPC cerca y presionás E, iniciar diálogo
        if (currentNPC != null && Input.GetKeyDown(interactKey))
        {
            DialogueManager.Instance.StartDialogue(currentNPC.GetDialogue());
        }
    }

    void DetectNearbyNPC()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius, npcLayer);

        if (hits.Length > 0)
        {
            currentNPC = hits[0].GetComponent<NPCDialogue>();
        }
        else
        {
            currentNPC = null;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
