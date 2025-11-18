using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class RocketProjectile : MonoBehaviour
{
    [SerializeField] float speed = 50f;
    [SerializeField] float spiralAmplitude = 0.2f;
    [SerializeField] float spiralFrequency = 10f;
    [SerializeField] float lifeTime = 5f;

    [SerializeField] float explosionRadius = 5f;
    [SerializeField] float explosionDamage = 50f;
    [SerializeField] GameObject explosionEffect;

    Rigidbody rb;
    float spawnTime;
    RocketFactory factory;
    float spiralTime;

    Transform owner;

    public void SetOwner(Transform o)
    {
        owner = o;
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        factory = FindObjectOfType<RocketFactory>();
    }

    public void ResetProjectile()
    {
        spawnTime = Time.time;
        spiralTime = 0f;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    void FixedUpdate()
    {
        if (!gameObject.activeInHierarchy) return;

        spiralTime += Time.fixedDeltaTime;
        Vector3 spiralOffset = transform.right * Mathf.Sin(spiralTime * spiralFrequency) * spiralAmplitude
                             + transform.up * Mathf.Cos(spiralTime * spiralFrequency) * spiralAmplitude;

        Vector3 moveDir = transform.forward * speed + spiralOffset * speed * 0.1f;
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
            {
                float dist = Vector3.Distance(hit.transform.position, transform.position);
                float t = Mathf.Clamp01(1f - dist / explosionRadius);
                float finalDamage = explosionDamage * t;

                if (finalDamage > 0.01f)
                    damageable.TakeDamage(finalDamage, transform.position, Vector3.up);
            }

            if (hit.attachedRigidbody)
                hit.attachedRigidbody.AddExplosionForce(500f, transform.position, explosionRadius);
        }

        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        factory.RecycleRocket(gameObject);
    }
}