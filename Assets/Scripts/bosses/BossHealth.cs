using UnityEngine;
using System;

public class BossHealth : MonoBehaviour, IDamageable
{
    private float currentHealth;
    private float maxHealth;
    public bool IsDead { get; private set; }

    [Header("Efectos")]
    [SerializeField] private ParticleSystem hitVFXPrefab;

    private IEnemyAnimator bossAnimator;
    private CharacterController characterController;

    // Evento para actualizar la barra de vida
    public static event Action<float, float> OnBossHealthChanged;
    public static event Action OnBossDied;

    private void Awake()
    {
        bossAnimator = GetComponent<IEnemyAnimator>();
        characterController = GetComponent<CharacterController>();
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

        bossAnimator?.PlayDeath();

        if (characterController != null)
            characterController.enabled = false;

        Debug.Log("¡Boss derrotado!");

        Destroy(gameObject, 5f);
    }

    public float GetCurrentHealth() => currentHealth;
    public float GetMaxHealth() => maxHealth;
}