using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class RocketProjectile : MonoBehaviour
{
    [Header("Rocket Settings")]
    [SerializeField] float speed = 50f;                
    [SerializeField] float spiralAmplitude = 0.2f;     
    [SerializeField] float spiralFrequency = 10f;      
    [SerializeField] float lifeTime = 5f;              

    [Header("Explosion Settings")]
    [SerializeField] float explosionRadius = 5f;       
    [SerializeField] float explosionDamage = 50f;      
    [SerializeField] GameObject explosionEffect;       

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
        {
            GameObject effect = Instantiate(explosionEffect, transform.position, Quaternion.identity);
            var ps = effect.GetComponent<ParticleSystem>();
            if (ps != null)
                Destroy(effect, ps.main.duration);
            else
                Destroy(effect, 2f); // por si acaso
        }

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
