using System.Collections;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using UnityEngine.Events;

public class CinematicTrigger : MonoBehaviour
{
    [Header("Video Configuration")]
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private RawImage videoOutputImage;
    [SerializeField] private VideoClip videoToPlay;

    [Header("Movement Configuration")]
    [SerializeField] private GameObject playerObject;
    [SerializeField] private Transform teleportDestination;

    [Header("Events")]
    public UnityEvent OnCinematicStart;
    public UnityEvent OnCinematicEnd;

    private bool hasPlayed = false;
    private bool isCinematicPlaying = false;
    private Coroutine cinematicSequenceCoroutine;

    private void Start()
    {
        if (videoOutputImage != null)
            videoOutputImage.enabled = false;

        if (videoPlayer != null)
            videoPlayer.loopPointReached += OnVideoFinished;
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

        EventManager.StartListening(GameEvents.GAME_PAUSED, OnGamePaused);
        EventManager.StartListening(GameEvents.GAME_RESUMED, OnGameResumed);
    }

    private void UnsubscribeFromEvents()
    {
        if (EventManager.Instance == null) return;

        EventManager.StopListening(GameEvents.GAME_PAUSED, OnGamePaused);
        EventManager.StopListening(GameEvents.GAME_RESUMED, OnGameResumed);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasPlayed)
        {
            hasPlayed = true;
            cinematicSequenceCoroutine = StartCoroutine(PlayCinematicSequence());
        }
    }

    private void Update()
    {
        if (isCinematicPlaying && Input.GetKeyDown(KeyCode.Return))
        {
            SkipCinematic();
        }
    }

    private void SkipCinematic()
    {
        if (isCinematicPlaying)
        {
            if (videoPlayer != null)
            {
                videoPlayer.Stop();
            }

            if (cinematicSequenceCoroutine != null)
            {
                StopCoroutine(cinematicSequenceCoroutine);
            }

            cinematicSequenceCoroutine = StartCoroutine(FinishSequence());
        }
    }

    private IEnumerator PlayCinematicSequence()
    {
        isCinematicPlaying = true;
        ScreenManager.isCinematicActive = true;

        if (ScreenManager.Instance != null)
        {
            ScreenManager.Instance.ToggleHUD(false);
            ScreenManager.Instance.EnginePause();
        }

        OnCinematicStart.Invoke();

        if (videoPlayer != null && videoToPlay != null)
        {
            videoPlayer.clip = videoToPlay;
            videoPlayer.Prepare();

            while (!videoPlayer.isPrepared)
            {
                yield return null;
            }

            if (videoOutputImage != null) videoOutputImage.enabled = true;
            videoPlayer.Play();
        }
        else
        {
            Debug.LogError("VideoPlayer or VideoClip missing in inspector.");
            OnVideoFinished(videoPlayer);
        }
    }

    private void OnVideoFinished(VideoPlayer vp)
    {
        if (isCinematicPlaying)
        {
            cinematicSequenceCoroutine = StartCoroutine(FinishSequence());
        }
    }

    private IEnumerator FinishSequence()
    {
        isCinematicPlaying = false;
        ScreenManager.isCinematicActive = false;

        if (videoOutputImage != null)
            videoOutputImage.enabled = false;

        TeleportPlayer();

        if (ScreenManager.Instance != null)
        {
            if (!ScreenManager.Instance.IsGamePaused())
            {
                ScreenManager.Instance.EngineResume();
                ScreenManager.Instance.ToggleHUD(true);
            }
        }

        yield return new WaitForSeconds(0.1f);

        OnCinematicEnd.Invoke();
    }

    private void TeleportPlayer()
    {
        if (playerObject == null || teleportDestination == null) return;

        CharacterController cc = playerObject.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;

        playerObject.transform.position = teleportDestination.position;
        playerObject.transform.rotation = teleportDestination.rotation;

        if (cc != null) cc.enabled = true;

        Debug.Log("Player teleported to: " + teleportDestination.name);
    }

    private void OnGamePaused(object data)
    {
        if (isCinematicPlaying && videoPlayer != null)
        {
            videoPlayer.Pause();

            if (cinematicSequenceCoroutine != null)
            {
                StopCoroutine(cinematicSequenceCoroutine);
            }
        }
    }

    private void OnGameResumed(object data)
    {
        if (isCinematicPlaying && videoPlayer != null)
        {
            videoPlayer.Play();

            cinematicSequenceCoroutine = StartCoroutine(PlayCinematicSequence());
        }
    }

    private void OnDestroy()
    {
        if (videoPlayer != null)
            videoPlayer.loopPointReached -= OnVideoFinished;

        UnsubscribeFromEvents();
    }
}