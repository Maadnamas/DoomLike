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
    [SerializeField] private ScreenAnimations screenAnimations; // NUEVO: Referencia al script de animaciones

    [Header("Pantalla de Victoria (Solo para Actualizar Datos)")]
    [SerializeField] private Image rankImage;
    [SerializeField] private Image rankImageLarva;

    [SerializeField] private Button continueButton;
    [SerializeField] private Button menuButton;
    [SerializeField] private string menuSceneName = "MainMenuScene";

    [Header("Fade")]
    [SerializeField] private CanvasGroup fadeCanvasGroup;
    [SerializeField] private float fadeDuration = 1f;

    private bool waitingForRestart = false;

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

        if (rankImage != null)
            rankImage.gameObject.SetActive(false);

        if (rankImageLarva != null)
            rankImageLarva.gameObject.SetActive(false);

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
        if (screenAnimations == null)
        {
            Debug.LogError("ScreenAnimations no está asignado.");
            yield break;
        }

        if (fadeCanvasGroup != null)
            yield return StartCoroutine(Fade(1f));

        hudGroup.SetActive(false);
        defeatGroup.SetActive(false);
        targetScreen.SetActive(true);

        // Determinar rank y actualizar sprites ANTES de iniciar animaciones
        string finalRank = CalculateRank(Score);
        Debug.Log($"Puntaje final: {Score}. Rango Obtenido: {finalRank}");

        UpdateRankImages(Score);

        // Delegar la secuencia de caída, conteo y aparición de imágenes al nuevo script
        yield return StartCoroutine(screenAnimations.StartVictorySequence(Score, EnemiesKilled));

        if (pauseGame)
        {
            Time.timeScale = 0f;
        }
    }

    private void UpdateRankImages(int score)
    {
        if (SceneSetup.Instance == null || screenAnimations == null) return;

        SceneSetup.RankData[] ranks = SceneSetup.Instance.Ranks;

        Sprite bestRankSprite = null;
        Sprite bestLarvaSprite = null;
        bool shouldJump = false;

        foreach (var rankData in ranks)
        {
            if (score >= rankData.RequiredScore)
            {
                bestRankSprite = rankData.RankSprite;
                bestLarvaSprite = rankData.LarvaSprite;
                shouldJump = rankData.LarvaJumpRank;
                break;
            }
        }

        // Asignar sprites a las imágenes
        if (rankImage != null && bestRankSprite != null)
        {
            rankImage.sprite = bestRankSprite;
        }
        if (rankImageLarva != null && bestLarvaSprite != null)
        {
            rankImageLarva.sprite = bestLarvaSprite;
        }

        // Comunicar el estado de salto a ScreenAnimations
        screenAnimations.SetLarvaJumpState(shouldJump);
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
            // Delegar la lógica de salteo al nuevo script
            else if (screenAnimations != null && screenAnimations.IsAnimationPlaying())
            {
                screenAnimations.RequestSkip();

                // Forzar los estados finales que RequestSkip no puede manejar automáticamente
                UpdateRankImages(Score); // Asegura que los sprites y la configuración de salto estén correctos
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