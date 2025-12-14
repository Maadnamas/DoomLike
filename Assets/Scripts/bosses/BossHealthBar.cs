using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BossHealthBar : MonoBehaviour
{
    [Header("Referencias UI")]
    public Image healthBarFill; // La imagen que se va a achicar
    public TextMeshProUGUI bossNameText;
    public GameObject healthBarPanel;

    [Header("Configuración")]
    public string bossName = "BOSS";

    [Header("Colores (Opcional)")]
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
        // Ocultar el panel al inicio
        if (healthBarPanel != null)
            healthBarPanel.SetActive(false);
    }

    private void UpdateHealthBar(float currentHealth, float maxHealth)
    {
        // Mostrar el panel la primera vez
        if (healthBarPanel != null && !healthBarPanel.activeSelf)
        {
            healthBarPanel.SetActive(true);

            if (bossNameText != null)
                bossNameText.text = bossName;
        }

        if (healthBarFill != null)
        {
            // Calcular porcentaje de vida
            float healthPercent = Mathf.Clamp01(currentHealth / maxHealth);

            // Achicar la imagen (fillAmount)
            healthBarFill.fillAmount = healthPercent;

            // Cambiar color según la vida (opcional)
            if (useGradientColor)
            {
                if (healthPercent > 0.5f)
                {
                    // Verde a amarillo
                    healthBarFill.color = Color.Lerp(halfHealthColor, fullHealthColor, (healthPercent - 0.5f) * 2f);
                }
                else
                {
                    // Amarillo a rojo
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