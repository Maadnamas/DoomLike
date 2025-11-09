public interface IDamageable
{
    bool TakeDamage(float amount, UnityEngine.Vector3 hitPoint, UnityEngine.Vector3 hitNormal);

    void Heal(float amount);

    void Die();
}
