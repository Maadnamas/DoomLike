using UnityEngine;

public class DestructibleBox : MonoBehaviour, IDamageable
{
    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth;

    [Header("Visual Effects")]
    [SerializeField] private Renderer boxRenderer;
    [SerializeField] private string shaderProperty = "_Amount";
    [SerializeField] private GameObject destructionEffect;
    [SerializeField] private AudioClip destructionSound;

    [Header("Physics")]
    [SerializeField] private bool enablePhysicsOnDeath = true;
    [SerializeField] private float explosionForce = 500f;
    [SerializeField] private float explosionRadius = 3f;

    private Material boxMaterial;
    private Collider boxCollider;
    private Rigidbody rb;
    private int shaderPropertyID;

    void Start()
    {
        currentHealth = maxHealth;
        shaderPropertyID = Shader.PropertyToID(shaderProperty);

        // Obtener referencias
        if (boxRenderer == null)
            boxRenderer = GetComponent<Renderer>();

        boxCollider = GetComponent<Collider>();
        rb = GetComponent<Rigidbody>();

        // Crear una copia del material para no afectar a otros objetos
        if (boxRenderer != null)
        {
            boxMaterial = boxRenderer.material;
            if (boxMaterial.HasProperty(shaderPropertyID))
                boxMaterial.SetFloat(shaderPropertyID, 0f);
        }
    }

    public bool TakeDamage(float amount, Vector3 hitPoint, Vector3 hitNormal)
    {
        if (currentHealth <= 0) return false;

        // Reducir salud
        currentHealth -= amount;

        Debug.Log($"Caja recibió {amount} de daño. Salud restante: {currentHealth}/{maxHealth}");

        // Actualizar el shader (progresión del daño visual)
        UpdateDamageVisual();

        // Crear efecto de impacto
        CreateImpactEffect(hitPoint, hitNormal);

        // Verificar si la caja se destruye
        if (currentHealth <= 0)
        {
            Die();
            return true;
        }

        return false;
    }

    private void UpdateDamageVisual()
    {
        if (boxMaterial != null && boxMaterial.HasProperty(shaderPropertyID))
        {
            // Calcular valor del shader (0 = sin daño, 1 = destruido)
            float damageAmount = 1f - (currentHealth / maxHealth);

            // Asegurar que esté entre 0 y 1
            damageAmount = Mathf.Clamp01(damageAmount);

            boxMaterial.SetFloat(shaderPropertyID, damageAmount);
            Debug.Log($"Shader _Amount actualizado a: {damageAmount}");
        }
        else if (boxMaterial != null)
        {
            Debug.LogWarning($"El material no tiene la propiedad '{shaderProperty}'. Asegúrate de que el shader tenga esta propiedad.");
        }
    }

    private void CreateImpactEffect(Vector3 position, Vector3 normal)
    {

    }

    public void Heal(float amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        UpdateDamageVisual();
        Debug.Log($"Caja curada: +{amount}. Salud actual: {currentHealth}");
    }

    public void Die()
    {
        Debug.Log("Caja destruida!");

        // Desactivar colisión
        if (boxCollider != null)
            boxCollider.enabled = false;

        // Efecto de destrucción
        if (destructionEffect != null)
        {
            GameObject effect = Instantiate(destructionEffect, transform.position, Quaternion.identity);

            // Destruir después de que terminen las partículas
            ParticleSystem ps = effect.GetComponent<ParticleSystem>();
            if (ps != null)
                Destroy(effect, ps.main.duration);
            else
                Destroy(effect, 3f);
        }

        // Sonido de destrucción
        if (destructionSound != null)
        {
            AudioSource.PlayClipAtPoint(destructionSound, transform.position);
        }

        // Activar física si está desactivada
        if (enablePhysicsOnDeath)
        {
            if (rb == null)
                rb = gameObject.AddComponent<Rigidbody>();

            rb.isKinematic = false;
            rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);

            // Destruir después de un tiempo
            Destroy(gameObject, 5f);
        }
        else
        {
            // Solo desactivar visualmente
            if (boxRenderer != null)
                boxRenderer.enabled = false;

            // Destruir el objeto después de un tiempo
            Destroy(gameObject, 3f);
        }

        // Opcional: Notificar al sistema de spawners o eventos
        // EventManager.TriggerEvent("BOX_DESTROYED", new { boxPosition = transform.position });
    }

    void OnDestroy()
    {
        // Limpiar material instanciado
        if (boxMaterial != null && Application.isPlaying)
        {
            Destroy(boxMaterial);
        }
    }
}
