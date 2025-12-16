using UnityEngine;

public class EnemyAudioBridge : MonoBehaviour
{
    private EnemyAI enemyAI;

    private void Awake()
    {
        // Busca el componente EnemyAI en el padre, o en el objeto principal
        enemyAI = GetComponentInParent<EnemyAI>();
        if (enemyAI == null)
        {
            Debug.LogError("EnemyAudioBridge requiere un componente EnemyAI en un padre.");
        }
    }



    // Esta función es llamada por el Evento de Animación
    public void CallPlayWalkSound()
    {
        if (enemyAI != null)
        {
            enemyAI.PlayWalkSound();
        }
    }
}