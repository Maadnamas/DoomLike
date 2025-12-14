using UnityEngine;
using System.Collections;

public class BossSpike : MonoBehaviour
{
    private float damage;
    private bool hasDealtDamage = false;

    [Header("Configuración")]
    public float riseSpeed = 5f;
    public float riseDuration = 0.3f;
    public float stayDuration = 2f;
    public float sinkDuration = 0.5f; // Aumentado para que baje más suavemente

    private Vector3 startPosition;
    private Vector3 targetPosition;
    private Vector3 originalTargetPosition; // Guardar posición original para el pincho del jugador

    public void Initialize(float spikeDamage, bool isPlayerSpike = false, float extraDownOffset = 0f)
    {
        damage = spikeDamage;
        startPosition = transform.position;

        // Para el pincho del jugador, subirlo un poco para el "efecto volando"
        if (isPlayerSpike)
        {
            targetPosition = startPosition + Vector3.up * 0.3f; // Un poco más alto
            originalTargetPosition = startPosition; // Guardamos la posición original
        }
        else
        {
            targetPosition = startPosition;
        }

        // Empezar enterrado - todos más profundos, y el del jugador aún más
        float depth = 4f + extraDownOffset; // 4f base + offset extra para el del jugador
        transform.position = startPosition - Vector3.up * depth;

        StartCoroutine(SpikeLifecycle(isPlayerSpike));
    }

    private IEnumerator SpikeLifecycle(bool isPlayerSpike = false)
    {
        // Fase 1: Salir del suelo
        float elapsed = 0f;
        Vector3 hiddenPos = transform.position; // Ya estamos en la posición enterrada

        while (elapsed < riseDuration)
        {
            transform.position = Vector3.Lerp(hiddenPos, targetPosition, elapsed / riseDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition;

        // Fase 2: Permanecer arriba
        yield return new WaitForSeconds(stayDuration);

        // Fase 3: Hundirse - ahora bajando mucho más
        elapsed = 0f;
        float sinkExtraDepth = 6f; // Aumentado para que bajen más al desaparecer

        // Para el pincho del jugador, bajamos desde su posición elevada
        Vector3 sinkStartPos = transform.position;
        Vector3 finalSinkPos;

        if (isPlayerSpike)
        {
            // Bajar a la posición original y luego más abajo
            finalSinkPos = originalTargetPosition - Vector3.up * sinkExtraDepth;
        }
        else
        {
            // Bajar mucho más desde la posición normal
            finalSinkPos = targetPosition - Vector3.up * sinkExtraDepth;
        }

        while (elapsed < sinkDuration)
        {
            float t = elapsed / sinkDuration;
            // Usar ease-out para que empiece rápido y termine lento
            t = 1f - Mathf.Pow(1f - t, 3f);

            transform.position = Vector3.Lerp(sinkStartPos, finalSinkPos, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = finalSinkPos;

        // Esperar un poco antes de destruir para que se vea que baja
        yield return new WaitForSeconds(0.1f);

        // Destruir
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasDealtDamage) return;

        if (other.CompareTag("Player"))
        {
            IDamageable damageable = other.GetComponent<IDamageable>();
            if (damageable != null)
            {
                Vector3 hitPoint = other.transform.position;
                Vector3 hitNormal = Vector3.up;
                damageable.TakeDamage(damage, hitPoint, hitNormal);
                hasDealtDamage = true;
                Debug.Log("Pincho golpeó al jugador");
            }
        }
    }
}