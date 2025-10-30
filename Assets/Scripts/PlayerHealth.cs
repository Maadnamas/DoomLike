using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
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

        UpdateHealthBar();
    }

    void Update()
    {
    
        if (Input.GetKeyDown(KeyCode.J))                                                      //Daños y curaciones de prueba (J = DAÑO) (H = HEAL)
            TakeDamage(20);                                                                   //Daños y curaciones de prueba (J = DAÑO) (H = HEAL)
                                                                                              //Daños y curaciones de prueba (J = DAÑO) (H = HEAL)
        if (Input.GetKeyDown(KeyCode.H))                                                      //Daños y curaciones de prueba (J = DAÑO) (H = HEAL)
            Heal(15);                                                                         //Daños y curaciones de prueba (J = DAÑO) (H = HEAL)
    }                                                                                         //Daños y curaciones de prueba (J = DAÑO) (H = HEAL)

    public void TakeDamage(int amount)
    {
        if (isDead) return;

        currentHealth -= amount;

        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (hurtSound) audioSource.PlayOneShot(hurtSound);

        if (bloodEffect) Instantiate(bloodEffect, transform.position, Quaternion.identity);

        if (screenFlash) screenFlash.Flash(flashColor, flashDuration);

        UpdateHealthBar();

        if (currentHealth <= 0)
            Die();
    }

    void Die()
    {
        isDead = true;
        if (deathSound) audioSource.PlayOneShot(deathSound);
        Debug.Log("te moriste wachin, corte Imelda");
    }

    public void Heal(int amount)
    {
        if (isDead) return;
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthBar();
    }

    void UpdateHealthBar()
    {
        if (healthBar)
            healthBar.fillAmount = (float)currentHealth / maxHealth;
    }
}
