using UnityEngine;

public class BossMeleeAttackBehavior : IBossBehavior
{
    private bool isAttacking = false;
    private float attackDelay = 0.4f;
    private float attackTimer = 0f;
    private IDamageable playerDamageable;

    public void OnEnter(BossAI boss)
    {
        boss.GetAnimator().SetWalking(false);
        boss.GetAnimator().SetIdle(true);
        boss.StopMovement();

        if (boss.player != null)
        {
            playerDamageable = boss.player.GetComponent<IDamageable>();
        }

        StartAttack(boss);
    }

    public void Execute(BossAI boss)
    {
        if (boss.player == null) return;

        // Keep stationary while attacking
        if (isAttacking)
        {
            boss.StopMovement();
            boss.RotateTowards(boss.player.position, 10f);

            attackTimer += Time.deltaTime;
            if (attackTimer >= attackDelay)
            {
                ApplyDamage(boss);
                isAttacking = false;
                attackTimer = 0f;
                boss.lastMeleeAttackTime = Time.time;
            }
            return;
        }

        // Return to chasing
        boss.SetBehavior(new BossChaseBehavior());
    }

    public void OnExit(BossAI boss)
    {
        boss.GetAnimator().SetIdle(false);
        isAttacking = false;
        attackTimer = 0f;
    }

    private void StartAttack(BossAI boss)
    {
        isAttacking = true;
        attackTimer = 0f;
        boss.StopMovement();
        boss.GetAnimator().PlayAttack();
        Debug.Log("Boss: Melee attack initiated");
    }

    private void ApplyDamage(BossAI boss)
    {
        if (boss.player != null &&
            Vector3.Distance(boss.transform.position, boss.player.position) <= boss.bossData.meleeAttackRange + 0.5f)
        {
            if (playerDamageable != null)
            {
                Vector3 hitPoint = boss.player.position;
                Vector3 hitNormal = (boss.player.position - boss.transform.position).normalized;
                playerDamageable.TakeDamage(boss.bossData.damage, hitPoint, hitNormal);
                Debug.Log("Boss hit the player with melee attack");
            }
        }
    }
}