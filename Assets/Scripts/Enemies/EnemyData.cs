using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Enemies/New Enemy Data")]
public class EnemyData : ScriptableObject
{
    [Header("Stats Base")]
    public float maxHealth = 100f;
    public float moveSpeed = 3f;
    public float runSpeed = 6f;
    public float damage = 10f;
    public float detectionRange = 15f;
    public float stopDistance = 2f;
    public string enemyType = "Default";

    [Header("Attack Settings")]
    public float attackCooldown = 1.5f;
    public float attackRange = 2.5f;
}