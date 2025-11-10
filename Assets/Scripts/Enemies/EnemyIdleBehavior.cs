using UnityEngine;

public class EnemyIdleBehavior : IEnemyBehavior
{
    public void Execute(EnemyAI enemy)
    {
        if (enemy.player == null) return;
        float distance = Vector3.Distance(enemy.transform.position, enemy.player.position);
        if (distance <= enemy.DetectionRange)
            enemy.SetBehavior(new EnemyChaseBehavior());
    }
}
