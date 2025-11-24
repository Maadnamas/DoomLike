public interface IEnemyBehavior
{
    void OnEnter(EnemyAI enemy);
    void Execute(EnemyAI enemy);
    void OnExit(EnemyAI enemy);
}
