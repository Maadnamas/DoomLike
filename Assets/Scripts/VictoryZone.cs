using UnityEngine;
using System.Collections;

public class VictoryZone : MonoBehaviour
{
    private bool bossIsDefeated = false;

    private void OnEnable()
    {
        StartCoroutine(SubscribeToBossEvent());
    }

    private void OnDisable()
    {
        UnsubscribeFromBossEvent();
    }

    private IEnumerator SubscribeToBossEvent()
    {
        while (EventManager.Instance == null)
        {
            yield return null;
        }

        EventManager.StartListening(GameEvents.BOSS_DIED, OnBossDefeated);
    }

    private void UnsubscribeFromBossEvent()
    {
        if (EventManager.Instance == null) return;
        EventManager.StopListening(GameEvents.BOSS_DIED, OnBossDefeated);
    }

    private void OnBossDefeated(object data)
    {
        bossIsDefeated = true;
        Debug.Log("Boss defeated. Victory zone is active.");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (bossIsDefeated)
            {
                Debug.Log("Player reached victory point and boss is defeated. VICTORY!");
                EventManager.TriggerEvent(GameEvents.GAME_VICTORY, null);
            }
            else
            {
                Debug.Log("Player attempted to pass, but the boss is still alive. You must kill the boss first!");
            }
        }
    }
}