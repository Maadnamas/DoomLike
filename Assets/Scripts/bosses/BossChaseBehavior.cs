using UnityEngine;

public class BossChaseBehavior : IBossBehavior
{
    public void OnEnter(BossAI boss)
    {
        boss.GetAnimator().SetWalking(true);
        boss.GetAnimator().SetIdle(false);
    }

    public void Execute(BossAI boss)
    {
        if (boss.player == null)
        {
            boss.SetBehavior(new BossIdleBehavior());
            return;
        }

        float distance = boss.GetDistanceToPlayer();

        // Decide which attack to use
        bool shouldUseSpikeAttack = ShouldUseSpikeAttack(boss, distance);
        bool shouldUseMeleeAttack = distance <= boss.bossData.meleeAttackRange && boss.CanMeleeAttack();

        // Priority: Spike attack if available and conditions are right
        if (shouldUseSpikeAttack)
        {
            boss.SetBehavior(new BossSpikeAttackBehavior());
            return;
        }

        // If close, melee attack
        if (shouldUseMeleeAttack)
        {
            boss.SetBehavior(new BossMeleeAttackBehavior());
            return;
        }

        // Chase the player
        boss.RotateTowards(boss.player.position);
        boss.MoveTo(boss.player.position, boss.MoveSpeed);
    }

    public void OnExit(BossAI boss)
    {
        boss.GetAnimator().SetWalking(false);
    }

    private bool ShouldUseSpikeAttack(BossAI boss, float distance)
    {
        if (!boss.CanSpikeAttack()) return false;

        // Use spike attack if:
        // 1. The player is far (outside melee range but inside spike radius)
        // 2. Or enough time has passed since the last spike attack
        bool playerInSpikeRange = distance >= boss.bossData.spikeAttackMinDistance &&
                                  distance <= boss.bossData.spikeRadius + 2f;

        bool longTimeSinceLastSpike = Time.time - boss.lastSpikeAttackTime >= boss.bossData.spikeAttackCooldown * 1.5f;

        return playerInSpikeRange || longTimeSinceLastSpike;
    }
}