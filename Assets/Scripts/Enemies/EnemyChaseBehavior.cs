using UnityEngine;

public class EnemyChaseBehavior : IEnemyBehavior
{
    public void OnEnter(EnemyAI enemy)
    {
        enemy.GetAnimator().SetWalking(true);
        enemy.GetAnimator().SetIdle(false);
    }

    public void Execute(EnemyAI enemy)
    {
        if (enemy.player == null) return;

        float distance = Vector3.Distance(enemy.transform.position, enemy.player.position);

        if (distance > enemy.DetectionRange * 1.5f)
        {
            enemy.SetBehavior(new EnemyPatrolBehavior());
            return;
        }

        Vector3 dir = (enemy.player.position - enemy.transform.position).normalized;
        dir.y = 0f;
        enemy.transform.rotation = Quaternion.LookRotation(dir);

        if (distance > enemy.StopDistance)
            enemy.transform.position += dir * enemy.MoveSpeed * Time.deltaTime;
        else
            enemy.SetBehavior(new EnemyAttackBehavior());
    }

    public void OnExit(EnemyAI enemy)
    {
        enemy.GetAnimator().SetWalking(false);
    }
}