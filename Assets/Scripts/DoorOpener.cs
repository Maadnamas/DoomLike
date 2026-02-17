using UnityEngine;

public class DoorOpener : MonoBehaviour
{
    [Header("Settings")]
    public float moveDistance = 10f;
    public float speed = 3f;

    [Header("Audio (Optional)")]
    [SerializeField] private AudioClip doorOpenSound;

    private Vector3 initialPos;
    private Vector3 targetPos;
    private bool opening = false;

    void Start()
    {
        initialPos = transform.position;
        targetPos = initialPos + Vector3.up * moveDistance;
    }

    public void OpenDoor()
    {
        if (!opening)
        {
            opening = true;
            Debug.Log("Door starting to open");

            if (doorOpenSound != null)
            {
                AudioManager.PlaySFX3D(doorOpenSound, transform.position);
            }
        }
    }

    void Update()
    {
        if (opening)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPos,
                speed * Time.deltaTime
            );

            if (transform.position == targetPos)
            {
                opening = false;
                Debug.Log("Door fully open");
            }
        }
    }
}