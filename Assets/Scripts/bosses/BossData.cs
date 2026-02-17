using UnityEngine;

[CreateAssetMenu(fileName = "BossData", menuName = "Enemies/New Boss Data")]
public class BossData : ScriptableObject
{
    [Header("Base Stats")]
    public float maxHealth = 500f;
    public float moveSpeed = 2f;
    public float damage = 20f;

    [Header("Detection")]
    public float detectionRange = 30f;
    public float stopDistance = 3f;

    [Header("Melee Attack")]
    public float meleeAttackCooldown = 2f;
    public float meleeAttackRange = 4f;

    [Header("Spike Attack")]
    public GameObject spikePrefab;
    public GameObject spikeWarningPrefab; // Warning particles

    [Tooltip("Amount of spikes that appear")]
    public int numberOfSpikes = 12;

    [Tooltip("Maximum radius where spikes can appear (from the boss center)")]
    public float spikeRadius = 8f;

    [Tooltip("Minimum radius where spikes can appear (to avoid spawning on top of the boss)")]
    [Range(0f, 10f)]
    public float spikeMinRadius = 2f;

    [Tooltip("Time the particles are shown before the spike emerges")]
    public float spikeWarningTime = 1f;

    public float spikeDamage = 15f;
    public float spikeAttackCooldown = 8f;

    [Tooltip("Minimum player distance to prefer this attack")]
    public float spikeAttackMinDistance = 6f;

    [Header("Advanced Spike Settings")]
    [Tooltip("If enabled, spikes can appear close to each other")]
    public bool allowOverlappingSpikes = false;

    [Tooltip("Minimum distance between spikes (if overlapping is disabled)")]
    public float minDistanceBetweenSpikes = 1.5f;
}