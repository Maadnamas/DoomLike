using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ScreenManager : MonoBehaviour
{
    [Header("Referencias a grupos de UI")]
    [SerializeField] private GameObject hudGroup;
    [SerializeField] private GameObject victoryGroup;
    [SerializeField] private GameObject defeatGroup;

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

        // Habilitar control y bloquear cursor al inicio
        SetPlayerControl(true);
    }

    private void OnGameStart(object data)
    {
        StartCoroutine(SwitchScreen(hudGroup));
        SetPlayerControl(true);
    }

    private void OnGameVictory(object data)
    {
        StartCoroutine(SwitchScreen(victoryGroup, pauseGame: true));
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
        // Fade out
        if (fadeCanvasGroup != null)
            yield return StartCoroutine(Fade(1f));

        // Desactivar todos
        hudGroup.SetActive(false);
        victoryGroup.SetActive(false);
        defeatGroup.SetActive(false);

        // Activar el objetivo
        targetScreen.SetActive(true);

        // Fade in
        if (fadeCanvasGroup != null)
            yield return StartCoroutine(Fade(0f));

        if (pauseGame)
        {
            Time.timeScale = 0f;
            waitingForRestart = true;
        }
    }

    private void Update()
    {
        // Si está esperando para reiniciar y se toca cualquier tecla
        if (waitingForRestart && Input.anyKeyDown)
        {
            waitingForRestart = false;
            Time.timeScale = 1f;

            // Volver a habilitar controles al reiniciar
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
            time += Time.unscaledDeltaTime; // para que funcione con el juego pausado
            fadeCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, time / fadeDuration);
            yield return null;
        }

        fadeCanvasGroup.alpha = targetAlpha;
    }

    /// <summary>
    /// Habilita o deshabilita el control del jugador y ajusta el cursor.
    /// </summary>
    private void SetPlayerControl(bool enabled)
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