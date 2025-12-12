using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    private float currentHealth;
    public bool IsDead { get; private set; }
    private float maxHealth;

    [Header("Efectos")]
    [SerializeField] private ParticleSystem hitVFXPrefab;

    private IEnemyAnimator enemyAnimator;
    private CharacterController characterController;
    private Collider col;
    private Rigidbody rb;

    private void Awake()
    {
        enemyAnimator = GetComponent<IEnemyAnimator>();
        characterController = GetComponent<CharacterController>();
        col = GetComponent<Collider>();
        rb = GetComponent<Rigidbody>();
    }

    public void Initialize(float maxHealthValue)
    {
        maxHealth = maxHealthValue;
        currentHealth = maxHealth;
        IsDead = false;

        if (characterController != null) characterController.enabled = true;
        if (col != null) col.enabled = true;
        if (rb != null) rb.isKinematic = true;
    }

    public bool TakeDamage(float amount, Vector3 hitPoint, Vector3 hitNormal)
    {
        if (IsDead) return false;

        currentHealth -= amount;

        if (hitVFXPrefab != null)
        {
 
            ParticleSystem effect = Instantiate(hitVFXPrefab, transform.position + Vector3.up * 2f, transform.rotation);
            Destroy(effect.gameObject, effect.main.duration);
        }



        if (currentHealth <= 0f)
        {
            Die();
            return true;
        }

        enemyAnimator?.PlayHit();

        return false;
    }

    public void Heal(float amount) { }

    public void Die()
    {
        if (IsDead) return;

        IsDead = true;

        EnemyAI enemyAI = GetComponent<EnemyAI>();
        if (enemyAI != null && enemyAI.player != null)
        {
            Collider[] playerColliders = enemyAI.player.GetComponentsInChildren<Collider>();

            foreach (Collider pCol in playerColliders)
            {
                if (characterController != null)
                    Physics.IgnoreCollision(characterController, pCol, true);

                if (col != null && col != characterController)
                    Physics.IgnoreCollision(col, pCol, true);
            }
        }

        enemyAnimator?.PlayDeath();
        StartCoroutine(DeathSequence());
    }

    private System.Collections.IEnumerator DeathSequence()
    {
        yield return new WaitForSeconds(15f);

        if (characterController != null) characterController.enabled = false;
        if (col != null) col.enabled = false;
        if (rb != null) rb.isKinematic = true;

        float duration = 5f;
        float elapsed = 0f;

        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + Vector3.down * 2f;

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