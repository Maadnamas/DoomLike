using UnityEngine;

public class DoorOpener : MonoBehaviour
{
    [Header("Configuración")]
    public float moveDistance = 10f;   // Unidades que sube la puerta
    public float speed = 3f;           // Velocidad del movimiento

    [Header("Audio (Opcional)")]
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
            Debug.Log("Puerta comenzando a abrirse");

            // Reproducir sonido de puerta si está asignado
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

            // Si llegó, dejar de mover
            if (transform.position == targetPos)
            {
                opening = false;
                Debug.Log("Puerta completamente abierta");
            }
        }
    }
}