using UnityEngine;

public class EnemyChaseBehavior : IEnemyBehavior
{
    private bool isSearching = false;

    public void OnEnter(EnemyAI enemy)
    {
        enemy.GetAnimator().SetRunning(true);
        enemy.GetAnimator().SetWalking(false);
        enemy.GetAnimator().SetIdle(false);
        isSearching = false;
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

        if (canSee)
        {
            isSearching = false;
            targetPos = enemy.player.position;
        }
        else
        {
            if (enemy.LastKnownPlayerPos.HasValue)
            {
                if (!enemy.IsPositionReachable(enemy.LastKnownPlayerPos.Value))
                {
                    enemy.LastKnownPlayerPos = null;
                    enemy.SetBehavior(new EnemyPatrolBehavior());
                    return;
                }

                isSearching = true;
                targetPos = enemy.LastKnownPlayerPos.Value;
            }
            else
            {
                enemy.SetBehavior(new EnemyPatrolBehavior());
                return;
            }
        }

        float distance = Vector3.Distance(enemy.transform.position, targetPos);

        if (isSearching && distance <= enemy.StopDistance)
        {
            enemy.LastKnownPlayerPos = null;
            enemy.SetBehavior(new EnemyPatrolBehavior());
            return;
        }

        if (!isSearching && distance <= enemy.StopDistance)
        {
            enemy.SetBehavior(new EnemyAttackBehavior());
            return;
        }

        float rotationSpeed = canSee ? 15f : 10f;

        if (canSee)
            enemy.RotateTowards(enemy.player.position, true, rotationSpeed);
        else
            enemy.RotateTowards(targetPos, false, rotationSpeed);

        float speed = canSee ? enemy.RunSpeed : enemy.MoveSpeed;

        enemy.MoveTo(targetPos, speed);
    }

    public void OnExit(EnemyAI enemy)
    {
        enemy.GetAnimator().SetRunning(false);
        if (enemy.headTransform != null)
            enemy.headTransform.localRotation = Quaternion.identity;
    }
}