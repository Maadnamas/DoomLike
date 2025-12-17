using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.EventSystems;

public class ScreenManager : MonoBehaviour
{
    public static ScreenManager Instance { get; private set; }

    public static bool isCinematicActive = false;
    public static int Score = 0;
    public static int EnemiesKilled = 0;

    [Header("Referencias a grupos de UI")]
    [SerializeField] private GameObject hudGroup;
    [SerializeField] private GameObject victoryGroup;
    [SerializeField] private GameObject defeatGroup;
    [SerializeField] private GameObject pauseGroup;
    [SerializeField] private ScreenAnimations screenAnimations;

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
    private bool isPaused = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            isCinematicActive = false;
        }
    }

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

        if (pauseGroup != null)
            pauseGroup.SetActive(false);

        Time.timeScale = 1f;
        isPaused = false;
        PlayerHealth.gameIsOver = false;
        ScreenManager.SetPlayerControl(true);
        AudioListener.pause = false;
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

        SceneSetup.StopBackgroundMusic();

        StartCoroutine(SwitchScreen(defeatGroup, pauseGame: true));
        ScreenManager.SetPlayerControl(false);
    }

    public bool IsGamePaused()
    {
        return isPaused;
    }

    public void EnginePause()
    {
        Time.timeScale = 0f;
        ScreenManager.SetPlayerControl(false);
    }

    public void EngineResume()
    {
        Time.timeScale = 1f;
        ScreenManager.SetPlayerControl(true);
    }

    public void OnPauseGame()
    {
        if (PlayerHealth.gameIsOver || isPaused) return;

        isPaused = true;
        EnginePause();

        SceneSetup.PauseBackgroundMusic();

        if (hudGroup != null) hudGroup.SetActive(false);
        if (pauseGroup != null) pauseGroup.SetActive(true);

        EventManager.TriggerEvent(GameEvents.GAME_PAUSED, null);
    }

    public void OnResumeGame()
    {
        if (!isPaused) return;

        isPaused = false;

        if (!isCinematicActive)
        {
            EngineResume();
        }

        SceneSetup.ResumeBackgroundMusic();

        if (pauseGroup != null) pauseGroup.SetActive(false);

        if (hudGroup != null)
        {
            hudGroup.SetActive(!isCinematicActive);
        }

        EventManager.TriggerEvent(GameEvents.GAME_RESUMED, null);
    }

    public void ToggleHUD(bool active)
    {
        if (hudGroup != null)
            hudGroup.SetActive(active);
    }

    private void ShowHUD()
    {
        ToggleHUD(true);
        victoryGroup.SetActive(false);
        defeatGroup.SetActive(false);
        if (pauseGroup != null) pauseGroup.SetActive(false);
    }

    private IEnumerator SwitchScreen(GameObject targetScreen, bool pauseGame = false)
    {
        if (isPaused) OnResumeGame();

        if (fadeCanvasGroup != null)
            yield return StartCoroutine(Fade(1f));

        ToggleHUD(false);
        victoryGroup.SetActive(false);
        defeatGroup.SetActive(false);
        if (pauseGroup != null) pauseGroup.SetActive(false);

        targetScreen.SetActive(true);

        if (fadeCanvasGroup != null)
            yield return StartCoroutine(Fade(0f));

        if (pauseGame)
        {
            EnginePause();
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

        ToggleHUD(false);
        defeatGroup.SetActive(false);
        if (pauseGroup != null) pauseGroup.SetActive(false);
        targetScreen.SetActive(true);

        string finalRank = CalculateRank(Score);
        Debug.Log($"Puntaje final: {Score}. Rango Obtenido: {finalRank}");

        UpdateRankImages(Score);

        yield return StartCoroutine(screenAnimations.StartVictorySequence(Score, EnemiesKilled));

        if (pauseGame)
        {
            EnginePause();
            waitingForRestart = true;
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

        if (rankImage != null && bestRankSprite != null)
        {
            rankImage.sprite = bestRankSprite;
        }
        if (rankImageLarva != null && bestLarvaSprite != null)
        {
            rankImageLarva.sprite = bestLarvaSprite;
        }

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
        if (fadeCanvasGroup != null)
            yield return StartCoroutine(Fade(1f));

        EngineResume();

        SceneManager.LoadScene(sceneName);
    }

    private IEnumerator LoadSceneWithFade(int sceneIndex)
    {
        if (fadeCanvasGroup != null)
            yield return StartCoroutine(Fade(1f));

        EngineResume();

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
        if (Input.GetKeyDown(KeyCode.Escape) && !PlayerHealth.gameIsOver && !waitingForRestart)
        {
            if (isPaused)
            {
                OnResumeGame();
            }
            else
            {
                OnPauseGame();
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            // Caso: Juego pausado por Victoria/Derrota
            if (Time.timeScale == 0f)
            {
                // Solo permitimos SALTEAR la animación si la pantalla de victoria está activa,
                // la animación está en curso, Y el clic NO está sobre un elemento de UI (botón).
                if (victoryGroup.activeSelf && screenAnimations != null && screenAnimations.IsAnimationPlaying())
                {
                    if (!EventSystem.current.IsPointerOverGameObject())
                    {
                        screenAnimations.RequestSkip();

                        UpdateRankImages(Score);
                        string finalRank = CalculateRank(Score);
                        Debug.Log($"Puntaje final: {Score}. Rango Obtenido: {finalRank}");
                    }
                }
            }
            // Caso: Juego activo (saltear diálogos/cinemáticas que corren con timeScale=1)
            else if (screenAnimations != null && screenAnimations.IsAnimationPlaying())
            {
                screenAnimations.RequestSkip();

                UpdateRankImages(Score);
                string finalRank = CalculateRank(Score);
                Debug.Log($"Puntaje final: {Score}. Rango Obtenido: {finalRank}");
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