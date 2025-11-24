using UnityEngine;

public class EnemyAttackBehavior : IEnemyBehavior
{
    private float attackCooldown = 1.5f;
    private float lastAttackTime;

    public void OnEnter(EnemyAI enemy)
    {
        enemy.GetAnimator().SetWalking(false);
        enemy.GetAnimator().SetIdle(true);
    }

    public void Execute(EnemyAI enemy)
    {
        if (enemy.player == null) return;

        float distance = Vector3.Distance(enemy.transform.position, enemy.player.position);
        if (distance > enemy.StopDistance + 1f)
        {
            enemy.SetBehavior(new EnemyChaseBehavior());
            return;
        }

        enemy.transform.LookAt(enemy.player);

        if (Time.time - lastAttackTime >= attackCooldown)
        {
            lastAttackTime = Time.time;
            Debug.Log(enemy.name + " atacó al jugador");
        }
    }

    public void OnExit(EnemyAI enemy)
    {
        enemy.GetAnimator().SetIdle(false);
    }
}