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
        // Espera a que el EventManager esté listo
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
        Debug.Log("Jefe derrotado. La zona de victoria está activa.");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (bossIsDefeated)
            {
                Debug.Log("Jugador llegó al punto de victoria y el jefe está derrotado. ¡VICTORIA!");
                EventManager.TriggerEvent(GameEvents.GAME_VICTORY, null);
            }
            else
            {
                Debug.Log("Jugador intentó pasar, pero el jefe aún está vivo. ¡Debes matar al jefe primero!");
                // Opcional: Podrías disparar un evento UI aquí para mostrar un mensaje al jugador.
            }
        }
    }
}