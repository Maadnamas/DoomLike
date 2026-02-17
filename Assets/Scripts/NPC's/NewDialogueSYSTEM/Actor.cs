using UnityEngine;

public class Actor : MonoBehaviour
{
    public Dialogue Dialogue;

    [Header("Interaction Configuration")]
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
        DialogueManager.Instance.StartDialogue(Dialogue);
    }

}