using UnityEngine;
using System.Collections;

public class EnemyKamikazeBehavior : IEnemyBehavior
{
    private bool isExploding = false;

    private float separationRadius = 2.0f;
    private float separationForce = 3.0f;

    public void OnEnter(EnemyAI enemy)
    {
        enemy.GetAnimator().SetRunning(true);
        enemy.GetAnimator().SetWalking(false);
        enemy.GetAnimator().SetIdle(false);
        isExploding = false;
    }

    public void Execute(EnemyAI enemy)
    {
        if (isExploding || enemy.player == null) return;

        float distanceToPlayer = Vector3.Distance(enemy.transform.position, enemy.player.position);

        if (distanceToPlayer <= enemy.enemyData.attackRange)
        {
            enemy.StartCoroutine(ExplodeSequence(enemy));
            return;
        }

        Vector3 directionToPlayer = (enemy.player.position - enemy.transform.position).normalized;
        Vector3 separation = CalculateSeparation(enemy);
        Vector3 finalDirection = (directionToPlayer + separation).normalized;

        enemy.RotateTowards(enemy.player.position, true, 15f);
        enemy.MoveTo(enemy.transform.position + finalDirection, enemy.enemyData.runSpeed);
    }

    public void OnExit(EnemyAI enemy) { }

    private Vector3 CalculateSeparation(EnemyAI me)
    {
        Vector3 separationVector = Vector3.zero;
        Collider[] neighbors = Physics.OverlapSphere(me.transform.position, separationRadius, LayerMask.GetMask("Enemy"));

        foreach (var neighbor in neighbors)
        {
            if (neighbor.gameObject != me.gameObject)
            {
                Vector3 pushDir = me.transform.position - neighbor.transform.position;
                separationVector += pushDir.normalized / (pushDir.magnitude + 0.1f);
            }
        }
        return separationVector * separationForce;
    }

    private IEnumerator ExplodeSequence(EnemyAI enemy)
    {
        isExploding = true;
        enemy.StopMovement();
        enemy.GetAnimator().SetRunning(false);
        enemy.GetAnimator().PlayAttack();

        if (enemy.enemyData.fuseSound)
            AudioManager.PlaySFX3D(enemy.enemyData.fuseSound, enemy.transform.position);

        yield return new WaitForSeconds(enemy.enemyData.fuseTime);

        Explode(enemy);
    }

    private void Explode(EnemyAI enemy)
    {
        if (enemy.enemyData.explosionVFX)
        {
            GameObject vfx = Object.Instantiate(enemy.enemyData.explosionVFX, enemy.transform.position, Quaternion.identity);
            Object.Destroy(vfx, 3f);
        }

        if (enemy.enemyData.explosionSound)
            AudioManager.PlaySFX3D(enemy.enemyData.explosionSound, enemy.transform.position);

        Collider[] hits = Physics.OverlapSphere(enemy.transform.position, enemy.enemyData.explosionRadius);
        foreach (var hit in hits)
        {
            IDamageable damageable = hit.GetComponent<IDamageable>();
            if (damageable != null)
            {
                float dist = Vector3.Distance(enemy.transform.position, hit.transform.position);
                float damageFactor = 1f - Mathf.Clamp01(dist / enemy.enemyData.explosionRadius);
                damageable.TakeDamage(enemy.enemyData.explosionDamage * damageFactor, hit.transform.position, Vector3.up);
            }

            if (hit.attachedRigidbody != null)
            {
                hit.attachedRigidbody.AddExplosionForce(500f, enemy.transform.position, enemy.enemyData.explosionRadius);
            }

            if (hit.CompareTag("Player"))
            {
                PlayerMovement pm = hit.GetComponent<PlayerMovement>();
                if (pm != null)
                {
                    Vector3 dir = hit.transform.position - enemy.transform.position;
                    dir += Vector3.up * 0.5f;
                    pm.AddImpact(dir, enemy.enemyData.explosionForce);
                }
            }
        }

        var myHealth = enemy.GetComponent<IDamageable>();
        if (myHealth != null)
            myHealth.TakeDamage(9999, enemy.transform.position, Vector3.up);
        else
            Object.Destroy(enemy.gameObject);
    }
}