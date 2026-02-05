using UnityEngine;

[RequireComponent(typeof(EnemyAI))]
public class EnemyBehaviorStarter : MonoBehaviour
{
    void Start()
    {
        // Al iniciar, le inyectamos la estrategia de Disparo
        GetComponent<EnemyAI>().SetBehavior(new EnemyShooterBehavior());
    }
}