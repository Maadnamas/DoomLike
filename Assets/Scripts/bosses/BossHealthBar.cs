using UnityEngine;
using TMPro;

public class BossHealthBar : MonoBehaviour
{
    [Header("Referencias UI")]
    public UnityEngine.UI.Slider healthSlider; // Especificar que es de UI
    public TextMeshProUGUI bossNameText;
    public GameObject healthBarPanel;

    [Header("Configuración")]
    public string bossName = "BOSS";

    private void OnEnable()
    {
        BossHealth.OnBossHealthChanged += UpdateHealthBar;
        BossHealth.OnBossDied += OnBossDied;
    }

    private void OnDisable()
    {
        BossHealth.OnBossHealthChanged -= UpdateHealthBar;
        BossHealth.OnBossDied -= OnBossDied;
    }

    private void Start()
    {
        if (bossNameText != null)
            bossNameText.text = bossName;

        if (healthBarPanel != null)
            healthBarPanel.SetActive(true);
    }

    private void UpdateHealthBar(float currentHealth, float maxHealth)
    {
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }
    }

    private void OnBossDied()
    {
        if (healthBarPanel != null)
        {
            // Opcional: hacer fade out o animación
            healthBarPanel.SetActive(false);
        }
    }
}