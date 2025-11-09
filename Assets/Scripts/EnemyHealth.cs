using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    [Header("Vida")]
    public float maxHealth = 100f;
    private float currentHealth;

    public bool IsDead { get; private set; }

    void Start()
    {
        currentHealth = maxHealth;
        IsDead = false;
    }

    public bool TakeDamage(float amount, Vector3 hitPoint, Vector3 hitNormal)
    {
        if (IsDead) return false;

        currentHealth -= amount;
        Debug.Log($"{gameObject.name} recibió {amount} de daño. Vida restante: {currentHealth}");

        if (currentHealth <= 0f)
        {
            Die();
            return true;
        }

        return false;
    }

    public void Heal(float amount)
    {

    }

    public void Die()
    {
        IsDead = true;
        Debug.Log($"{gameObject.name} ha muerto.");

        // EVENTO AGREGADO
        EventManager.TriggerEvent(GameEvents.ENEMY_DIED, new EnemyDeathEventData
        {
            enemyName = gameObject.name,
            position = transform.position
        });

        Destroy(gameObject);
    }
}