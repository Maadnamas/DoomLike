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
    public int numberOfSpikes = 8; // Cantidad de pinchos alrededor
    public float spikeRadius = 5f; // Radio donde aparecen los pinchos
    public float spikeWarningTime = 1f; // Tiempo que muestran las partículas antes de salir
    public float spikeDamage = 15f;
    public float spikeAttackCooldown = 8f;
    public float spikeAttackMinDistance = 6f; // Distancia mínima del jugador para usar este ataque
}