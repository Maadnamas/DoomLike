using UnityEngine;

[CreateAssetMenu(fileName = "BossData", menuName = "Enemies/New Boss Data")]
public class BossData : ScriptableObject
{
    [Header("Stats Base")]
    public float maxHealth = 500f;
    public float moveSpeed = 2f;
    public float damage = 20f;

    [Header("Detección")]
    public float detectionRange = 30f;
    public float stopDistance = 3f;

    [Header("Ataque Melee")]
    public float meleeAttackCooldown = 2f;
    public float meleeAttackRange = 4f;

    [Header("Ataque de Pinchos")]
    public GameObject spikePrefab;
    public GameObject spikeWarningPrefab; // Partículas de aviso

    [Tooltip("Cantidad de pinchos que aparecen")]
    public int numberOfSpikes = 12; // Aumentado para más caos

    [Tooltip("Radio máximo donde pueden aparecer los pinchos (desde el centro del boss)")]
    public float spikeRadius = 8f; // Radio del área de spawn

    [Tooltip("Radio mínimo donde pueden aparecer (para no spawnear encima del boss)")]
    [Range(0f, 10f)]
    public float spikeMinRadius = 2f; // Nuevo: radio mínimo

    [Tooltip("Tiempo que muestran las partículas antes de que salga el pincho")]
    public float spikeWarningTime = 1f;

    public float spikeDamage = 15f;
    public float spikeAttackCooldown = 8f;

    [Tooltip("Distancia mínima del jugador para preferir este ataque")]
    public float spikeAttackMinDistance = 6f;

    [Header("Configuración Avanzada de Pinchos")]
    [Tooltip("Si está activado, los pinchos pueden aparecer cerca unos de otros")]
    public bool allowOverlappingSpikes = false;

    [Tooltip("Distancia mínima entre pinchos (si overlapping está desactivado)")]
    public float minDistanceBetweenSpikes = 1.5f;
}