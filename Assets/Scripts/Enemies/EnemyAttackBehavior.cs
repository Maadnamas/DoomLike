using UnityEngine;

public class EnemyAttackBehavior : IEnemyBehavior
{
    private float attackCooldown = 1.5f;
    private float lastAttackTime;

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
}
