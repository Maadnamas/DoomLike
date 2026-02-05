using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Enemies/New Enemy Data")]
public class EnemyData : ScriptableObject
{
    [Header("Stats Base")]
    public float maxHealth = 100f;
    public float moveSpeed = 3f;
    public float runSpeed = 6f;
    public float damage = 10f;

    public bool useGravity = true;

    [Header("Rango y Detección")]
    public float detectionRange = 15f;
    public float stopDistance = 2f;
    [Range(0, 360)] public float fieldOfView = 90f;
    public float proximityDetectionRadius = 3f;

    [Header("Attack Settings")]
    public float attackCooldown = 1.5f;
    public float attackRange = 2.5f;

    [Header("Ranged Attack (Shooter)")]
    public GameObject projectilePrefab; 
    public float projectileSpeed = 20f;
    public float shootRange = 15f;      

    public string enemyType = "Default";

    [Header("Sonidos")]
    public AudioClip walkSound;
    public AudioClip deathSound;
    public AudioClip hitSound;
}