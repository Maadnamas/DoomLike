using UnityEngine;

[RequireComponent(typeof(EnemyHealth))]
public class EnemyAI : MonoBehaviour
{
    [Header("Flyweight Data")]
    public EnemyData enemyData;
    [Header("Patrulla")]
    public Transform[] patrolPoints;

    public Transform player { get; private set; }
    public float MoveSpeed => enemyData.moveSpeed;
    public float StopDistance => enemyData.stopDistance;
    public float DetectionRange => enemyData.detectionRange;

    private EnemyHealth enemyHealth;
    private IEnemyBehavior currentBehavior;

    void Start()
    {
        enemyHealth = GetComponent<EnemyHealth>();

        if (enemyData == null)
            enemyData = ScriptableObject.CreateInstance<EnemyData>();

        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
        }

        enemyHealth.Initialize(enemyData.maxHealth);

        if (patrolPoints != null && patrolPoints.Length > 0)
            currentBehavior = new EnemyPatrolBehavior();
        else
            currentBehavior = new EnemyIdleBehavior();
    }

    void Update()
    {
        if (enemyHealth.IsDead) return;
        currentBehavior?.Execute(this);
    }

    public void SetBehavior(IEnemyBehavior newBehavior)
    {
        currentBehavior = newBehavior;
    }
}
