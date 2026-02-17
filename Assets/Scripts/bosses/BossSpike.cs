using UnityEngine;
using System.Collections;

public class BossSpike : MonoBehaviour
{
    private float damage;
    private bool hasDealtDamage = false;

    [Header("Configuration")]
    public float riseSpeed = 5f;
    public float riseDuration = 0.3f;
    public float stayDuration = 2f;
    public float sinkDuration = 0.5f;

    private Vector3 startPosition;
    private Vector3 targetPosition;
    private Vector3 originalTargetPosition;

    public void Initialize(float spikeDamage, bool isPlayerSpike = false, float extraDownOffset = 0f)
    {
        damage = spikeDamage;
        startPosition = transform.position;

        // For player spike, raise it a bit for the "flying effect"
        if (isPlayerSpike)
        {
            targetPosition = startPosition + Vector3.up * 0.3f;
            originalTargetPosition = startPosition;
        }
        else
        {
            targetPosition = startPosition;
        }

        // Start buried
        float depth = 4f + extraDownOffset;
        transform.position = startPosition - Vector3.up * depth;

        StartCoroutine(SpikeLifecycle(isPlayerSpike));
    }

    private IEnumerator SpikeLifecycle(bool isPlayerSpike = false)
    {
        // Phase 1: Emerge from ground
        float elapsed = 0f;
        Vector3 hiddenPos = transform.position;

        while (elapsed < riseDuration)
        {
            transform.position = Vector3.Lerp(hiddenPos, targetPosition, elapsed / riseDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition;

        // Phase 2: Stay up
        yield return new WaitForSeconds(stayDuration);

        // Phase 3: Sink
        elapsed = 0f;
        float sinkExtraDepth = 6f;

        Vector3 sinkStartPos = transform.position;
        Vector3 finalSinkPos;

        if (isPlayerSpike)
        {
            finalSinkPos = originalTargetPosition - Vector3.up * sinkExtraDepth;
        }
        else
        {
            finalSinkPos = targetPosition - Vector3.up * sinkExtraDepth;
        }

        while (elapsed < sinkDuration)
        {
            float t = elapsed / sinkDuration;
            // Ease-out
            t = 1f - Mathf.Pow(1f - t, 3f);

            transform.position = Vector3.Lerp(sinkStartPos, finalSinkPos, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = finalSinkPos;

        yield return new WaitForSeconds(0.1f);

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
                Debug.Log("Spike hit the player");
            }
        }
    }
}