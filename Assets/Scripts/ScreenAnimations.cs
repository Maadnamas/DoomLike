using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScreenAnimations : MonoBehaviour
{
    [Header("Referencias de UI")]
    [SerializeField] private RectTransform victoryPanelRect;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI enemiesKilledText;
    [SerializeField] private Image rankImage;
    [SerializeField] private Image rankImageLarva;

    [Header("Configuración de Animación")]
    [SerializeField] private float fallDuration = 0.5f;
    [SerializeField] private float bounceHeight = 50f;
    [SerializeField] private float bounceDuration = 0.2f;
    [SerializeField] private float counterDuration = 1.5f;
    [SerializeField] private float initialOffScreenOffset = 1000f;
    [SerializeField] private float rankPopDuration = 0.2f;

    [Header("Animación de Larva")]
    [SerializeField] private float larvaJumpHeight = 50f;
    [SerializeField] private float larvaJumpDuration = 0.3f;
    [SerializeField] private float larvaJumpInterval = 1f;
    [SerializeField] private float larvaEnterDuration = 0.5f;

    [Header("Rotación de Ranking Principal")]
    [SerializeField] private float rankRotationSpeed = 3f;
    [SerializeField] private float rankRotationMaxAngle = 10f;

    private Vector3 initialVictoryPanelPosition;
    private Vector3 initialLarvaPosition;
    private bool shouldLarvaJump = false;
    private bool skipAnimationRequested = false;

    private Coroutine scoreAnimationCoroutine;
    private Coroutine enemiesAnimationCoroutine;
    private Coroutine larvaJumpCoroutine;
    private Coroutine rankRotationCoroutine;


    private void Awake()
    {
        if (victoryPanelRect != null)
        {
            initialVictoryPanelPosition = victoryPanelRect.localPosition;
        }

        if (rankImageLarva != null)
        {
            initialLarvaPosition = rankImageLarva.rectTransform.localPosition;
        }
    }

    public void RequestSkip()
    {
        if (scoreAnimationCoroutine != null)
        {
            StopCoroutine(scoreAnimationCoroutine);
            scoreAnimationCoroutine = null;
        }
        if (enemiesAnimationCoroutine != null)
        {
            StopCoroutine(enemiesAnimationCoroutine);
            enemiesAnimationCoroutine = null;
        }

        if (scoreText != null)
        {
            scoreText.text = ScreenManager.Score.ToString();
        }
        if (enemiesKilledText != null)
        {
            enemiesKilledText.text = ScreenManager.EnemiesKilled.ToString();
        }

        // --- ACTIVACIÓN FORZADA DE RANKING PRINCIPAL Y ROTACIÓN ---
        if (rankImage != null)
        {
            rankImage.gameObject.SetActive(true);
            rankImage.transform.localScale = Vector3.one;
            // Iniciar la rotación inmediatamente al saltear si no está ya activa
            if (rankRotationCoroutine == null)
            {
                rankRotationCoroutine = StartCoroutine(RotateRankImage());
            }
        }
        // --------------------------------------------------------

        if (rankImageLarva != null)
        {
            rankImageLarva.gameObject.SetActive(true);
            rankImageLarva.rectTransform.localPosition = initialLarvaPosition;
        }

        if (shouldLarvaJump && larvaJumpCoroutine == null)
        {
            larvaJumpCoroutine = StartCoroutine(JumpRoutine());
        }

        skipAnimationRequested = true;
    }

    public void SetLarvaJumpState(bool jumpState)
    {
        shouldLarvaJump = jumpState;
    }

    public bool IsAnimationPlaying()
    {
        return scoreAnimationCoroutine != null || enemiesAnimationCoroutine != null;
    }

    // ------------------------------------------
    // MÉTODOS PÚBLICOS DE INICIO DE ANIMACIÓN
    // ------------------------------------------

    public IEnumerator StartVictorySequence(int score, int enemiesKilled)
    {
        if (rankRotationCoroutine != null) StopCoroutine(rankRotationCoroutine);
        if (larvaJumpCoroutine != null) StopCoroutine(larvaJumpCoroutine);

        if (scoreText != null) scoreText.text = "0";
        if (enemiesKilledText != null) enemiesKilledText.text = "0";
        if (rankImage != null) rankImage.gameObject.SetActive(false);
        if (rankImageLarva != null) rankImageLarva.gameObject.SetActive(false);

        // Resetear la rotación para que inicie desde cero
        if (rankImage != null) rankImage.rectTransform.localRotation = Quaternion.identity;

        skipAnimationRequested = false;

        if (victoryPanelRect != null)
        {
            victoryPanelRect.localPosition = initialVictoryPanelPosition + Vector3.up * initialOffScreenOffset;
            yield return StartCoroutine(AnimateVictoryPanelFall());
        }

        scoreAnimationCoroutine = StartCoroutine(CountUp(scoreText, score, counterDuration));
        yield return scoreAnimationCoroutine;

        enemiesAnimationCoroutine = StartCoroutine(CountUp(enemiesKilledText, enemiesKilled, counterDuration));
        yield return enemiesAnimationCoroutine;

        StartCoroutine(AnimateRankAppearance());
        rankRotationCoroutine = StartCoroutine(RotateRankImage()); // La rotación se inicia inmediatamente con el pop-in

        yield return new WaitForSecondsRealtime(rankPopDuration);

        yield return StartCoroutine(AnimateLarvaEnter());

        if (shouldLarvaJump)
        {
            larvaJumpCoroutine = StartCoroutine(JumpRoutine());
        }
    }

    // ------------------------------------------
    // COROUTINES PRIVADOS
    // ------------------------------------------

    private IEnumerator AnimateVictoryPanelFall()
    {
        RectTransform rectToAnimate = victoryPanelRect;
        Vector3 startPos = rectToAnimate.localPosition;
        Vector3 targetPos = initialVictoryPanelPosition;

        float time = 0f;
        while (time < fallDuration)
        {
            time += Time.unscaledDeltaTime;
            float t = time / fallDuration;
            rectToAnimate.localPosition = Vector3.Lerp(startPos, targetPos, t);
            yield return null;
        }
        rectToAnimate.localPosition = targetPos;

        Vector3 bounceUpPos = targetPos + Vector3.up * bounceHeight;
        time = 0f;
        while (time < bounceDuration)
        {
            time += Time.unscaledDeltaTime;
            float t = time / bounceDuration;
            rectToAnimate.localPosition = Vector3.Lerp(targetPos, bounceUpPos, t);
            yield return null;
        }
        rectToAnimate.localPosition = bounceUpPos;

        time = 0f;
        while (time < bounceDuration)
        {
            time += Time.unscaledDeltaTime;
            float t = time / bounceDuration;
            rectToAnimate.localPosition = Vector3.Lerp(bounceUpPos, targetPos, t);
            yield return null;
        }
        rectToAnimate.localPosition = targetPos;
    }

    private IEnumerator CountUp(TextMeshProUGUI text, int targetValue, float duration)
    {
        float startTime = Time.unscaledTime;
        float endTime = startTime + duration;
        int startValue = 0;

        while (Time.unscaledTime < endTime && !skipAnimationRequested)
        {
            float elapsed = Time.unscaledTime - startTime;
            float t = elapsed / duration;
            int currentValue = Mathf.RoundToInt(Mathf.Lerp(startValue, targetValue, t));
            text.text = currentValue.ToString();
            yield return null;
        }

        text.text = targetValue.ToString();
    }

    private IEnumerator AnimateRankAppearance()
    {
        if (rankImage == null) yield break;

        rankImage.gameObject.SetActive(true);
        rankImage.transform.localScale = Vector3.zero;

        float time = 0f;
        Vector3 targetScale = Vector3.one;

        while (time < rankPopDuration && !skipAnimationRequested)
        {
            time += Time.unscaledDeltaTime;
            rankImage.transform.localScale = Vector3.Lerp(Vector3.zero, targetScale, time / rankPopDuration);
            yield return null;
        }

        rankImage.transform.localScale = targetScale;
    }

    private IEnumerator RotateRankImage()
    {
        if (rankImage == null) yield break;
        RectTransform rect = rankImage.rectTransform;

        float time = 0f;

        while (true)
        {
            time += Time.unscaledDeltaTime * rankRotationSpeed;
            // La rotación se aplica al eje Z (rotación lateral)
            float angle = Mathf.Sin(time) * rankRotationMaxAngle;
            rect.localRotation = Quaternion.Euler(0f, 0f, angle);
            yield return null;
        }
    }

    private IEnumerator AnimateLarvaEnter()
    {
        if (rankImageLarva == null) yield break;

        rankImageLarva.gameObject.SetActive(true);
        RectTransform rect = rankImageLarva.rectTransform;

        Vector3 startPos = initialLarvaPosition + Vector3.down * Screen.height;
        Vector3 targetPos = initialLarvaPosition;

        rect.localPosition = startPos;

        float time = 0f;
        while (time < larvaEnterDuration && !skipAnimationRequested)
        {
            time += Time.unscaledDeltaTime;
            float t = time / larvaEnterDuration;
            rect.localPosition = Vector3.Lerp(startPos, targetPos, t);
            yield return null;
        }

        rect.localPosition = targetPos;
        skipAnimationRequested = false;
    }

    private IEnumerator JumpRoutine()
    {
        if (rankImageLarva == null) yield break;
        RectTransform rect = rankImageLarva.rectTransform;
        Vector3 restPos = initialLarvaPosition;

        while (shouldLarvaJump)
        {
            float time = 0f;

            while (time < larvaJumpDuration)
            {
                time += Time.unscaledDeltaTime;
                float t = time / larvaJumpDuration;
                float yOffset = Mathf.Sin(t * Mathf.PI) * larvaJumpHeight;
                rect.localPosition = restPos + Vector3.up * yOffset;
                yield return null;
            }

            rect.localPosition = restPos;
            yield return new WaitForSecondsRealtime(larvaJumpInterval);
        }
        rect.localPosition = restPos;
    }
}