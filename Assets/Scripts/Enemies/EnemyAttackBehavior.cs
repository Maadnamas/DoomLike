using UnityEngine;

public class EnemyAttackBehavior : IEnemyBehavior
{
    private float lastAttackTime;
    private IDamageable playerDamageable;
    private bool isAttacking = false;
    private float attackDelay = 0.8f; // Ajusta esto al momento del impacto de tu animación
    private float attackTimer = 0f;

    public void OnEnter(EnemyAI enemy)
    {
        enemy.GetAnimator().SetRunning(false);
        enemy.GetAnimator().SetWalking(false);
        enemy.GetAnimator().SetIdle(true);

        enemy.StopMovement();

        lastAttackTime = Time.time - enemy.enemyData.attackCooldown;

        if (enemy.player != null)
        {
            playerDamageable = enemy.player.GetComponent<IDamageable>();
        }

        StartAttack(enemy);
    }

    public void Execute(EnemyAI enemy)
    {
        if (enemy.player == null) return;

        // Si estamos atacando, bloqueamos todo movimiento y rotación
        if (isAttacking)
        {
            // FUERZA EL FRENO EN CADA FRAME
            enemy.StopMovement();

            attackTimer += Time.deltaTime;
            if (attackTimer >= attackDelay)
            {
                ApplyDamage(enemy);
                isAttacking = false;
                attackTimer = 0f;
                lastAttackTime = Time.time;
            }
            return;
        }

        // Lógica normal de persecución si el jugador se aleja mucho entre ataques
        float distance = Vector3.Distance(enemy.transform.position, enemy.player.position);

        if (distance > enemy.enemyData.attackRange)
        {
            enemy.SetBehavior(new EnemyChaseBehavior());
            return;
        }

        // Rotar hacia el jugador mientras espera el cooldown (pero no moverse)
        enemy.RotateTowards(enemy.player.position, true, 5f);

        // Revisar cooldown
        if (Time.time - lastAttackTime >= enemy.enemyData.attackCooldown)
        {
            StartAttack(enemy);
        }
    }

    public void OnExit(EnemyAI enemy)
    {
        enemy.GetAnimator().SetIdle(false);
        isAttacking = false;
        attackTimer = 0f;
    }

    private void StartAttack(EnemyAI enemy)
    {
        isAttacking = true;
        attackTimer = 0f;
        enemy.StopMovement(); // Freno inicial
        enemy.GetAnimator().PlayAttack();
    }

    private void ApplyDamage(EnemyAI enemy)
    {
        // Verificamos distancia nuevamente para no golpear si el jugador se alejó
        if (enemy.player != null && Vector3.Distance(enemy.transform.position, enemy.player.position) <= enemy.enemyData.attackRange + 0.5f)
        {
            if (playerDamageable != null)
            {
                Vector3 hitPoint = enemy.player.position;
                Vector3 hitNormal = (enemy.player.position - enemy.transform.position).normalized;
                playerDamageable.TakeDamage(enemy.enemyData.damage, hitPoint, hitNormal);
            }
            Debug.Log(enemy.name + " golpeó al jugador.");
        }
    }
}