using UnityEngine;
using System;

public class BossHealth : MonoBehaviour, IDamageable
{
    private float currentHealth;
    private float maxHealth;
    public bool IsDead { get; private set; }

    [Header("Efectos")]
    [SerializeField] private ParticleSystem hitVFXPrefab;

    [Header("Ragdoll")]
    [SerializeField] private bool useRagdoll = true;
    [SerializeField] private float ragdollDestroyDelay = 5f;

    private BossAnimator bossAnimator;
    private CharacterController characterController;
    private Animator animator;
    private Rigidbody[] ragdollRigidbodies;
    private Collider[] ragdollColliders;

    // Evento para actualizar la barra de vida
    public static event Action<float, float> OnBossHealthChanged;
    public static event Action OnBossDied;

    private void Awake()
    {
        bossAnimator = GetComponent<BossAnimator>();
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }

        // Inicializar ragdoll
        SetupRagdoll();
    }

    private void SetupRagdoll()
    {
        // Obtener todos los Rigidbodies y Colliders del ragdoll
        ragdollRigidbodies = GetComponentsInChildren<Rigidbody>();
        ragdollColliders = GetComponentsInChildren<Collider>();

        // Desactivar ragdoll al inicio
        SetRagdollActive(false);
    }

    private void SetRagdollActive(bool active)
    {
        // Activar/desactivar todos los Rigidbodies
        foreach (Rigidbody rb in ragdollRigidbodies)
        {
            rb.isKinematic = !active;
            rb.useGravity = active;
        }

        // Activar/desactivar todos los Colliders del ragdoll
        // (excepto el collider principal del boss si existe)
        foreach (Collider col in ragdollColliders)
        {
            // No desactivar el collider principal del CharacterController
            if (col != characterController)
            {
                col.enabled = active;
            }
        }

        // Desactivar animator cuando el ragdoll está activo
        if (animator != null)
        {
            animator.enabled = !active;
        }
    }

    public void Initialize(float maxHealthValue)
    {
        maxHealth = maxHealthValue;
        currentHealth = maxHealth;
        IsDead = false;

        // Notificar la salud inicial
        OnBossHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public bool TakeDamage(float amount, Vector3 hitPoint, Vector3 hitNormal)
    {
        if (IsDead) return false;

        currentHealth -= amount;
        currentHealth = Mathf.Max(currentHealth, 0);
        EventManager.TriggerEvent(GameEvents.ENEMY_DAMAGED, new EnemyDamagedEventData
        {
            damageTaken = amount,
            hitPoint = hitPoint
        });

        // Notificar cambio de salud
        OnBossHealthChanged?.Invoke(currentHealth, maxHealth);

        if (hitVFXPrefab != null)
        {
            ParticleSystem effect = Instantiate(hitVFXPrefab, hitPoint, Quaternion.LookRotation(hitNormal));
            Destroy(effect.gameObject, effect.main.duration);
        }

        if (currentHealth <= 0f)
        {
            Die();
            return true;
        }

        bossAnimator?.PlayHit();
        return false;
    }

    public void Heal(float amount)
    {
        if (IsDead) return;

        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);

        OnBossHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void Die()
    {
        if (IsDead) return;

        IsDead = true;
        OnBossDied?.Invoke();

        Debug.Log("¡Boss derrotado!");
        EventManager.TriggerEvent(GameEvents.BOSS_DIED, new BossDeathEventData
        {
            bossName = gameObject.name,
            position = transform.position,
        });

        if (useRagdoll)
        {
            // Activar ragdoll
            ActivateRagdoll();
        }
        else
        {
            // Reproducir animación de muerte tradicional
            bossAnimator?.PlayDeath();
        }

        // Desactivar Character Controller
        if (characterController != null)
            characterController.enabled = false;

        // Destruir el boss después de un tiempo
        Destroy(gameObject, ragdollDestroyDelay);
    }

    private void ActivateRagdoll()
    {
        Debug.Log("Activando ragdoll del boss");

        // Activar el ragdoll
        SetRagdollActive(true);

        // Opcional: Aplicar una fuerza al ragdoll para que caiga de forma más dramática
        // Puedes descomentar esto si quieres que el boss salga volando un poco
        /*
        Rigidbody mainRb = GetComponent<Rigidbody>();
        if (mainRb == null && ragdollRigidbodies.Length > 0)
        {
            // Aplicar fuerza al torso o al primer rigidbody
            ragdollRigidbodies[0].AddForce(Vector3.up * 300f + transform.forward * 200f);
        }
        */
    }

    public float GetCurrentHealth() => currentHealth;
    public float GetMaxHealth() => maxHealth;
}