using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    private float currentHealth;
    public bool IsDead { get; private set; }
    private float maxHealth;

    public void Initialize(float maxHealthValue)
    {
        maxHealth = maxHealthValue;
        currentHealth = maxHealth;
        IsDead = false;
    }

    public bool TakeDamage(float amount, Vector3 hitPoint, Vector3 hitNormal)
    {
        if (IsDead) return false;
        currentHealth -= amount;
        if (currentHealth <= 0f)
        {
            Die();
            return true;
        }
        return false;
    }

    public void Heal(float amount) { }

    public void Die()
    {
        IsDead = true;
        Destroy(gameObject);
    }
}
