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
    /// Recibe da�o desde un raycast u otra fuente.
    /// </summary>
    /// <param name="amount">Cantidad de da�o recibido.</param>
    /// <param name="hitPoint">Punto donde impact� el raycast.</param>
    /// <param name="hitNormal">Direcci�n normal del impacto.</param>
    /// <returns>True si el enemigo muri�.</returns>
    public bool TakeDamage(float amount, Vector3 hitPoint, Vector3 hitNormal)
    {
        if (IsDead) return false;

        currentHealth -= amount;
        Debug.Log($"{gameObject.name} recibi� {amount} de da�o. Vida restante: {currentHealth}");

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

        //  Ac� podr�as agregar efectos visuales o de sonido:
        // Instantiate(deathEffectPrefab, hitPoint, Quaternion.LookRotation(hitNormal));

        Destroy(gameObject);
    }
}
