using UnityEngine;

public class EnemyIdleBehavior : IEnemyBehavior
{
    public void OnEnter(EnemyAI enemy)
    {
        enemy.GetAnimator().SetIdle(true);
        enemy.GetAnimator().SetWalking(false);
    }

    public void Execute(EnemyAI enemy)
    {
        if (enemy.player == null) return;
        float distance = Vector3.Distance(enemy.transform.position, enemy.player.position);
        if (distance <= enemy.DetectionRange)
            enemy.SetBehavior(new EnemyChaseBehavior());
    }

    public void OnExit(EnemyAI enemy)
    {
        enemy.GetAnimator().SetIdle(false);
    }
}