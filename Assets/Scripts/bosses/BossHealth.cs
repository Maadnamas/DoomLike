using UnityEngine;
using System;

public class BossHealth : MonoBehaviour, IDamageable
{
    private float currentHealth;
    private float maxHealth;
    public bool IsDead { get; private set; }

    [Header("Effects")]
    [SerializeField] private ParticleSystem hitVFXPrefab;

    [Header("Ragdoll")]
    [SerializeField] private bool useRagdoll = true;
    [SerializeField] private float ragdollDestroyDelay = 5f;

    [Header("Door")]
    [SerializeField] private DoorOpener doorToOpen;

    private BossAnimator bossAnimator;
    private CharacterController characterController;
    private Animator animator;
    private Rigidbody[] ragdollRigidbodies;
    private Collider[] ragdollColliders;

    // Events to update health bar
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

        // Initialize ragdoll
        SetupRagdoll();
    }

    private void SetupRagdoll()
    {
        // Get all Rigidbodies and Colliders for the ragdoll
        ragdollRigidbodies = GetComponentsInChildren<Rigidbody>();
        ragdollColliders = GetComponentsInChildren<Collider>();

        // Disable ragdoll at start
        SetRagdollActive(false);
    }

    private void SetRagdollActive(bool active)
    {
        // Enable/disable all Rigidbodies
        foreach (Rigidbody rb in ragdollRigidbodies)
        {
            rb.isKinematic = !active;
            rb.useGravity = active;
        }

        // Enable/disable all ragdoll Colliders
        foreach (Collider col in ragdollColliders)
        {
            // Do not disable the main CharacterController collider
            if (col != characterController)
            {
                col.enabled = active;
            }
        }

        // Disable animator when ragdoll is active
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

        // Notify initial health
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

        // Notify health change
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

        Debug.Log("Boss defeated!");
        EventManager.TriggerEvent(GameEvents.BOSS_DIED, new BossDeathEventData
        {
            bossName = gameObject.name,
            position = transform.position,
        });

        // Open the door when boss dies
        if (doorToOpen != null)
        {
            doorToOpen.OpenDoor();
        }

        if (useRagdoll)
        {
            ActivateRagdoll();
        }
        else
        {
            // Play traditional death animation
            bossAnimator?.PlayDeath();
        }

        // Disable Character Controller
        if (characterController != null)
            characterController.enabled = false;

        // Destroy boss after delay
        Destroy(gameObject, ragdollDestroyDelay);
    }

    private void ActivateRagdoll()
    {
        Debug.Log("Activating boss ragdoll");

        // Enable ragdoll
        SetRagdollActive(true);
    }

    public float GetCurrentHealth() => currentHealth;
    public float GetMaxHealth() => maxHealth;
}