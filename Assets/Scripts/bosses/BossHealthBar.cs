using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BossHealthBar : MonoBehaviour
{
    [Header("UI References")]
    public Image healthBarFill;
    public TextMeshProUGUI bossNameText;
    public GameObject healthBarPanel;

    [Header("Configuration")]
    public string bossName = "BOSS";

    [Header("Colors (Optional)")]
    public bool useGradientColor = true;
    public Color fullHealthColor = Color.green;
    public Color halfHealthColor = Color.yellow;
    public Color lowHealthColor = Color.red;

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
        // Hide panel at start
        if (healthBarPanel != null)
            healthBarPanel.SetActive(false);
    }

    private void UpdateHealthBar(float currentHealth, float maxHealth)
    {
        // Show panel the first time
        if (healthBarPanel != null && !healthBarPanel.activeSelf)
        {
            healthBarPanel.SetActive(true);

            if (bossNameText != null)
                bossNameText.text = bossName;
        }

        if (healthBarFill != null)
        {
            // Calculate health percentage
            float healthPercent = Mathf.Clamp01(currentHealth / maxHealth);

            // Update fillAmount
            healthBarFill.fillAmount = healthPercent;

            // Gradient color (optional)
            if (useGradientColor)
            {
                if (healthPercent > 0.5f)
                {
                    // Green to yellow
                    healthBarFill.color = Color.Lerp(halfHealthColor, fullHealthColor, (healthPercent - 0.5f) * 2f);
                }
                else
                {
                    // Yellow to red
                    healthBarFill.color = Color.Lerp(lowHealthColor, halfHealthColor, healthPercent * 2f);
                }
            }
        }
    }

    private void OnBossDied()
    {
        if (healthBarPanel != null)
        {
            StartCoroutine(FadeOutPanel());
        }
    }

    private System.Collections.IEnumerator FadeOutPanel()
    {
        CanvasGroup canvasGroup = healthBarPanel.GetComponent<CanvasGroup>();

        if (canvasGroup == null)
            canvasGroup = healthBarPanel.AddComponent<CanvasGroup>();

        float duration = 1f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
            yield return null;
        }

        healthBarPanel.SetActive(false);
    }
}