using UnityEngine;

[RequireComponent(typeof(BossHealth))]
[RequireComponent(typeof(EnemyAnimator))]
[RequireComponent(typeof(CharacterController))]
public class BossAI : MonoBehaviour
{
    [Header("Boss Data")]
    public BossData bossData;

    [Header("Referencias")]
    public Transform player;
    public LayerMask obstacleMask;

    public float MoveSpeed => bossData.moveSpeed;
    public float StopDistance => bossData.stopDistance;
    public float DetectionRange => bossData.detectionRange;

    private BossHealth bossHealth;
    private IEnemyAnimator bossAnimator;
    private IBossBehavior currentBehavior;
    private CharacterController characterController;
    private Vector3 currentVelocity;

    // Timers para ataques
    public float lastMeleeAttackTime { get; set; }
    public float lastSpikeAttackTime { get; set; }

    void Start()
    {
        bossHealth = GetComponent<BossHealth>();
        bossAnimator = GetComponent<EnemyAnimator>();
        characterController = GetComponent<CharacterController>();

        if (characterController == null)
            characterController = gameObject.AddComponent<CharacterController>();

        if (bossData == null)
        {
            Debug.LogError("¡Boss Data no asignado!");
            return;
        }

        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
        }

        bossHealth.Initialize(bossData.maxHealth);

        // Inicializar timers
        lastMeleeAttackTime = -bossData.meleeAttackCooldown;
        lastSpikeAttackTime = -bossData.spikeAttackCooldown;

        SetBehavior(new BossIdleBehavior());
    }

    void Update()
    {
        if (bossHealth.IsDead) return;

        currentBehavior?.Execute(this);

        // Aplicar gravedad
        if (characterController.isGrounded)
        {
            currentVelocity.y = -0.5f;
        }
        else
        {
            currentVelocity.y += Physics.gravity.y * Time.deltaTime;
        }

        characterController.Move(new Vector3(0, currentVelocity.y, 0) * Time.deltaTime);
    }

    public void MoveTo(Vector3 targetPosition, float speed)
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        direction.y = 0;

        RotateTowards(targetPosition);

        Vector3 movement = direction * speed * Time.deltaTime;
        characterController.Move(movement);
    }

    public void StopMovement()
    {
        characterController.Move(Vector3.zero);
    }

    public void RotateTowards(Vector3 targetPosition, float rotationSpeed = 5f)
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        direction.y = 0;

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
    }

    public void SetBehavior(IBossBehavior newBehavior)
    {
        currentBehavior?.OnExit(this);
        currentBehavior = newBehavior;
        currentBehavior?.OnEnter(this);
    }

    public IEnemyAnimator GetAnimator() => bossAnimator;

    public float GetDistanceToPlayer()
    {
        if (player == null) return Mathf.Infinity;
        return Vector3.Distance(transform.position, player.position);
    }

    public bool CanMeleeAttack()
    {
        return Time.time - lastMeleeAttackTime >= bossData.meleeAttackCooldown;
    }

    public bool CanSpikeAttack()
    {
        return Time.time - lastSpikeAttackTime >= bossData.spikeAttackCooldown;
    }

    void OnDrawGizmosSelected()
    {
        if (bossData == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, bossData.meleeAttackRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, bossData.spikeRadius);

        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, bossData.detectionRange);
    }
}