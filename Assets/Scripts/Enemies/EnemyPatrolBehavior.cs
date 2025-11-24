using UnityEngine;

public class EnemyPatrolBehavior : IEnemyBehavior
{
    private int currentPoint = -1;
    private bool isWaiting = false;
    private float waitTimer = 0f;

    public void OnEnter(EnemyAI enemy)
    {
        enemy.GetAnimator().SetWalking(true);
        enemy.GetAnimator().SetIdle(false);
        if (currentPoint < 0)
            currentPoint = GetRandomPatrolPoint(enemy.patrolPoints.Length);
    }

    public void Execute(EnemyAI enemy)
    {
        if (enemy.patrolPoints == null || enemy.patrolPoints.Length == 0)
            return;

        if (isWaiting)
        {
            waitTimer += Time.deltaTime;
            if (waitTimer >= enemy.idleWaitTime)
            {
                isWaiting = false;
                waitTimer = 0f;
                currentPoint = GetRandomPatrolPoint(enemy.patrolPoints.Length);
                enemy.GetAnimator().SetWalking(true);
                enemy.GetAnimator().SetIdle(false);
            }
            else
            {
                if (enemy.player != null && Vector3.Distance(enemy.transform.position, enemy.player.position) <= enemy.DetectionRange)
                    enemy.SetBehavior(new EnemyChaseBehavior());
                return;
            }
        }

        if (currentPoint < 0 || currentPoint >= enemy.patrolPoints.Length)
            currentPoint = GetRandomPatrolPoint(enemy.patrolPoints.Length);

        Transform target = enemy.patrolPoints[currentPoint];
        Vector3 dir = (target.position - enemy.transform.position).normalized;
        enemy.transform.position += dir * enemy.MoveSpeed * Time.deltaTime;
        enemy.transform.rotation = Quaternion.LookRotation(dir);

        float distance = Vector3.Distance(enemy.transform.position, target.position);

        if (distance < enemy.patrolPointTolerance)
        {
            isWaiting = true;
            waitTimer = 0f;
            enemy.GetAnimator().SetWalking(false);
            enemy.GetAnimator().SetIdle(true);
        }

        if (enemy.player != null && Vector3.Distance(enemy.transform.position, enemy.player.position) <= enemy.DetectionRange)
            enemy.SetBehavior(new EnemyChaseBehavior());
    }

    public void OnExit(EnemyAI enemy)
    {
        enemy.GetAnimator().SetWalking(false);
        enemy.GetAnimator().SetIdle(false);
    }

    private int GetRandomPatrolPoint(int length)
    {
        if (length <= 1) return 0;

        int newPoint;
        do
        {
            newPoint = Random.Range(0, length);
        } while (newPoint == currentPoint && length > 1);

        return newPoint;
    }
}