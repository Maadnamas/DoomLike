using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour, IDamageable, IPowerable, IMedkitInventory
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    private int currentHealth;

    [Header("Medkit System")]
    [SerializeField] private int maxMedkits = 5;
    private int currentMedkits = 0;
    [SerializeField] private int healAmountPerMedkit = 30;
    [SerializeField] private AudioClip useMedkitSound;

    [Header("Visual Effects")]
    public GameObject bloodEffect;
    public Image healthBar;
    public float flashDuration = 0.1f;
    [SerializeField] private Material dithering;
    [SerializeField] Color flashColor = new Color(1, 0, 0, 0.25f);
    [SerializeField] Color healColor = new Color(0, 1, 0, 0.25f);

    [Header("Sounds")]
    public AudioClip hurtSound;
    public AudioClip deathSound;
    public AudioClip powerUpMusic;

    private ScreenFlash screenFlash;
    private bool isDead;
    private bool isHealing;
    private bool isPowered;
    public static bool gameIsOver = false;

    void Start()
    {
        currentHealth = maxHealth;
        screenFlash = FindObjectOfType<ScreenFlash>();

        PlayerHealth.gameIsOver = false;

        EventManager.TriggerEvent(GameEvents.UI_UPDATE_HEALTH, new DamageEventData
        {
            currentHealth = currentHealth,
            maxHealth = maxHealth
        });

        UpdateMedkitUI();

        EventManager.TriggerEvent(GameEvents.GAME_START, null);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
            UseMedkit();
    }

    public bool AddMedkit()
    {
        if (currentMedkits >= maxMedkits)
        {
            Debug.Log("Medkit inventory full!");
            return false;
        }

        currentMedkits++;
        Debug.Log($"Medkit collected! Total: {currentMedkits}/{maxMedkits}");

        UpdateMedkitUI();
        return true;
    }

    public bool UseMedkit()
    {
        if (currentMedkits <= 0)
        {
            Debug.Log("No medkits available!");
            return false;
        }

        if (currentHealth >= maxHealth)
        {
            Debug.Log("Health is already full!");
            return false;
        }

        if (isDead) return false;

        currentMedkits--;
        Heal(healAmountPerMedkit);

        if (useMedkitSound != null)
        {
            AudioManager.PlaySFX2D(useMedkitSound);
        }

        Debug.Log($"Medkit used! Remaining: {currentMedkits}/{maxMedkits}");
        UpdateMedkitUI();

        return true;
    }

    public int GetMedkitCount() => currentMedkits;
    public int GetMaxMedkits() => maxMedkits;

    private void UpdateMedkitUI()
    {
        EventManager.TriggerEvent(GameEvents.UI_UPDATE_MEDKITS, new MedkitEventData
        {
            currentMedkits = currentMedkits,
            maxMedkits = maxMedkits
        });
    }

    public bool TakeDamage(float amount, Vector3 hitPoint, Vector3 hitNormal)
    {
        if (gameIsOver) return false;

        if (!isPowered)
        {
            CalculateDamage(Mathf.RoundToInt(amount));
            return !isDead;
        }

        return !isDead;
    }

    public void CalculateDamage(int amount)
    {
        if (isDead || gameIsOver) return;

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (hurtSound) AudioManager.PlaySFX2D(hurtSound);
        if (bloodEffect) Instantiate(bloodEffect, transform.position, Quaternion.identity);
        if (screenFlash) screenFlash.Flash(flashColor, flashDuration);

        EventManager.TriggerEvent(GameEvents.UI_UPDATE_HEALTH, new DamageEventData
        {
            damageAmount = amount,
            currentHealth = currentHealth,
            maxHealth = maxHealth
        });

        EventManager.TriggerEvent(GameEvents.PLAYER_DAMAGED, new DamageEventData
        {
            damageAmount = amount,
            currentHealth = currentHealth,
            maxHealth = maxHealth
        });

        if (currentHealth <= 0)
            Die();
    }

    public void SetMedkitCount(int count)
    {
        currentMedkits = Mathf.Clamp(count, 0, maxMedkits);

        EventManager.TriggerEvent(GameEvents.UI_UPDATE_MEDKITS, new MedkitEventData
        {
            currentMedkits = currentMedkits,
            maxMedkits = maxMedkits
        });

        Debug.Log("Medkits updated to: " + currentMedkits);
    }

    public void Die()
    {
        isDead = true;
        PlayerHealth.gameIsOver = true;
        if (deathSound) AudioManager.PlaySFX2D(deathSound);

        ScreenManager.SetPlayerControl(false);

        EventManager.TriggerEvent(GameEvents.PLAYER_DIED, null);
        Debug.Log("Player died.");

        EventManager.TriggerEvent(GameEvents.GAME_OVER, null);
    }

    public void Heal(float amount)
    {
        if (isDead) return;

        currentHealth += (int)amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (screenFlash) screenFlash.Flash(healColor, flashDuration);

        EventManager.TriggerEvent(GameEvents.UI_UPDATE_HEALTH, new DamageEventData
        {
            currentHealth = currentHealth,
            maxHealth = maxHealth
        });

        EventManager.TriggerEvent(GameEvents.PLAYER_HEALED, new HealEventData
        {
            healAmount = (int)amount,
            currentHealth = currentHealth
        });
    }

    public int GetCurrentHealth() => currentHealth;

    public void SetCurrentHealth(int value)
    {
        currentHealth = Mathf.Clamp(value, 0, maxHealth);

        EventManager.TriggerEvent(GameEvents.UI_UPDATE_HEALTH, new DamageEventData
        {
            currentHealth = currentHealth,
            maxHealth = maxHealth
        });
    }

    public void ActivatePowerUp(float duration)
    {
        StopAllCoroutines();
        StartCoroutine(PowerRoutine(duration));
    }

    System.Collections.IEnumerator PowerRoutine(float duration)
    {
        isPowered = true;
        AudioManager.PlaySFX2D(powerUpMusic);

        if (dithering != null)
            dithering.SetFloat("_Color_Dithering", 1f);

        yield return new WaitForSeconds(duration);

        isPowered = false;

        if (dithering != null)
            dithering.SetFloat("_Color_Dithering", 0f);
    }
}