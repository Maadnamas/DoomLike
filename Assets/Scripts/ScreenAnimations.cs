using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScreenAnimations : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private RectTransform victoryPanelRect;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI enemiesKilledText;
    [SerializeField] private Image rankImage;
    [SerializeField] private Image rankImageLarva;

    [Header("Animation Settings")]
    [SerializeField] private float fallDuration = 0.5f;
    [SerializeField] private float bounceHeight = 50f;
    [SerializeField] private float bounceDuration = 0.2f;
    [SerializeField] private float counterDuration = 1.5f;
    [SerializeField] private float initialOffScreenOffset = 1000f;
    [SerializeField] private float rankPopDuration = 0.2f;

    [Header("Larva Animation")]
    [SerializeField] private float larvaJumpHeight = 50f;
    [SerializeField] private float larvaJumpDuration = 0.3f;
    [SerializeField] private float larvaJumpInterval = 1f;
    [SerializeField] private float larvaEnterDuration = 0.5f;

    [Header("Main Rank Rotation")]
    [SerializeField] private float rankRotationSpeed = 3f;
    [SerializeField] private float rankRotationMaxAngle = 10f;

    [Header("Audio (Counting & Appearance)")]
    [SerializeField] private AudioClip victoryStartSound;
    [SerializeField] private AudioClip countIncrementSound;
    [SerializeField] private AudioClip countEndSound;
    [SerializeField] private AudioClip rankAppearSound;
    [SerializeField] private AudioClip larvaAppearSound;

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

        AudioManager.StopSFXLong();

        if (scoreText != null)
        {
            scoreText.text = ScreenManager.Score.ToString();
        }
        if (enemiesKilledText != null)
        {
            enemiesKilledText.text = ScreenManager.EnemiesKilled.ToString();
        }

        if (countEndSound != null)
        {
            AudioManager.PlaySFX2D(countEndSound);
        }

        if (rankImage != null)
        {
            rankImage.gameObject.SetActive(true);
            rankImage.transform.localScale = Vector3.one;

            if (rankAppearSound != null)
            {
                AudioManager.PlaySFX2D(rankAppearSound);
            }

            if (rankRotationCoroutine == null)
            {
                rankRotationCoroutine = StartCoroutine(RotateRankImage());
            }
        }

        if (rankImageLarva != null)
        {
            rankImageLarva.gameObject.SetActive(true);
            rankImageLarva.rectTransform.localPosition = initialLarvaPosition;

            if (shouldLarvaJump && larvaAppearSound != null)
            {
                AudioManager.PlaySFX2D(larvaAppearSound);
            }
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

    public IEnumerator StartVictorySequence(int score, int enemiesKilled)
    {
        if (rankRotationCoroutine != null) StopCoroutine(rankRotationCoroutine);
        if (larvaJumpCoroutine != null) StopCoroutine(larvaJumpCoroutine);

        AudioManager.StopSFXLong();

        if (scoreText != null) scoreText.text = "0";
        if (enemiesKilledText != null) enemiesKilledText.text = "0";
        if (rankImage != null) rankImage.gameObject.SetActive(false);
        if (rankImageLarva != null) rankImageLarva.gameObject.SetActive(false);

        if (rankImage != null) rankImage.rectTransform.localRotation = Quaternion.identity;

        skipAnimationRequested = false;

        if (victoryPanelRect != null)
        {
            victoryPanelRect.localPosition = initialVictoryPanelPosition + Vector3.up * initialOffScreenOffset;
            yield return StartCoroutine(AnimateVictoryPanelFall());
        }

        if (victoryStartSound != null)
        {
            AudioManager.PlaySFX2D(victoryStartSound);
        }

        scoreAnimationCoroutine = StartCoroutine(CountUp(scoreText, score, counterDuration));
        yield return scoreAnimationCoroutine;

        enemiesAnimationCoroutine = StartCoroutine(CountUp(enemiesKilledText, enemiesKilled, counterDuration));
        yield return enemiesAnimationCoroutine;

        yield return StartCoroutine(AnimateRankAppearance());

        rankRotationCoroutine = StartCoroutine(RotateRankImage());

        yield return new WaitForSecondsRealtime(rankPopDuration);

        yield return StartCoroutine(AnimateLarvaEnter());

        if (shouldLarvaJump)
        {
            larvaJumpCoroutine = StartCoroutine(JumpRoutine());
        }
    }

    private IEnumerator AnimateVictoryPanelFall()
    {
        RectTransform rectToAnimate = victoryPanelRect;
        Vector3 startPos = rectToAnimate.localPosition;
        Vector3 targetPos = initialVictoryPanelPosition;

        float time = 0f;
        while (time < fallDuration)
        {
            time += Time.unscaledDeltaTime;
            rectToAnimate.localPosition = Vector3.Lerp(startPos, targetPos, time / fallDuration);
            yield return null;
        }
        rectToAnimate.localPosition = targetPos;

        Vector3 bounceUpPos = targetPos + Vector3.up * bounceHeight;
        time = 0f;
        while (time < bounceDuration)
        {
            time += Time.unscaledDeltaTime;
            rectToAnimate.localPosition = Vector3.Lerp(targetPos, bounceUpPos, time / bounceDuration);
            yield return null;
        }

        time = 0f;
        while (time < bounceDuration)
        {
            time += Time.unscaledDeltaTime;
            rectToAnimate.localPosition = Vector3.Lerp(bounceUpPos, targetPos, time / bounceDuration);
            yield return null;
        }
        rectToAnimate.localPosition = targetPos;
    }

    private IEnumerator CountUp(TextMeshProUGUI text, int targetValue, float duration)
    {
        float startTime = Time.unscaledTime;
        float endTime = startTime + duration;

        if (countIncrementSound != null)
        {
            AudioManager.PlaySFXLong(countIncrementSound);
        }

        while (Time.unscaledTime < endTime && !skipAnimationRequested)
        {
            float elapsed = Time.unscaledTime - startTime;
            int currentValue = Mathf.RoundToInt(Mathf.Lerp(0, targetValue, elapsed / duration));
            text.text = currentValue.ToString();
            yield return null;
        }

        text.text = targetValue.ToString();
        AudioManager.StopSFXLong();

        if (countEndSound != null)
        {
            AudioManager.PlaySFX2D(countEndSound);
        }
    }

    private IEnumerator AnimateRankAppearance()
    {
        if (rankImage == null) yield break;

        rankImage.gameObject.SetActive(true);
        rankImage.transform.localScale = Vector3.zero;

        if (rankAppearSound != null)
        {
            AudioManager.PlaySFX2D(rankAppearSound);
        }

        float time = 0f;
        while (time < rankPopDuration && !skipAnimationRequested)
        {
            time += Time.unscaledDeltaTime;
            rankImage.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, time / rankPopDuration);
            yield return null;
        }

        rankImage.transform.localScale = Vector3.one;
    }

    private IEnumerator RotateRankImage()
    {
        if (rankImage == null) yield break;
        RectTransform rect = rankImage.rectTransform;

        float time = 0f;
        while (true)
        {
            time += Time.unscaledDeltaTime * rankRotationSpeed;
            rect.localRotation = Quaternion.Euler(0f, 0f, Mathf.Sin(time) * rankRotationMaxAngle);
            yield return null;
        }
    }

    private IEnumerator AnimateLarvaEnter()
    {
        if (rankImageLarva == null) yield break;

        rankImageLarva.gameObject.SetActive(true);
        RectTransform rect = rankImageLarva.rectTransform;

        if (shouldLarvaJump && larvaAppearSound != null)
        {
            AudioManager.PlaySFX2D(larvaAppearSound);
        }

        Vector3 startPos = initialLarvaPosition + Vector3.down * Screen.height;
        float time = 0f;
        while (time < larvaEnterDuration && !skipAnimationRequested)
        {
            time += Time.unscaledDeltaTime;
            rect.localPosition = Vector3.Lerp(startPos, initialLarvaPosition, time / larvaEnterDuration);
            yield return null;
        }

        rect.localPosition = initialLarvaPosition;
        skipAnimationRequested = false;
    }

    private IEnumerator JumpRoutine()
    {
        if (rankImageLarva == null) yield break;
        RectTransform rect = rankImageLarva.rectTransform;

        while (shouldLarvaJump)
        {
            float time = 0f;
            while (time < larvaJumpDuration)
            {
                time += Time.unscaledDeltaTime;
                rect.localPosition = initialLarvaPosition + Vector3.up * (Mathf.Sin((time / larvaJumpDuration) * Mathf.PI) * larvaJumpHeight);
                yield return null;
            }

            rect.localPosition = initialLarvaPosition;
            yield return new WaitForSecondsRealtime(larvaJumpInterval);
        }
    }
}