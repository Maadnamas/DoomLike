using UnityEngine;

public class SniperRifle : WeaponBase
{
    [Header("References")]
    public Camera fpsCamera;
    public Transform muzzlePoint;
    public ParticleSystem muzzleFlash;
    public GameObject impactPrefab;
    public float range = 1000f;
    public LayerMask hitMask = ~0;
    public float impactLife = 5f;

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
