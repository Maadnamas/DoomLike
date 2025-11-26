using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    private float currentHealth;
    public bool IsDead { get; private set; }
    private float maxHealth;

    private IEnemyAnimator enemyAnimator;

    private void Awake()
    {
        enemyAnimator = GetComponent<IEnemyAnimator>();
    }

    public void Initialize(float maxHealthValue)
    {
        maxHealth = maxHealthValue;
        currentHealth = maxHealth;
        IsDead = false;
    }

    public bool TakeDamage(float amount, Vector3 hitPoint, Vector3 hitNormal)
    {
        if (IsDead) return false;

        currentHealth -= amount;

        enemyAnimator?.PlayHit();

        if (currentHealth <= 0f)
        {
            Die();
            return true;
        }

        return false;
    }

    public void Heal(float amount) { }

    public void Die()
    {
        if (IsDead) return;

        IsDead = true;

        enemyAnimator?.PlayDeath();

        StartCoroutine(DeathSequence());
    }

    private System.Collections.IEnumerator DeathSequence()
    {
        yield return new WaitForSeconds(15f);

        float duration = 5f;
        float elapsed = 0f;

        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + Vector3.down * 1f;

        while (elapsed < duration)
        {
            transform.position = Vector3.Lerp(startPos, endPos, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = endPos;
        Destroy(gameObject);
    }
}
