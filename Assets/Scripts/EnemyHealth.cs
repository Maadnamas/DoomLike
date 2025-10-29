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

    /// <summary>
    /// Recibe daño desde un raycast u otra fuente.
    /// </summary>
    /// <param name="amount">Cantidad de daño recibido.</param>
    /// <param name="hitPoint">Punto donde impactó el raycast.</param>
    /// <param name="hitNormal">Dirección normal del impacto.</param>
    /// <returns>True si el enemigo murió.</returns>
    public bool TakeDamage(float amount, Vector3 hitPoint, Vector3 hitNormal)
    {
        if (IsDead) return false;

        currentHealth -= amount;
        Debug.Log($"{gameObject.name} recibió {amount} de daño. Vida restante: {currentHealth}");

        if (currentHealth <= 0f)
        {
            Die(hitPoint, hitNormal);
            return true;
        }

        return false;
    }

    private void Die(Vector3 hitPoint, Vector3 hitNormal)
    {
        IsDead = true;
        Debug.Log($"{gameObject.name} ha muerto.");

        //  Acá podrías agregar efectos visuales o de sonido:
        // Instantiate(deathEffectPrefab, hitPoint, Quaternion.LookRotation(hitNormal));

        Destroy(gameObject);
    }
}
