using UnityEngine;

public class EnemyBehaviorStarter : MonoBehaviour
{
    // 1. Definimos los tipos de enemigos disponibles
    public enum EnemyType
    {
        Shooter,    // El de antes (Seraphim)
        Kamikaze,   // El nuevo (Suicida)
        Patrol      // (Opcional por si quieres uno que solo patrulle)
    }

    [Header("Configuración")]
    // 2. Esta variable aparecerá como lista desplegable en el Inspector
    public EnemyType enemyType = EnemyType.Shooter;

    void Start()
    {
        var ai = GetComponent<EnemyAI>();
        if (ai == null) return;

        // 3. Decidimos qué cerebro ponerle según lo que elegiste en el Inspector
        switch (enemyType)
        {
            case EnemyType.Shooter:
                ai.SetBehavior(new EnemyShooterBehavior());
                break;

            case EnemyType.Kamikaze:
                ai.SetBehavior(new EnemyKamikazeBehavior());
                break;

            case EnemyType.Patrol:
                // Si tienes un comportamiento solo de patrulla, úsalo aquí
                ai.SetBehavior(new EnemyPatrolBehavior());
                break;
        }
    }
}