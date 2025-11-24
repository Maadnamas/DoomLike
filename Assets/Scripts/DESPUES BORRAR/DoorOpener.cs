using UnityEngine;

public class DoorOpener : MonoBehaviour
{
    [Header("Configuración")]
    public float delay = 47f;          // Tiempo antes de abrir
    public float moveDistance = 10f;   // Unidades que sube la puerta
    public float speed = 3f;           // Velocidad del movimiento

    private Vector3 initialPos;
    private Vector3 targetPos;
    private bool opening = false;

    void Start()
    {
        initialPos = transform.position;
        targetPos = initialPos + Vector3.up * moveDistance;

        Invoke(nameof(StartOpening), delay);
    }

    void StartOpening()
    {
        opening = true;
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
                opening = false;
        }
    }
}
