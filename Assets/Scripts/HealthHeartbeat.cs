using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HealthHeartbeat : MonoBehaviour
{
    [SerializeField] private Image healthImage;
    [SerializeField] private float maxScale = 1.15f;
    [SerializeField] private float minScale = 1f;
    [SerializeField] private float minBPM = 40f;
    [SerializeField] private float maxBPM = 160f;
    [SerializeField] private float pulseDuration = 0.2f;
    [SerializeField] private float tiltAngle = 10f;
    [SerializeField] private float tiltSpeed = 3f;

    private float currentHealthPercent = 1f;
    private Coroutine heartbeatRoutine;

    void OnEnable()
    {
        StartCoroutine(SubscribeWithDelay());
        if (heartbeatRoutine == null)
            heartbeatRoutine = StartCoroutine(HeartbeatLoop());
    }

    void OnDisable()
    {
        if (EventManager.Instance != null)
            EventManager.StopListening(GameEvents.UI_UPDATE_HEALTH, OnHealthUpdated);
        if (heartbeatRoutine != null)
            StopCoroutine(heartbeatRoutine);
        heartbeatRoutine = null;
    }

    IEnumerator SubscribeWithDelay()
    {
        while (EventManager.Instance == null)
            yield return null;
        EventManager.StartListening(GameEvents.UI_UPDATE_HEALTH, OnHealthUpdated);
    }

    void OnHealthUpdated(object data)
    {
        if (data is DamageEventData h)
            currentHealthPercent = Mathf.Clamp01((float)h.currentHealth / h.maxHealth);
    }

    IEnumerator HeartbeatLoop()
    {
        float time = 0;
        while (true)
        {
            float bpm = Mathf.Lerp(minBPM, maxBPM, 1f - currentHealthPercent);
            float pulseInterval = 60f / bpm;
            float scalePulse = Mathf.PingPong(Time.unscaledTime * (1f / pulseDuration) * (bpm / 60f), 1f);
            float s = Mathf.Lerp(minScale, maxScale, scalePulse);
            transform.localScale = Vector3.one * s;

            time += Time.unscaledDeltaTime * tiltSpeed;
            float angle = Mathf.Sin(time) * tiltAngle;
            transform.rotation = Quaternion.Euler(0, 0, angle);

            yield return null;
        }
    }
}