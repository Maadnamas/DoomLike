using UnityEngine;

[RequireComponent(typeof(EnemyHealth))]
public class EnemyAI : MonoBehaviour
{
    [Header("Movimiento")]
    public float moveSpeed = 3f;
    public float stopDistance = 2f; // distancia mínima al jugador

    [Header("Detección")]
    public float detectionRange = 15f;
    public Transform player;

    private EnemyHealth enemyHealth;

    void Start()
    {
        enemyHealth = GetComponent<EnemyHealth>();

        // Si no se asignó el jugador, lo buscamos automáticamente
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
        }
    }

    void Update()
    {
        if (enemyHealth.IsDead) return; // no hacer nada si está muerto

        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= detectionRange)
        {
            // Mirar hacia el jugador
            Vector3 direction = (player.position - transform.position).normalized;
            direction.y = 0f; // mantenerlo en plano horizontal
            transform.rotation = Quaternion.LookRotation(direction);

            // Mover hacia el jugador solo si está más lejos del "stopDistance"
            if (distance > stopDistance)
            {
                transform.position += direction * moveSpeed * Time.deltaTime;
            }
        }
    }
}
