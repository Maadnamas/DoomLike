using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyData", menuName = "Enemy/Data")]
public class EnemyData : ScriptableObject
{
    [Header("Stats Base")]
    public float maxHealth = 100f;
    public float moveSpeed = 3f;
    public float runSpeed = 6f;
    public float damage = 10f;
    public bool useGravity = true; // True para caminantes, False para torretas

    [Header("Rango y Detección")]
    public float detectionRange = 20f;
    public float stopDistance = 2f;
    [Range(0, 360)] public float fieldOfView = 90f;
    public float proximityDetectionRadius = 3f;

    [Header("Attack Settings")]
    public float attackCooldown = 1.5f;
    public float attackRange = 2f; // Distancia para empezar a explotar o golpear

    [Header("Ranged Attack (Shooter)")]
    public GameObject projectilePrefab;
    public float projectileSpeed = 20f;
    public float shootRange = 15f;

    [Header("Kamikaze / Explosion Stats")] // --- ESTO ES LO QUE TE FALTA ---
    public float explosionRadius = 5f;
    public float explosionDamage = 40f;
    public float explosionForce = 20f;     // Fuerza de empuje al Player
    public float fuseTime = 0.5f;          // Tiempo de "mecha" (animación antes de explotar)
    public GameObject explosionVFX;        // Prefab de partículas
    public AudioClip fuseSound;            // Sonido de carga (sssss)
    public AudioClip explosionSound;       // BOOM

    [Header("Sonidos")]
    public AudioClip walkSound;
    public AudioClip deathSound;
    public AudioClip hitSound;
}