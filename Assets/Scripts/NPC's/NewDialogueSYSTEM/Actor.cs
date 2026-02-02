using UnityEngine;

public class Actor : MonoBehaviour
{
    public string Name;
    public Dialogue Dialogue;

    [Header("Configuración Interacción")]
    public Transform player;       // Arrastra a tu Jugador (Player) aquí
    public float interactionRange = 3.0f; // Distancia máxima para hablar

    private void Update()
    {
        // 1. Calculamos distancia
        float distance = Vector3.Distance(transform.position, player.position);

        // 2. Si está cerca Y presiona E Y no hay otro diálogo abierto
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
        DialogueManager.Instance.StartDialogue(Name, Dialogue.RootNode);
    }

    // Dibujito amarillo en el editor para ver el rango
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}