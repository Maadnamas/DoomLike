using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ScreenManager : MonoBehaviour
{
    public static int Score = 1500;
    public static int EnemiesKilled = 45;

    [SerializeField] private GameObject hudGroup;
    [SerializeField] private GameObject victoryGroup;
    [SerializeField] private GameObject defeatGroup;

    [SerializeField] private RectTransform victoryPanelToAnimate;
    [SerializeField] private Text scoreText;
    [SerializeField] private Text enemiesKilledText;
    [SerializeField] private Text rankText;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button menuButton;
    [SerializeField] private string nextSceneName = "NextLevelScene";
    [SerializeField] private string menuSceneName = "MainMenuScene";

    [SerializeField] private float fallDuration = 0.5f;
    [SerializeField] private float bounceHeight = 50f;
    [SerializeField] private float bounceDuration = 0.2f;

    [SerializeField] private CanvasGroup fadeCanvasGroup;
    [SerializeField] private float fadeDuration = 1f;

    private bool waitingForRestart = false;
    private Vector3 initialVictoryPanelPosition;


    private void OnEnable()
    {
        StartCoroutine(SubscribeWithDelay());

        if (continueButton != null)
            continueButton.onClick.AddListener(OnContinueButtonPressed);
        if (menuButton != null)
            menuButton.onClick.AddListener(OnMenuButtonPressed);
    }

    private void OnDisable()
    {
        UnsubscribeFromEvents();

        if (continueButton != null)
            continueButton.onClick.RemoveListener(OnContinueButtonPressed);
        if (menuButton != null)
            menuButton.onClick.RemoveListener(OnMenuButtonPressed);
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

        if (victoryPanelToAnimate != null)
        {
            initialVictoryPanelPosition = victoryPanelToAnimate.localPosition;
            victoryPanelToAnimate.gameObject.SetActive(false);
        }

        SetPlayerControl(true);
    }

    private void OnGameStart(object data)
    {
        StartCoroutine(SwitchScreen(hudGroup));
        SetPlayerControl(true);
    }

    private void OnGameVictory(object data)
    {
        UpdateVictoryScreen(Score, EnemiesKilled);

        StartCoroutine(SwitchScreenAndAnimateVictory(victoryGroup, pauseGame: true));
        SetPlayerControl(false);
    }

    private void OnGameOver(object data)
    {
        StartCoroutine(SwitchScreen(defeatGroup, pauseGame: true));
        SetPlayerControl(false);
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

        if (victoryPanelToAnimate != null)
        {
            victoryPanelToAnimate.gameObject.SetActive(true);
            victoryPanelToAnimate.localPosition = initialVictoryPanelPosition + Vector3.up * Screen.height;
        }

        if (fadeCanvasGroup != null)
            yield return StartCoroutine(Fade(0f));

        if (victoryPanelToAnimate != null)
            yield return StartCoroutine(AnimateVictoryPanelFall());

        if (pauseGame)
        {
            Time.timeScale = 0f;
        }
    }

    private IEnumerator AnimateVictoryPanelFall()
    {
        Vector3 startPos = victoryPanelToAnimate.localPosition;
        Vector3 targetPos = initialVictoryPanelPosition;

        float time = 0f;
        while (time < fallDuration)
        {
            time += Time.unscaledDeltaTime;
            float t = time / fallDuration;
            victoryPanelToAnimate.localPosition = Vector3.Lerp(startPos, targetPos, t);
            yield return null;
        }
        victoryPanelToAnimate.localPosition = targetPos;

        Vector3 bounceUpPos = targetPos + Vector3.up * bounceHeight;
        time = 0f;
        while (time < bounceDuration)
        {
            time += Time.unscaledDeltaTime;
            float t = time / bounceDuration;
            victoryPanelToAnimate.localPosition = Vector3.Lerp(targetPos, bounceUpPos, t);
            yield return null;
        }
        victoryPanelToAnimate.localPosition = bounceUpPos;

        time = 0f;
        while (time < bounceDuration)
        {
            time += Time.unscaledDeltaTime;
            float t = time / bounceDuration;
            victoryPanelToAnimate.localPosition = Vector3.Lerp(bounceUpPos, targetPos, t);
            yield return null;
        }
        victoryPanelToAnimate.localPosition = targetPos;
    }

    private void UpdateVictoryScreen(int score, int enemiesKilled)
    {
        if (scoreText != null)
            scoreText.text = score.ToString();

        if (enemiesKilledText != null)
            enemiesKilledText.text = enemiesKilled.ToString();

        if (rankText != null)
            rankText.text = CalculateRank(score);
    }

    private string CalculateRank(int score)
    {
        if (SceneSetup.Instance == null)
        {
            Debug.LogError("SceneSetup.Instance es NULL. Asegúrate de que un SceneSetup exista en la escena.");
            return "N/A";
        }

        SceneSetup config = SceneSetup.Instance;

        if (score >= config.RankSThreshold)
            return "S";
        else if (score >= config.RankAThreshold)
            return "A";
        else if (score >= config.RankBThreshold)
            return "B";
        else if (score >= config.RankCThreshold)
            return "C";
        else
            return "D";
    }

    public void OnContinueButtonPressed()
    {
        StartCoroutine(LoadSceneWithFade(nextSceneName));
    }

    public void OnMenuButtonPressed()
    {
        StartCoroutine(LoadSceneWithFade(menuSceneName));
    }

    private IEnumerator LoadSceneWithFade(string sceneName)
    {
        Time.timeScale = 1f;
        SetPlayerControl(true);

        if (fadeCanvasGroup != null)
            yield return StartCoroutine(Fade(1f));

        SceneManager.LoadScene(sceneName);
    }

    private void Update()
    {
        if (waitingForRestart && Input.anyKeyDown)
        {
            waitingForRestart = false;
            Time.timeScale = 1f;

            SetPlayerControl(true);

            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
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

    private void SetPlayerControl(bool enabled)
    {
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