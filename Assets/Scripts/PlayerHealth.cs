using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [Header("settings de la vida")]
    public int maxHealth = 100;
    private int currentHealth;

    [Header("efectofeles")]
    public AudioClip hurtSound;
    public AudioClip deathSound;
    public GameObject bloodEffect;
    public Image healthBar;
    public float flashDuration = 0.1f;
    public Color flashColor = new Color(1, 0, 0, 0.25f);

    private AudioSource audioSource;
    private ScreenFlash screenFlash;
    private bool isDead;

    void Start()
    {
        currentHealth = maxHealth;
        audioSource = GetComponent<AudioSource>();
        screenFlash = FindObjectOfType<ScreenFlash>();

        // Disparar evento inicial de salud
        EventManager.TriggerEvent(GameEvents.UI_UPDATE_HEALTH, new DamageEventData
        {
            currentHealth = currentHealth,
            maxHealth = maxHealth
        });
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
            TakeDamage(20,Vector3.zero, Vector3.zero);

        if (Input.GetKeyDown(KeyCode.H))
            Heal(15);
    }

    // Implementación de IDamageable
    public bool TakeDamage(float amount, Vector3 hitPoint, Vector3 hitNormal)
    {
        CalculateDamage(Mathf.RoundToInt(amount));
        return !isDead;
    }

    public void CalculateDamage(int amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (hurtSound) audioSource.PlayOneShot(hurtSound);
        if (bloodEffect) Instantiate(bloodEffect, transform.position, Quaternion.identity);
        if (screenFlash) screenFlash.Flash(flashColor, flashDuration);

        // EVENTO ACTUALIZADO - usar UI_UPDATE_HEALTH
        EventManager.TriggerEvent(GameEvents.UI_UPDATE_HEALTH, new DamageEventData
        {
            damageAmount = amount,
            currentHealth = currentHealth,
            maxHealth = maxHealth
        });

        // EVENTO DE DAÑO (para otros sistemas)
        EventManager.TriggerEvent(GameEvents.PLAYER_DAMAGED, new DamageEventData
        {
            damageAmount = amount,
            currentHealth = currentHealth,
            maxHealth = maxHealth
        });

        if (currentHealth <= 0)
            Die();
    }

    public void Die()
    {
        isDead = true;
        if (deathSound) audioSource.PlayOneShot(deathSound);

        EventManager.TriggerEvent(GameEvents.PLAYER_DIED, null);
        Debug.Log("te moriste wachin");
    }

    public void Heal(float amount)
    {
        if (isDead) return;
        currentHealth += (int)amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        // CORRECCIÓN: Usar DamageEventData sin healAmount o crear uno específico
        EventManager.TriggerEvent(GameEvents.UI_UPDATE_HEALTH, new DamageEventData
        {
            currentHealth = currentHealth,
            maxHealth = maxHealth
        });

        // EVENTO DE CURA (para otros sistemas)
        EventManager.TriggerEvent(GameEvents.PLAYER_HEALED, new HealEventData
        {
            healAmount = (int)amount,
            currentHealth = currentHealth
        });
    }
}