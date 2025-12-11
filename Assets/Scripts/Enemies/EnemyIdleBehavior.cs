using UnityEngine;

public class EnemyIdleBehavior : IEnemyBehavior
{
    public void OnEnter(EnemyAI enemy)
    {
        enemy.GetAnimator().SetIdle(true);
        enemy.GetAnimator().SetWalking(false);

        enemy.StopMovement();
    }

    public void Execute(EnemyAI enemy)
    {
        if (enemy.player == null) return;

        if (enemy.CanSeePlayer())
            enemy.SetBehavior(new EnemyChaseBehavior());
    }

    public void OnExit(EnemyAI enemy)
    {
        enemy.GetAnimator().SetIdle(false);
    }
}