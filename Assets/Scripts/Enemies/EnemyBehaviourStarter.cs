using UnityEngine;

public class EnemyBehaviorStarter : MonoBehaviour
{
    public enum EnemyType
    {
        Shooter,
        Kamikaze,
        Patrol
    }

    [Header("Configuration")]
    public EnemyType enemyType = EnemyType.Shooter;

    void Start()
    {
        var ai = GetComponent<EnemyAI>();
        if (ai == null) return;

        switch (enemyType)
        {
            case EnemyType.Shooter:
                ai.SetBehavior(new EnemyShooterBehavior());
                break;

            case EnemyType.Kamikaze:
                ai.SetBehavior(new EnemyKamikazeBehavior());
                break;

            case EnemyType.Patrol:
                ai.SetBehavior(new EnemyPatrolBehavior());
                break;
        }
    }
}