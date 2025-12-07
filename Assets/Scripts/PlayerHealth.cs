using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour, IDamageable, IPowerable, IMedkitInventory
{
    [Header("settings de la vida")]
    public int maxHealth = 100;
    private int currentHealth;

    [Header("Sistema de Medkits")]
    [SerializeField] private int maxMedkits = 5;
    private int currentMedkits = 0;
    [SerializeField] private int healAmountPerMedkit = 30;
    [SerializeField] private AudioClip useMedkitSound;

    [Header("efectofeles")]
    public GameObject bloodEffect;
    public Image healthBar;
    public float flashDuration = 0.1f;
    [SerializeField] private Material dithering;
    [SerializeField] Color flashColor = new Color(1, 0, 0, 0.25f);
    [SerializeField] Color healcolor = new Color(0, 1, 0, 0.25f);

    [Header("Sonidos")]
    public AudioClip hurtSound;
    public AudioClip deathSound;
    public AudioClip powerUPmusic;

    private ScreenFlash screenFlash;
    private bool isDead;
    private bool Healing;
    private bool powered;

    void Start()
    {
        currentHealth = maxHealth;
        screenFlash = FindObjectOfType<ScreenFlash>();

        // Disparar evento inicial de salud
        EventManager.TriggerEvent(GameEvents.UI_UPDATE_HEALTH, new DamageEventData
        {
            currentHealth = currentHealth,
            maxHealth = maxHealth
        });

        // Disparar evento inicial de medkits
        UpdateMedkitUI();

        // Disparar evento de inicio del juego
        EventManager.TriggerEvent(GameEvents.GAME_START, null);
    }

    void Update()
    {
        // Teclas de prueba
        if (Input.GetKeyDown(KeyCode.J))
            TakeDamage(20, Vector3.zero, Vector3.zero);

        // Usar medkit con la tecla H
        if (Input.GetKeyDown(KeyCode.H))
            UseMedkit();
    }

    // ==================== SISTEMA DE MEDKITS ====================

    public bool AddMedkit()
    {
        if (currentMedkits >= maxMedkits)
        {
            Debug.Log("Inventario de medkits lleno!");
            return false;
        }

        currentMedkits++;
        Debug.Log($"Medkit recogido! Total: {currentMedkits}/{maxMedkits}");

        UpdateMedkitUI();
        return true;
    }

    public bool UseMedkit()
    {
        // Verificar si tenemos medkits y no estamos con vida máxima
        if (currentMedkits <= 0)
        {
            Debug.Log("No tienes medkits!");
            return false;
        }

        if (currentHealth >= maxHealth)
        {
            Debug.Log("Ya tienes vida máxima!");
            return false;
        }

        if (isDead)
        {
            return false;
        }

        // Usar el medkit
        currentMedkits--;
        Heal(healAmountPerMedkit);

        if (useMedkitSound != null)
        {
            AudioManager.PlaySFX2D(useMedkitSound);
        }

        Debug.Log($"Medkit usado! Restantes: {currentMedkits}/{maxMedkits}");
        UpdateMedkitUI();

        return true;
    }

    public int GetMedkitCount()
    {
        return currentMedkits;
    }

    public int GetMaxMedkits()
    {
        return maxMedkits;
    }

    private void UpdateMedkitUI()
    {
        // Disparar evento para actualizar UI de medkits
        EventManager.TriggerEvent(GameEvents.UI_UPDATE_MEDKITS, new MedkitEventData
        {
            currentMedkits = currentMedkits,
            maxMedkits = maxMedkits
        });
    }

    // ==================== SISTEMA DE DAÑO Y CURACIÓN ====================

    // Implementación de IDamageable
    public bool TakeDamage(float amount, Vector3 hitPoint, Vector3 hitNormal)
    {
        if (!powered)
        {
            CalculateDamage(Mathf.RoundToInt(amount));
            return !isDead;
        }

        return !isDead;
    }

    public void CalculateDamage(int amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (hurtSound) AudioManager.PlaySFX2D(hurtSound);
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
        if (deathSound) AudioManager.PlaySFX2D(deathSound);

        EventManager.TriggerEvent(GameEvents.PLAYER_DIED, null);
        Debug.Log("te moriste wachin");

        // Disparar evento de Game Over para el ScreenManager
        EventManager.TriggerEvent(GameEvents.GAME_OVER, null);
    }

    public void Heal(float amount)
    {
        if (isDead) return;

        currentHealth += (int)amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (screenFlash) screenFlash.Flash(healcolor, flashDuration);

        // Actualizar UI de salud
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

    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    public void SetCurrentHealth(int value)
    {
        currentHealth = Mathf.Clamp(value, 0, maxHealth);

        EventManager.TriggerEvent(GameEvents.UI_UPDATE_HEALTH, new DamageEventData
        {
            currentHealth = currentHealth,
            maxHealth = maxHealth
        });
    }

    // ==================== SISTEMA DE POWER-UP ====================

    public void ActivatePowerUp(float duration)
    {
        StopAllCoroutines();
        StartCoroutine(PowerRoutine(duration));
    }

    System.Collections.IEnumerator PowerRoutine(float duration)
    {
        powered = true;
        AudioManager.PlaySFX2D(powerUPmusic);

        if (dithering != null)
            dithering.SetFloat("_Color_Dithering", 1f);

        yield return new WaitForSeconds(duration);

        powered = false;

        if (dithering != null)
            dithering.SetFloat("_Color_Dithering", 0f);
    }
}