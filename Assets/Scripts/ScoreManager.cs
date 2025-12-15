using UnityEngine;
using System.Collections;

public class ScoreManager : MonoBehaviour
{
    [Header("Configuración de Puntos (Sumar)")]
    [SerializeField] private int pointsPerEnemyKill = 100;
    [SerializeField] private int pointsPerEnemyHit = 10;
    [SerializeField] private int pointsPerAmmoPickup = 5;
    [SerializeField] private int pointsPerHeal = 20;
    [SerializeField] private int pointsPerCard = 200;

    [Header("Configuración de Puntos (Restar)")]
    [SerializeField] private int pointsPerShotFired = 1;
    [SerializeField] private int pointsPerDamageTaken = 15;

    [Header("Penalización por Tiempo")]
    [SerializeField] private int pointsTimePenalty = 1;
    [SerializeField] private float timePenaltyInterval = 1f;

    private int currentScore = 0;
    private bool isGameActive = true;

    private void Start()
    {
        currentScore = 0;
        UpdateGlobalScore();

        StartCoroutine(TimePenaltyRoutine());
    }

    private void OnEnable()
    {
        StartCoroutine(SubscribeEvents());
    }

    private void OnDisable()
    {
        UnsubscribeEvents();
    }

    private IEnumerator SubscribeEvents()
    {
        while (EventManager.Instance == null) yield return null;

        EventManager.StartListening(GameEvents.ENEMY_DIED, OnEnemyDied);
        EventManager.StartListening(GameEvents.ENEMY_DAMAGED, OnEnemyDamaged);
        EventManager.StartListening(GameEvents.AMMO_PICKED_UP, OnAmmoPickedUp);
        EventManager.StartListening(GameEvents.PLAYER_HEALED, OnPlayerHealed);
        EventManager.StartListening(GameEvents.CARD_COLLECTED, OnCardCollected);

        EventManager.StartListening(GameEvents.WEAPON_FIRED, OnWeaponFired);
        EventManager.StartListening(GameEvents.PLAYER_DAMAGED, OnPlayerDamaged);

        EventManager.StartListening(GameEvents.GAME_OVER, OnGameEnd);
        EventManager.StartListening(GameEvents.GAME_VICTORY, OnGameEnd);
    }

    private void UnsubscribeEvents()
    {
        if (EventManager.Instance == null) return;

        EventManager.StopListening(GameEvents.ENEMY_DIED, OnEnemyDied);
        EventManager.StopListening(GameEvents.ENEMY_DAMAGED, OnEnemyDamaged);
        EventManager.StopListening(GameEvents.AMMO_PICKED_UP, OnAmmoPickedUp);
        EventManager.StopListening(GameEvents.PLAYER_HEALED, OnPlayerHealed);
        EventManager.StopListening(GameEvents.CARD_COLLECTED, OnCardCollected);

        EventManager.StopListening(GameEvents.WEAPON_FIRED, OnWeaponFired);
        EventManager.StopListening(GameEvents.PLAYER_DAMAGED, OnPlayerDamaged);

        EventManager.StopListening(GameEvents.GAME_OVER, OnGameEnd);
        EventManager.StopListening(GameEvents.GAME_VICTORY, OnGameEnd);
    }

    private void AddScore(int amount)
    {
        if (!isGameActive) return;
        currentScore += amount;
        UpdateGlobalScore();
    }

    private void SubtractScore(int amount)
    {
        if (!isGameActive) return;
        currentScore -= amount;
        UpdateGlobalScore();
    }

    private void UpdateGlobalScore()
    {
        Debug.Log("Puntaje actual: " + currentScore);
        ScreenManager.Score = currentScore;
    }

    private IEnumerator TimePenaltyRoutine()
    {
        while (isGameActive)
        {
            yield return new WaitForSeconds(timePenaltyInterval);
            SubtractScore(pointsTimePenalty);
        }
    }

    private void OnEnemyDied(object data) => AddScore(pointsPerEnemyKill);
    private void OnEnemyDamaged(object data) => AddScore(pointsPerEnemyHit);
    private void OnAmmoPickedUp(object data) => AddScore(pointsPerAmmoPickup);
    private void OnPlayerHealed(object data) => AddScore(pointsPerHeal);
    private void OnCardCollected(object data) => AddScore(pointsPerCard);

    private void OnWeaponFired(object data) => SubtractScore(pointsPerShotFired);
    private void OnPlayerDamaged(object data) => SubtractScore(pointsPerDamageTaken);

    private void OnGameEnd(object data)
    {
        isGameActive = false;
    }
}