using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    private float speed;
    private float damage;

    public void Initialize(float _speed, float _damage)
    {
        speed = _speed;
        damage = _damage;
        Destroy(gameObject, 5f); // Se autodestruye a los 5s si no choca
    }

    private void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Ignorar colisión con otros enemigos o proyectiles
        if (other.CompareTag("Enemy") || other.CompareTag("Projectile")) return;

        // Si choca con el jugador
        if (other.CompareTag("Player"))
        {
            IDamageable damageable = other.GetComponent<IDamageable>();
            if (damageable != null)
            {
                // Vector de impacto simple (hacia atrás de la bala)
                damageable.TakeDamage(damage, transform.position, -transform.forward);
            }
        }

        // Destruir la bala al chocar con Player o Paredes
        Destroy(gameObject);
    }
}