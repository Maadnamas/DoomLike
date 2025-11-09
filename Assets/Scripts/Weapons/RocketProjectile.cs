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
    private RocketFactory factory;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        factory = FindObjectOfType<RocketFactory>();
    }

    public void ResetProjectile()
    {
        startPosition = transform.position;
        spawnTime = Time.time;
        rb.velocity = transform.forward * speed;
    }

    void FixedUpdate()
    {
        float time = Time.time - spawnTime;
        Vector3 spiralOffset = transform.right * Mathf.Sin(time * spiralFrequency) * spiralAmplitude
                             + transform.up * Mathf.Cos(time * spiralFrequency) * spiralAmplitude;

        Vector3 moveDir = (transform.forward * speed + spiralOffset * speed * 0.1f);
        rb.velocity = moveDir;

        if (Time.time - spawnTime > lifeTime)
            Explode();
    }

    void OnCollisionEnter(Collision collision)
    {
        Explode();
    }

    void Explode()
    {
        if (explosionEffect)
        {
            GameObject effect = Instantiate(explosionEffect, transform.position, Quaternion.identity);
            var ps = effect.GetComponent<ParticleSystem>();
            Destroy(effect, ps != null ? ps.main.duration : 2f);
        }

        Collider[] hits = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider hit in hits)
        {
            var damageable = hit.GetComponent<IDamageable>();
            if (damageable != null)
                damageable.TakeDamage(explosionDamage, transform.position, Vector3.up);

            if (hit.attachedRigidbody)
                hit.attachedRigidbody.AddExplosionForce(500f, transform.position, explosionRadius);
        }

        rb.velocity = Vector3.zero;
        factory.RecycleRocket(gameObject);
    }
}