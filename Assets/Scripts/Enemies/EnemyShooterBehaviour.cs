using UnityEngine;

public class EnemyShooterBehavior : IEnemyBehavior
{
    private float lastShootTime;

    public void OnEnter(EnemyAI enemy)
    {
        Debug.Log($"[SHOOTER] Starting Shooter behavior on {enemy.name}");

        var anim = enemy.GetAnimator();
        if (anim != null)
        {
            anim.SetRunning(false);
            anim.SetWalking(false);
            anim.SetIdle(true);
        }

        enemy.StopMovement();
        lastShootTime = Time.time - (enemy.enemyData.attackCooldown * 0.5f);
    }

    public void Execute(EnemyAI enemy)
    {
        if (enemy.player == null)
        {
            Debug.LogWarning("[SHOOTER] Player not found (variable is null)");
            return;
        }

        float distance = Vector3.Distance(enemy.transform.position, enemy.player.position);

        enemy.RotateTowards(enemy.player.position, true, 20f);

        if (distance > enemy.enemyData.shootRange)
        {
            return;
        }

        if (Time.time - lastShootTime >= enemy.enemyData.attackCooldown)
        {
            Debug.Log("[SHOOTER] Cooldown ready. Attempting to aim...");

            if (HasLineOfSight(enemy))
            {
                Debug.Log("[SHOOTER] Line of sight confirmed! FIRING.");
                Shoot(enemy);
                lastShootTime = Time.time;
            }
            else
            {
                Debug.Log("[SHOOTER] No line of sight (Blocked).");
            }
        }
    }

    public void OnExit(EnemyAI enemy)
    {
        Debug.Log($"[SHOOTER] Exiting Shooter state");
    }

    private void Shoot(EnemyAI enemy)
    {
        enemy.GetAnimator().PlayAttack();

        if (enemy.enemyData.projectilePrefab != null)
        {
            Vector3 spawnOrigin = enemy.transform.position + Vector3.up * 1.5f + enemy.transform.forward * 0.8f;
            Vector3 targetPosition = enemy.player.position + Vector3.up * 0.0f;
            Vector3 shootDirection = (targetPosition - spawnOrigin).normalized;

            Quaternion bulletRotation = Quaternion.LookRotation(shootDirection);

            GameObject bullet = Object.Instantiate(enemy.enemyData.projectilePrefab, spawnOrigin, bulletRotation);

            Debug.DrawLine(spawnOrigin, targetPosition, Color.magenta, 2f);

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

        Vector3 origin = enemy.transform.position + Vector3.up * 1.2f;
        Vector3 targetPos = enemy.player.position + Vector3.up * 1.0f;

        Vector3 direction = (targetPos - origin).normalized;
        float range = enemy.enemyData.shootRange;

        Debug.DrawLine(origin, targetPos, Color.yellow);

        if (Physics.SphereCast(origin, 0.5f, direction, out hit, range, ~0, QueryTriggerInteraction.Ignore))
        {
            if (hit.transform.CompareTag("Player"))
            {
                Debug.DrawLine(origin, hit.point, Color.green);
                return true;
            }
            else
            {
                Debug.DrawLine(origin, hit.point, Color.red);
                Debug.Log($"[SHOOTER] Blocked by: {hit.transform.name}");
                return false;
            }
        }

        Debug.Log("[SHOOTER] SphereCast hit nothing (Player too far or no Collider?)");
        return false;
    }
}