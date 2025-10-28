using UnityEngine;

public class SimpleEnemy : MonoBehaviour, IDamageable
{
    public float maxHealth = 100f;
    float currentHealth;

    public GameObject deathPrefab; // opcional: efecto al morir

    void Awake()
    {
        currentHealth = maxHealth;
    }

    public bool TakeDamage(float amount, Vector3 hitPoint, Vector3 hitNormal)
    {
        currentHealth -= amount;
        // Aquí podés spawnear sangre, sonido, animación, etc.
        Debug.Log($"{gameObject.name} recibió {amount} de daño. Vida: {currentHealth}");

        if (currentHealth <= 0f)
        {
            Die();
            return true;
        }
        return false;
    }

    void Die()
    {
        Debug.Log($"{gameObject.name} murió.");
        if (deathPrefab != null)
            Instantiate(deathPrefab, transform.position, Quaternion.identity);
        // por defecto destruimos el gameObject, adaptalo a tu sistema (pooling, anim death, etc.)
        Destroy(gameObject);
    }
}
