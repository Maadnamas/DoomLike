public interface IEnemyAnimator
{
    void SetWalking(bool walking);
    void SetIdle(bool idle);
    void SetRunning(bool running);
    void PlayAttack();
    void PlayHit();
    void PlayDeath();
}