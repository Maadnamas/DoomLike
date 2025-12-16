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
        if (enemy.CanSeePlayer())
        {
            enemy.SetBehavior(new EnemyChaseBehavior());
            return;
        }

        if (enemy.patrolPoints == null || enemy.patrolPoints.Length == 0) return;

        if (isWaiting)
        {
            enemy.RotateTowards(enemy.transform.position + enemy.transform.forward, false, 5f);

            waitTimer += Time.deltaTime;
            if (waitTimer >= enemy.idleWaitTime)
            {
                isWaiting = false;
                waitTimer = 0f;
                currentPoint = GetRandomPatrolPoint(enemy.patrolPoints.Length);
                enemy.GetAnimator().SetWalking(true);
                enemy.GetAnimator().SetIdle(false);
            }
            return;
        }

        Transform target = enemy.patrolPoints[currentPoint];

        Vector3 enemyPosHorizontal = new Vector3(enemy.transform.position.x, 0, enemy.transform.position.z);
        Vector3 targetPosHorizontal = new Vector3(target.position.x, 0, target.position.z);
        float distance = Vector3.Distance(enemyPosHorizontal, targetPosHorizontal);

        if (distance < enemy.patrolPointTolerance)
        {
            isWaiting = true;
            waitTimer = 0f;
            enemy.GetAnimator().SetWalking(false);
            enemy.GetAnimator().SetIdle(true);
            return;
        }

        enemy.MoveTo(target.position, enemy.MoveSpeed);
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
        do { newPoint = Random.Range(0, length); } while (newPoint == currentPoint);
        return newPoint;
    }

    public bool IsWaiting() => isWaiting;
}