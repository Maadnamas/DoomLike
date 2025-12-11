using UnityEngine;

[RequireComponent(typeof(EnemyHealth))]
[RequireComponent(typeof(EnemyAnimator))]
[RequireComponent(typeof(CharacterController))]
public class EnemyAI : MonoBehaviour
{
    [Header("Flyweight Data")]
    public EnemyData enemyData;

    [Header("Referencias")]
    public Transform headTransform;
    public Transform[] patrolPoints;

    [Header("Configuración de Capas (Scene Setup)")]
    public LayerMask visionMask;
    public LayerMask playerLayer;
    public LayerMask obstacleMask;

    [Header("Configuración de Obstáculos y Terreno")]
    public float obstacleDetectionRange = 2.0f;
    public float maxSlopeAngle = 45f;

    [Header("Configuración de Patrulla")]
    public float patrolPointTolerance = 1.0f;
    public float idleWaitTime = 2f;

    public Transform player { get; private set; }
    public Vector3? LastKnownPlayerPos { get; set; }
    public float MoveSpeed => enemyData.moveSpeed;
    public float RunSpeed => enemyData.runSpeed;
    public float StopDistance => enemyData.stopDistance;
    public float DetectionRange => enemyData.detectionRange;
    public float AttackRange => enemyData.attackRange;

    private EnemyHealth enemyHealth;
    private IEnemyAnimator enemyAnimator;
    private IEnemyBehavior currentBehavior;
    private CharacterController characterController;
    private Vector3 currentVelocity;

    void Start()
    {
        enemyHealth = GetComponent<EnemyHealth>();
        enemyAnimator = GetComponent<EnemyAnimator>();

        characterController = GetComponent<CharacterController>();
        if (characterController == null)
            characterController = gameObject.AddComponent<CharacterController>();

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

        CheckVision();
        currentBehavior?.Execute(this);

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

    private void CheckVision()
    {
        if (player == null) return;

        Vector3 dirToPlayer = (player.position - transform.position).normalized;
        float distToPlayer = Vector3.Distance(transform.position, player.position);

        if (distToPlayer <= enemyData.proximityDetectionRadius)
        {
            LastKnownPlayerPos = player.position;
            return;
        }

        if (distToPlayer > DetectionRange) return;

        if (Vector3.Angle(transform.forward, dirToPlayer) < enemyData.fieldOfView / 2f)
        {
            Vector3 origin = transform.position + Vector3.up * 1.5f;
            Vector3 targetPos = player.position + Vector3.up * 1.5f;

            if (!Physics.Linecast(origin, targetPos, visionMask))
            {
                LastKnownPlayerPos = player.position;
            }
        }
    }

    public bool CanSeePlayer()
    {
        if (LastKnownPlayerPos.HasValue && player != null)
        {
            return Vector3.Distance(LastKnownPlayerPos.Value, player.position) < 0.5f &&
                   Vector3.Distance(transform.position, player.position) <= DetectionRange;
        }
        return false;
    }

    public void RotateTowards(Vector3 targetPosition, bool rotateHead = true, float rotationSpeed = 15f)
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        Vector3 lookDir = new Vector3(direction.x, 0, direction.z);

        if (lookDir != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(lookDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }

        if (rotateHead && headTransform != null)
        {
            Vector3 dirToTarget = targetPosition - headTransform.position;
            if (dirToTarget != Vector3.zero)
            {
                Quaternion headTargetRot = Quaternion.LookRotation(dirToTarget);
                headTransform.rotation = Quaternion.Slerp(headTransform.rotation, headTargetRot, Time.deltaTime * 10f);
            }
        }
        else if (headTransform != null)
        {
            headTransform.localRotation = Quaternion.Slerp(headTransform.localRotation, Quaternion.identity, Time.deltaTime * 5f);
        }
    }

    public void MoveTo(Vector3 targetPosition, float speed)
    {
        Vector3 desiredDir = (targetPosition - transform.position).normalized;
        Vector3 finalDir = desiredDir;

        Vector3 origin = transform.position + Vector3.up * 0.5f;

        bool hitFront = Physics.Raycast(origin, transform.forward, obstacleDetectionRange, obstacleMask);
        bool terrainSafe = IsTerrainSafe(transform.forward, speed);

        if (hitFront || !terrainSafe)
        {
            Vector3 rightDir = Vector3.Cross(Vector3.up, transform.forward);
            finalDir = (transform.forward + rightDir * Time.deltaTime * 2f).normalized;
        }

        RotateTowards(transform.position + finalDir, false, 15f);

        Vector3 horizontalMovement = finalDir * speed * Time.deltaTime;
        horizontalMovement.y = 0;
        characterController.Move(horizontalMovement);
    }

    public void StopMovement()
    {
        characterController.Move(Vector3.zero);
    }

    private bool IsTerrainSafe(Vector3 direction, float speed)
    {
        Vector3 futurePos = transform.position + (direction * speed * Time.deltaTime * 20f);

        RaycastHit hit;
        if (Physics.Raycast(futurePos + Vector3.up, Vector3.down, out hit, 3f, obstacleMask))
        {
            float slopeAngle = Vector3.Angle(hit.normal, Vector3.up);
            if (slopeAngle > maxSlopeAngle) return false;
            return true;
        }
        return false;
    }

    public bool IsPositionReachable(Vector3 position)
    {
        Vector3 rayOrigin = position + Vector3.up * 5f;
        RaycastHit hit;

        if (Physics.Raycast(rayOrigin, Vector3.down, out hit, 5.5f, obstacleMask))
        {
            float slopeAngle = Vector3.Angle(hit.normal, Vector3.up);
            if (slopeAngle <= maxSlopeAngle)
            {
                return true;
            }
        }

        return false;
    }

    public void SetBehavior(IEnemyBehavior newBehavior)
    {
        currentBehavior?.OnExit(this);
        currentBehavior = newBehavior;
        currentBehavior?.OnEnter(this);
    }

    public IEnemyAnimator GetAnimator() => enemyAnimator;

    void OnDrawGizmosSelected()
    {
        if (enemyData == null) return;

        
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, enemyData.proximityDetectionRadius);

        
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, enemyData.detectionRange);

        
        Vector3 forward = transform.forward * enemyData.detectionRange;
        Vector3 leftRay = Quaternion.Euler(0, -enemyData.fieldOfView / 2, 0) * forward;
        Vector3 rightRay = Quaternion.Euler(0, enemyData.fieldOfView / 2, 0) * forward;

        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(transform.position, leftRay);
        Gizmos.DrawRay(transform.position, rightRay);
        Gizmos.DrawLine(transform.position + leftRay, transform.position + rightRay);
    }
}