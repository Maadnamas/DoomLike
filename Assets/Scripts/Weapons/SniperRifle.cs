using UnityEngine;

public class SniperRifle : WeaponBase
{
    [Header("References")]
    [SerializeField]  ParticleSystem muzzleFlash;
    [SerializeField]  GameObject impactPrefab;
    [SerializeField]  float range = 1000f;
    [SerializeField]  LayerMask hitMask = ~0;
    [SerializeField] float impactLife = 5f;

    protected override void Shoot()
    {
        if (muzzleFlash != null) muzzleFlash.Play();
        if (audioSource && shootSound) audioSource.PlayOneShot(shootSound);

        Vector3 origin = fpsCamera.transform.position;
        Vector3 dir = fpsCamera.transform.forward;

        if (Physics.Raycast(origin, dir, out RaycastHit hit, range, hitMask))
        {
            if (impactPrefab)
            {
                GameObject fx = Instantiate(impactPrefab, hit.point + hit.normal * 0.01f, Quaternion.LookRotation(hit.normal));
                Destroy(fx, impactLife);
            }

            IDamageable dmg = hit.collider.GetComponentInParent<IDamageable>();
            if (dmg != null)
                dmg.TakeDamage(damage, hit.point, hit.normal);
        }
    }
}
