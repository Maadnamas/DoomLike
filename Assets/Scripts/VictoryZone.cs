using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VictoryZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Jugador llegó al punto de victoria!");
            EventManager.TriggerEvent(GameEvents.GAME_VICTORY, null);
        }
    }
}