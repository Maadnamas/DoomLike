using UnityEngine;

public class BossIdleBehavior : IBossBehavior
{
    public void OnEnter(BossAI boss)
    {
        boss.GetAnimator().SetIdle(true);
        boss.GetAnimator().SetWalking(false);
        boss.StopMovement();
    }

    public void Execute(BossAI boss)
    {
        if (boss.player == null) return;

        float distance = boss.GetDistanceToPlayer();

        // Si detecta al jugador, empezar a perseguirlo
        if (distance <= boss.DetectionRange)
        {
            boss.SetBehavior(new BossChaseBehavior());
        }
    }

    public void OnExit(BossAI boss)
    {
        boss.GetAnimator().SetIdle(false);
    }
}