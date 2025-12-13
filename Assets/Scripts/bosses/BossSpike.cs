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
    public float sinkDuration = 0.5f;

    private Vector3 startPosition;
    private Vector3 targetPosition;

    public void Initialize(float spikeDamage)
    {
        damage = spikeDamage;
        startPosition = transform.position;
        targetPosition = transform.position;

        // Empezar enterrado
        transform.position = startPosition - Vector3.up * 2f;

        StartCoroutine(SpikeLifecycle());
    }

    private IEnumerator SpikeLifecycle()
    {
        // Fase 1: Salir del suelo
        float elapsed = 0f;
        Vector3 hiddenPos = startPosition - Vector3.up * 2f;

        while (elapsed < riseDuration)
        {
            transform.position = Vector3.Lerp(hiddenPos, targetPosition, elapsed / riseDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition;

        // Fase 2: Permanecer arriba
        yield return new WaitForSeconds(stayDuration);

        // Fase 3: Hundirse
        elapsed = 0f;
        while (elapsed < sinkDuration)
        {
            transform.position = Vector3.Lerp(targetPosition, hiddenPos, elapsed / sinkDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

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