using UnityEngine;

public class EnemyPatrolBehavior : IEnemyBehavior
{
    private int currentPoint = 0;

    public void Execute(EnemyAI enemy)
    {
        if (enemy.patrolPoints == null || enemy.patrolPoints.Length == 0)
            return;

        Transform target = enemy.patrolPoints[currentPoint];
        Vector3 dir = (target.position - enemy.transform.position).normalized;
        enemy.transform.position += dir * enemy.MoveSpeed * Time.deltaTime;
        enemy.transform.rotation = Quaternion.LookRotation(dir);

        float distance = Vector3.Distance(enemy.transform.position, target.position);
        if (distance < 0.5f)
            currentPoint = (currentPoint + 1) % enemy.patrolPoints.Length;

        if (enemy.player != null && Vector3.Distance(enemy.transform.position, enemy.player.position) <= enemy.DetectionRange)
            enemy.SetBehavior(new EnemyChaseBehavior());
    }
}
