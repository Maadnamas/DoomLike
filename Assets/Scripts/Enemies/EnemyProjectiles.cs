using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    private float speed;
    private float damage;

    public void Initialize(float _speed, float _damage)
    {
        speed = _speed;
        damage = _damage;
        Destroy(gameObject, 5f);
    }

    private void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy") || other.CompareTag("Projectile")) return;

        if (other.CompareTag("Player"))
        {
            IDamageable damageable = other.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(damage, transform.position, -transform.forward);
            }
        }

        Destroy(gameObject);
    }
}