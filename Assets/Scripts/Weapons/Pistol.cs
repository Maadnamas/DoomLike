using UnityEngine;
using System.Collections;

public class Pistol : WeaponBase
{
    [Header("References")]
    [SerializeField] private ParticleSystem muzzleFlash;
    [SerializeField] private GameObject impactPrefab;
    [SerializeField] private LineRenderer tracerPrefab;

    [Header("Settings")]
    [SerializeField]  float range = 100f;
    [SerializeField]  LayerMask hitMask = ~0;
    [SerializeField]  float impactForce = 50f;
    [SerializeField]  float impactLife = 5f;
    [SerializeField]  float tracerDuration = 0.03f;

    protected override void Shoot()
    {
        // Efectos
        if (muzzleFlash != null) muzzleFlash.Play();
        if (shootSound) AudioManager.PlaySFX2D(shootSound);

        Vector3 origin = fpsCamera.transform.position;
        Vector3 dir = fpsCamera.transform.forward;

        RaycastHit hit;
        if (Physics.Raycast(origin, dir, out hit, range, hitMask, QueryTriggerInteraction.Ignore))
        {
            IDamageable dmg = hit.collider.GetComponentInParent<IDamageable>();
            if (dmg != null)
                dmg.TakeDamage(damage, hit.point, hit.normal);
            else
                        if (impactPrefab)
            {
                GameObject fx = Instantiate(impactPrefab, hit.point + hit.normal * 0.01f, Quaternion.LookRotation(hit.normal));
                Destroy(fx, impactLife);
            }



            if (hit.rigidbody != null)
                hit.rigidbody.AddForceAtPosition(dir * impactForce, hit.point, ForceMode.Impulse);

            if (tracerPrefab && muzzlePoint)
                StartCoroutine(SpawnTracer(muzzlePoint.position, hit.point));
        }
        else if (tracerPrefab && muzzlePoint)
        {
            StartCoroutine(SpawnTracer(muzzlePoint.position, origin + dir * range));
        }

        currentAmmo --;
    }

    IEnumerator SpawnTracer(Vector3 from, Vector3 to)
    {
        LineRenderer lr = Instantiate(tracerPrefab);
        lr.positionCount = 2;
        lr.SetPosition(0, from);
        lr.SetPosition(1, to);
        yield return new WaitForSeconds(tracerDuration);
        Destroy(lr.gameObject);
    }
}
