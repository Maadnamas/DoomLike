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

        // Decidir qué ataque usar
        bool shouldUseSpikeAttack = ShouldUseSpikeAttack(boss, distance);
        bool shouldUseMeleeAttack = distance <= boss.bossData.meleeAttackRange && boss.CanMeleeAttack();

        // Prioridad: Ataque de pinchos si está disponible y las condiciones son correctas
        if (shouldUseSpikeAttack)
        {
            boss.SetBehavior(new BossSpikeAttackBehavior());
            return;
        }

        // Si está cerca, ataque melee
        if (shouldUseMeleeAttack)
        {
            boss.SetBehavior(new BossMeleeAttackBehavior());
            return;
        }

        // Perseguir al jugador
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

        // Usar ataque de pinchos si:
        // 1. El jugador está lejos (fuera de rango melee pero dentro del radio de pinchos)
        // 2. O ha pasado suficiente tiempo desde el último ataque de pinchos
        bool playerInSpikeRange = distance >= boss.bossData.spikeAttackMinDistance &&
                                  distance <= boss.bossData.spikeRadius + 2f;

        bool longTimeSinceLastSpike = Time.time - boss.lastSpikeAttackTime >= boss.bossData.spikeAttackCooldown * 1.5f;

        return playerInSpikeRange || longTimeSinceLastSpike;
    }
}