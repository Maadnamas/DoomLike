using UnityEngine;

public class EnemyShooterBehavior : IEnemyBehavior
{
    private float lastShootTime;

    public void OnEnter(EnemyAI enemy)
    {
        // Configuración inicial: Quieto
        enemy.GetAnimator().SetRunning(false);
        enemy.GetAnimator().SetWalking(false);
        enemy.GetAnimator().SetIdle(true);

        enemy.StopMovement(); // Usamos tu método existente

        // Permitir disparar casi inmediatamente al ver al jugador
        lastShootTime = Time.time - (enemy.enemyData.attackCooldown * 0.5f);
    }

    public void Execute(EnemyAI enemy)
    {
        if (enemy.player == null) return;

        float distance = Vector3.Distance(enemy.transform.position, enemy.player.position);

        // 1. SIEMPRE MIRAR AL JUGADOR
        enemy.RotateTowards(enemy.player.position, true, 10f);

        // 2. COMPROBACIÓN DE RANGO
        // Si el jugador está muy lejos, no hace nada
        if (distance > enemy.enemyData.shootRange)
        {
            return;
        }

        // 3. DISPARO
        if (Time.time - lastShootTime >= enemy.enemyData.attackCooldown)
        {
            // Raycast para asegurar que te ve a los ojos (aprox 1.5m de altura)
            if (HasLineOfSight(enemy))
            {
                Shoot(enemy);
                lastShootTime = Time.time;
            }
        }
    }

    public void OnExit(EnemyAI enemy)
    {
        // Limpieza si hiciera falta
    }

    private void Shoot(EnemyAI enemy)
    {
        enemy.GetAnimator().PlayAttack(); // Trigger "Attack"

        if (enemy.enemyData.projectilePrefab != null)
        {
            // Punto de salida: Un poco arriba (1.5m) y adelante del centro
            Vector3 spawnPos = enemy.transform.position + Vector3.up * 1.5f + enemy.transform.forward * 0.8f;

            // Instanciar bala
            GameObject bullet = Object.Instantiate(enemy.enemyData.projectilePrefab, spawnPos, enemy.transform.rotation);

            // Configurar bala
            EnemyProjectile projScript = bullet.GetComponent<EnemyProjectile>();
            if (projScript != null)
            {
                projScript.Initialize(enemy.enemyData.projectileSpeed, enemy.enemyData.damage);
            }
        }
    }

    private bool HasLineOfSight(EnemyAI enemy)
    {
        RaycastHit hit;
        Vector3 direction = (enemy.player.position - enemy.transform.position).normalized;
        // Raycast desde altura de ojos (1.5m)
        if (Physics.Raycast(enemy.transform.position + Vector3.up * 1.5f, direction, out hit, enemy.enemyData.shootRange))
        {
            if (hit.transform.CompareTag("Player")) return true;
        }
        return false;
    }
}