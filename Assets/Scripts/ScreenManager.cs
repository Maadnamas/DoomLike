using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class ScreenManager : MonoBehaviour
{
    public static int Score = 0;
    public static int EnemiesKilled = 0;

    [Header("Referencias a grupos de UI")]
    [SerializeField] private GameObject hudGroup;
    [SerializeField] private GameObject victoryGroup;
    [SerializeField] private GameObject defeatGroup;

    [Header("Pantalla de Victoria")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI enemiesKilledText;
    [SerializeField] private Image rankImage;

    [SerializeField] private Button continueButton;
    [SerializeField] private Button menuButton;
    [SerializeField] private string menuSceneName = "MainMenuScene";

    [Header("Configuración de Animación de Victoria")]
    [SerializeField] private float fallDuration = 0.5f;
    [SerializeField] private float bounceHeight = 50f;
    [SerializeField] private float bounceDuration = 0.2f;
    [SerializeField] private float counterDuration = 1.5f;
    [SerializeField] private float initialOffScreenOffset = 1000f;

    [Header("Sprites de Ranking")]
    [SerializeField] private Sprite rankSSprite;
    [SerializeField] private Sprite rankASprite;
    [SerializeField] private Sprite rankBSprite;
    [SerializeField] private Sprite rankCSprite;
    [SerializeField] private Sprite rankDSprite;

    [Header("Fade")]
    [SerializeField] private CanvasGroup fadeCanvasGroup;
    [SerializeField] private float fadeDuration = 1f;

    private bool waitingForRestart = false;
    private Vector3 initialVictoryPanelPosition;
    private bool skipAnimationRequested = false;

    private Coroutine scoreAnimationCoroutine;
    private Coroutine enemiesAnimationCoroutine;


    private void OnEnable()
    {
        StartCoroutine(SubscribeWithDelay());
    }

    private void OnDisable()
    {
        UnsubscribeFromEvents();
    }

    private IEnumerator SubscribeWithDelay()
    {
        while (EventManager.Instance == null)
        {
            yield return null;
        }

        EventManager.StartListening(GameEvents.GAME_OVER, OnGameOver);
        EventManager.StartListening(GameEvents.GAME_VICTORY, OnGameVictory);
        EventManager.StartListening(GameEvents.GAME_START, OnGameStart);
    }

    private void UnsubscribeFromEvents()
    {
        if (EventManager.Instance == null) return;

        EventManager.StopListening(GameEvents.GAME_OVER, OnGameOver);
        EventManager.StopListening(GameEvents.GAME_VICTORY, OnGameVictory);
        EventManager.StopListening(GameEvents.GAME_START, OnGameStart);
    }

    private void Start()
    {
        ShowHUD();
        if (fadeCanvasGroup != null)
            fadeCanvasGroup.alpha = 0f;

        if (victoryGroup != null)
        {
            RectTransform victoryRect = victoryGroup.GetComponent<RectTransform>();
            if (victoryRect != null)
            {
                initialVictoryPanelPosition = victoryRect.localPosition;
            }
        }

        if (rankImage != null)
            rankImage.gameObject.SetActive(false);

        PlayerHealth.gameIsOver = false;
        ScreenManager.SetPlayerControl(true);
    }

    private void OnGameStart(object data)
    {
        StartCoroutine(SwitchScreen(hudGroup));
        PlayerHealth.gameIsOver = false;
        ScreenManager.SetPlayerControl(true);
    }

    private void OnGameVictory(object data)
    {
        PlayerHealth.gameIsOver = true;
        StartCoroutine(SwitchScreenAndAnimateVictory(victoryGroup, pauseGame: true));
        ScreenManager.SetPlayerControl(false);
    }

    private void OnGameOver(object data)
    {
        PlayerHealth.gameIsOver = true;
        StartCoroutine(SwitchScreen(defeatGroup, pauseGame: true));
        ScreenManager.SetPlayerControl(false);
    }

    private void ShowHUD()
    {
        hudGroup.SetActive(true);
        victoryGroup.SetActive(false);
        defeatGroup.SetActive(false);
    }

    private IEnumerator SwitchScreen(GameObject targetScreen, bool pauseGame = false)
    {
        if (fadeCanvasGroup != null)
            yield return StartCoroutine(Fade(1f));

        hudGroup.SetActive(false);
        victoryGroup.SetActive(false);
        defeatGroup.SetActive(false);

        targetScreen.SetActive(true);

        if (fadeCanvasGroup != null)
            yield return StartCoroutine(Fade(0f));

        if (pauseGame)
        {
            Time.timeScale = 0f;
            waitingForRestart = true;
        }
    }

    private IEnumerator SwitchScreenAndAnimateVictory(GameObject targetScreen, bool pauseGame = false)
    {
        if (fadeCanvasGroup != null)
            yield return StartCoroutine(Fade(1f));

        hudGroup.SetActive(false);
        defeatGroup.SetActive(false);
        targetScreen.SetActive(true);

        if (scoreText != null) scoreText.text = "0";
        if (enemiesKilledText != null) enemiesKilledText.text = "0";
        if (rankImage != null) rankImage.gameObject.SetActive(false);

        RectTransform rectToAnimate = targetScreen.GetComponent<RectTransform>();

        if (rectToAnimate != null)
        {
            rectToAnimate.localPosition = initialVictoryPanelPosition + Vector3.up * initialOffScreenOffset;
        }

        if (fadeCanvasGroup != null)
            yield return StartCoroutine(Fade(0f));

        if (rectToAnimate != null)
            yield return StartCoroutine(AnimateVictoryPanelFall(rectToAnimate));

        skipAnimationRequested = false;

        scoreAnimationCoroutine = StartCoroutine(CountUp(scoreText, Score, counterDuration));
        yield return scoreAnimationCoroutine;

        enemiesAnimationCoroutine = StartCoroutine(CountUp(enemiesKilledText, EnemiesKilled, counterDuration));
        yield return enemiesAnimationCoroutine;

        // Aquí NO se ejecuta el Debug. Se llama a UpdateRankImage para que lo imprima
        UpdateRankImage(Score);
        yield return StartCoroutine(AnimateRankAppearance());

        if (pauseGame)
        {
            Time.timeScale = 0f;
        }
    }

    private IEnumerator AnimateVictoryPanelFall(RectTransform rectToAnimate)
    {
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
        skipAnimationRequested = false;
    }

    private IEnumerator AnimateRankAppearance()
    {
        rankImage.gameObject.SetActive(true);
        rankImage.transform.localScale = Vector3.zero;

        float time = 0f;
        float popDuration = 0.2f;
        Vector3 targetScale = Vector3.one;

        while (time < popDuration && !skipAnimationRequested)
        {
            time += Time.unscaledDeltaTime;
            rankImage.transform.localScale = Vector3.Lerp(Vector3.zero, targetScale, time / popDuration);
            yield return null;
        }

        rankImage.transform.localScale = targetScale;
        skipAnimationRequested = false;
    }

    private void UpdateRankImage(int score)
    {
        if (SceneSetup.Instance == null)
        {
            Debug.LogError("SceneSetup.Instance es NULL.");
            return;
        }

        SceneSetup.RankData[] ranks = SceneSetup.Instance.Ranks;

        Sprite bestRankSprite = null;
        string finalRankName = "D"; // Por defecto es D

        foreach (var rankData in ranks)
        {
            if (score >= rankData.RequiredScore)
            {
                bestRankSprite = rankData.RankSprite;
                finalRankName = rankData.RankName; // Capturar el nombre
                break;
            }
        }

        // ¡DEBUG LOG MOVIDO AQUÍ! Se ejecuta siempre antes de mostrar la imagen.
        Debug.Log($"Puntaje final: {Score}. Rango Obtenido: {finalRankName}");

        if (rankImage != null && bestRankSprite != null)
        {
            rankImage.sprite = bestRankSprite;
        }
    }

    private string CalculateRank(int score)
    {
        if (SceneSetup.Instance == null)
        {
            return "N/A";
        }

        SceneSetup.RankData[] ranks = SceneSetup.Instance.Ranks;

        foreach (var rankData in ranks)
        {
            if (score >= rankData.RequiredScore)
            {
                return rankData.RankName;
            }
        }
        return "D";
    }

    public void OnContinueButtonPressed()
    {
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        StartCoroutine(LoadSceneWithFade(nextSceneIndex));
    }

    public void OnMenuButtonPressed()
    {
        StartCoroutine(LoadSceneWithFade(menuSceneName));
    }

    private IEnumerator LoadSceneWithFade(string sceneName)
    {
        Time.timeScale = 1f;
        ScreenManager.SetPlayerControl(true);
        PlayerHealth.gameIsOver = false;

        if (fadeCanvasGroup != null)
            yield return StartCoroutine(Fade(1f));

        SceneManager.LoadScene(sceneName);
    }

    private IEnumerator LoadSceneWithFade(int sceneIndex)
    {
        Time.timeScale = 1f;
        ScreenManager.SetPlayerControl(true);
        PlayerHealth.gameIsOver = false;

        if (fadeCanvasGroup != null)
            yield return StartCoroutine(Fade(1f));

        if (sceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(sceneIndex);
        }
        else
        {
            Debug.LogError("No hay más escenas en el índice de compilación. Cargando menú...");
            SceneManager.LoadScene(menuSceneName);
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (Time.timeScale == 0f)
            {
                if (waitingForRestart)
                {
                    waitingForRestart = false;
                    Time.timeScale = 1f;
                    ScreenManager.SetPlayerControl(true);
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                    return;
                }
            }
            // Si las animaciones están corriendo
            else if (scoreAnimationCoroutine != null || enemiesAnimationCoroutine != null || (rankImage != null && rankImage.gameObject.activeSelf))
            {
                skipAnimationRequested = true;

                // Forzar fin del conteo de Score
                if (scoreAnimationCoroutine != null)
                {
                    StopCoroutine(scoreAnimationCoroutine);
                    scoreText.text = Score.ToString();
                    scoreAnimationCoroutine = null;
                }
                // Forzar fin del conteo de Enemies
                if (enemiesAnimationCoroutine != null)
                {
                    StopCoroutine(enemiesAnimationCoroutine);
                    enemiesKilledText.text = EnemiesKilled.ToString();
                    enemiesAnimationCoroutine = null;
                }

                // Si aún no está visible, forzar la aparición de la imagen del ranking
                if (rankImage != null && !rankImage.gameObject.activeSelf)
                {
                    UpdateRankImage(Score); // Llama al método que hace el cálculo y el Debug.Log
                    rankImage.gameObject.SetActive(true);
                    rankImage.transform.localScale = Vector3.one;
                }

                skipAnimationRequested = false;
            }
        }
    }

    private IEnumerator Fade(float targetAlpha)
    {
        float startAlpha = fadeCanvasGroup.alpha;
        float time = 0f;

        while (time < fadeDuration)
        {
            time += Time.unscaledDeltaTime;
            fadeCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, time / fadeDuration);
            yield return null;
        }

        fadeCanvasGroup.alpha = targetAlpha;
    }

    public static void SetPlayerControl(bool enabled)
    {
        PlayerMovement.isControlEnabled = enabled;

        if (enabled)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}