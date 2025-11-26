using UnityEngine;

[RequireComponent(typeof(EnemyHealth))]
[RequireComponent(typeof(EnemyAnimator))]
public class EnemyAI : MonoBehaviour
{
    [Header("Flyweight Data")]
    public EnemyData enemyData;
    [Header("Patrulla")]
    public Transform[] patrolPoints;

    [Header("Configuración de Patrulla")]
    public float patrolPointTolerance = 0.5f;
    public float idleWaitTime = 2f;

    public Transform player { get; private set; }
    public float MoveSpeed => enemyData.moveSpeed;
    public float RunSpeed => enemyData.runSpeed;
    public float StopDistance => enemyData.stopDistance;
    public float DetectionRange => enemyData.detectionRange;
    public float AttackRange => enemyData.attackRange;

    private EnemyHealth enemyHealth;
    private IEnemyAnimator enemyAnimator;
    private IEnemyBehavior currentBehavior;

    void Start()
    {
        enemyHealth = GetComponent<EnemyHealth>();
        enemyAnimator = GetComponent<EnemyAnimator>();

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
            SetBehavior(new EnemyPatrolBehavior());
        else
            SetBehavior(new EnemyIdleBehavior());
    }

    void Update()
    {
        if (enemyHealth.IsDead) return;
        currentBehavior?.Execute(this);
    }

    public void SetBehavior(IEnemyBehavior newBehavior)
    {
        currentBehavior?.OnExit(this);
        currentBehavior = newBehavior;
        currentBehavior?.OnEnter(this);
    }

    public IEnemyAnimator GetAnimator()
    {
        return enemyAnimator;
    }
}