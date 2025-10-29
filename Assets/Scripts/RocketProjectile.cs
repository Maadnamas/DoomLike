using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class RocketProjectile : MonoBehaviour
{
    [Header("Rocket Settings")]
    public float speed = 50f;                // Velocidad hacia adelante
    public float spiralAmplitude = 0.2f;     // Amplitud del movimiento circular
    public float spiralFrequency = 10f;      // Frecuencia del movimiento circular
    public float lifeTime = 5f;              // Tiempo antes de autodestruirse

    [Header("Explosion Settings")]
    public float explosionRadius = 5f;       // Radio de daño en área
    public float explosionDamage = 50f;      // Daño en área
    public GameObject explosionEffect;       // Partícula opcional al explotar

    private Rigidbody rb;
    private Vector3 startPosition;
    private float spawnTime;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        startPosition = transform.position;
        spawnTime = Time.time;
    }

    void FixedUpdate()
    {
        // Movimiento hacia adelante + patrón espiral
        float time = Time.time - spawnTime;
        Vector3 spiralOffset = transform.right * Mathf.Sin(time * spiralFrequency) * spiralAmplitude
                             + transform.up * Mathf.Cos(time * spiralFrequency) * spiralAmplitude;

        Vector3 moveDir = (transform.forward * speed + spiralOffset * speed * 0.1f);
        rb.velocity = moveDir;

        // Autodestrucción
        if (Time.time - spawnTime > lifeTime)
            Explode();
    }

    void OnCollisionEnter(Collision collision)
    {
        Explode();
    }

    void Explode()
    {
        // Instancia efecto visual
        if (explosionEffect)
            Instantiate(explosionEffect, transform.position, Quaternion.identity);

        // Busca objetos en el radio
        Collider[] hits = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (Collider hit in hits)
        {
            var damageable = hit.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(explosionDamage, transform.position, Vector3.up);
            }

            // Si tiene rigidbody, aplica empuje
            if (hit.attachedRigidbody)
            {
                hit.attachedRigidbody.AddExplosionForce(500f, transform.position, explosionRadius);
            }
        }

        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.5f, 0f, 0.3f);
        Gizmos.DrawSphere(transform.position, explosionRadius);
    }
}
