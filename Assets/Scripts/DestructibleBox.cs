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

        if (boxRenderer == null)
            boxRenderer = GetComponent<Renderer>();

        boxCollider = GetComponent<Collider>();
        rb = GetComponent<Rigidbody>();

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

        currentHealth -= amount;

        Debug.Log($"Box received {amount} damage. Remaining health: {currentHealth}/{maxHealth}");

        UpdateDamageVisual();

        CreateImpactEffect(hitPoint, hitNormal);

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
            float damageAmount = 1f - (currentHealth / maxHealth);
            damageAmount = Mathf.Clamp01(damageAmount);

            boxMaterial.SetFloat(shaderPropertyID, damageAmount);
            Debug.Log($"Shader _Amount updated to: {damageAmount}");
        }
        else if (boxMaterial != null)
        {
            Debug.LogWarning($"Material does not have property '{shaderProperty}'. Make sure the shader has this property.");
        }
    }

    private void CreateImpactEffect(Vector3 position, Vector3 normal)
    {

    }

    public void Heal(float amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        UpdateDamageVisual();
        Debug.Log($"Box healed: +{amount}. Current health: {currentHealth}");
    }

    public void Die()
    {
        Debug.Log("Box destroyed!");

        if (boxCollider != null)
            boxCollider.enabled = false;

        if (destructionEffect != null)
        {
            GameObject effect = Instantiate(destructionEffect, transform.position, Quaternion.identity);

            ParticleSystem ps = effect.GetComponent<ParticleSystem>();
            if (ps != null)
                Destroy(effect, ps.main.duration);
            else
                Destroy(effect, 3f);
        }

        if (destructionSound != null)
        {
            AudioSource.PlayClipAtPoint(destructionSound, transform.position);
        }

        if (enablePhysicsOnDeath)
        {
            if (rb == null)
                rb = gameObject.AddComponent<Rigidbody>();

            rb.isKinematic = false;
            rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);

            Destroy(gameObject, 5f);
        }
        else
        {
            if (boxRenderer != null)
                boxRenderer.enabled = false;

            Destroy(gameObject, 3f);
        }
    }

    void OnDestroy()
    {
        if (boxMaterial != null && Application.isPlaying)
        {
            Destroy(boxMaterial);
        }
    }
}