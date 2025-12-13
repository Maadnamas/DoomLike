public interface IBossBehavior
{
    void OnEnter(BossAI boss);
    void Execute(BossAI boss);
    void OnExit(BossAI boss);
}