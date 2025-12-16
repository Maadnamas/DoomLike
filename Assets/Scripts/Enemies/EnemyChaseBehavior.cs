using UnityEngine;

public class EnemyChaseBehavior : IEnemyBehavior
{
    private bool isSearching = false;
    private bool isStuckWatching = false;
    private bool isWaitingToPatrol = false;
    private float waitTimer = 0f;

    public void OnEnter(EnemyAI enemy)
    {
        enemy.GetAnimator().SetRunning(true);
        enemy.GetAnimator().SetWalking(false);
        enemy.GetAnimator().SetIdle(false);

        isSearching = false;
        isStuckWatching = false;
        isWaitingToPatrol = false;
        waitTimer = 0f;
    }

    public void Execute(EnemyAI enemy)
    {
        if (enemy.player == null)
        {
            enemy.SetBehavior(new EnemyPatrolBehavior());
            return;
        }

        Vector3 targetPos = enemy.player.position;
        bool canSee = enemy.CanSeePlayer();

        if (isWaitingToPatrol)
        {
            enemy.StopMovement();
            enemy.GetAnimator().SetRunning(false);
            enemy.GetAnimator().SetWalking(false);
            enemy.GetAnimator().SetIdle(true);

            if (canSee)
            {
                isWaitingToPatrol = false;
                enemy.GetAnimator().SetIdle(false);
                enemy.GetAnimator().SetRunning(true);
                return;
            }

            waitTimer += Time.deltaTime;
            if (waitTimer >= enemy.loseInterestWaitTime)
            {
                if (enemy.patrolPoints != null && enemy.patrolPoints.Length > 0)
                    enemy.SetBehavior(new EnemyPatrolBehavior());
                else
                    enemy.SetBehavior(new EnemyIdleBehavior());
            }
            return;
        }

        if (canSee)
        {
            isSearching = false;
            isStuckWatching = false;
            targetPos = enemy.player.position;

            Vector3 dirToPlayer = (targetPos - enemy.transform.position).normalized;
            bool terrainSafeDesired = enemy.IsTerrainSafe(dirToPlayer, enemy.RunSpeed);

            if (!terrainSafeDesired)
            {
                enemy.StopMovement();
                enemy.RotateTowards(targetPos, true, 5f);
                enemy.GetAnimator().SetRunning(false);
                enemy.GetAnimator().SetWalking(false);
                enemy.GetAnimator().SetIdle(true);
                isStuckWatching = true;
                return;
            }

            enemy.GetAnimator().SetRunning(true);
            enemy.GetAnimator().SetIdle(false);
            isStuckWatching = false;
        }

        else
        {
            if (isStuckWatching)
            {
                isWaitingToPatrol = true;
                isStuckWatching = false;
                waitTimer = 0f;
                return;
            }

            if (enemy.LastKnownPlayerPos.HasValue)
            {
                if (!enemy.IsPositionReachable(enemy.LastKnownPlayerPos.Value))
                {
                    enemy.LastKnownPlayerPos = null;
                    isWaitingToPatrol = true;
                    return;
                }

                isSearching = true;
                targetPos = enemy.LastKnownPlayerPos.Value;

                enemy.GetAnimator().SetRunning(false);
                enemy.GetAnimator().SetWalking(true);

            }
            else
            {
                isWaitingToPatrol = true;
                return;
            }
        }

        float distance = Vector3.Distance(enemy.transform.position, targetPos);

        if (isSearching)
        {
            if (distance <= enemy.StopDistance)
            {
                enemy.LastKnownPlayerPos = null;
                isWaitingToPatrol = true;
                return;
            }
        }

        if (!isSearching && distance <= enemy.StopDistance)
        {
            enemy.SetBehavior(new EnemyAttackBehavior());
            return;
        }

        float rotationSpeed = canSee ? 15f : 10f;
        float speed = canSee ? enemy.RunSpeed : enemy.MoveSpeed;

        if (!isStuckWatching)
        {
            if (canSee)
                enemy.RotateTowards(enemy.player.position, true, rotationSpeed);
            else
                enemy.RotateTowards(targetPos, false, rotationSpeed);

            enemy.MoveTo(targetPos, speed);
        }
    }

    public void OnExit(EnemyAI enemy)
    {
        enemy.GetAnimator().SetRunning(false);
        enemy.GetAnimator().SetIdle(false);
        enemy.GetAnimator().SetWalking(false);
        if (enemy.headTransform != null)
            enemy.headTransform.localRotation = Quaternion.identity;
    }

    // MÉTODO AGREGADO/CORREGIDO PARA ACCESO EXTERNO
    public bool IsStuckWatching() => isStuckWatching;
}