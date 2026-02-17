using UnityEngine;
using System.Collections;

public class Hand : WeaponBase
{
    [Header("Melee Settings")]
    [SerializeField] float meleeRange = 2f;
    [SerializeField] float meleeRadius = 0.4f;
    [SerializeField] LayerMask hitMask = ~0;
    [SerializeField] float impactForce = 5f;
    [SerializeField] GameObject impactPrefab;
    [SerializeField] float impactLife = 2f;

    [Header("Animation / Audio")]
    [SerializeField] Animator handAnimator;
    [SerializeField] string animationTrigger = "Shoot";

    public override void TryShoot()
    {
        if (Time.time >= nextTimeToFire)
        {
            nextTimeToFire = Time.time + 1f / fireRate;
            Shoot();
            OnShoot?.Invoke();
        }
    }

    protected override void Shoot()
    {
        if (shootSound != null)
        {
            AudioManager.PlaySFX2D(shootSound);
        }

        if (handAnimator != null && !string.IsNullOrEmpty(animationTrigger))
        {
            handAnimator.SetTrigger(animationTrigger);
        }

        Vector3 origin = fpsCamera.transform.position;
        Vector3 dir = fpsCamera.transform.forward;

        RaycastHit hit;

        if (Physics.SphereCast(origin, meleeRadius, dir, out hit, meleeRange, hitMask, QueryTriggerInteraction.Ignore))
        {
            IDamageable dmg = hit.collider.GetComponentInParent<IDamageable>();
            if (dmg != null)
            {
                dmg.TakeDamage(damage, hit.point, hit.normal);
            }
            else
            {
                if (impactPrefab)
                {
                    GameObject fx = Instantiate(impactPrefab, hit.point + hit.normal * 0.01f, Quaternion.LookRotation(hit.normal));
                    Destroy(fx, impactLife);
                }
            }

            if (hit.rigidbody != null)
            {
                hit.rigidbody.AddForceAtPosition(dir * impactForce, hit.point, ForceMode.Impulse);
            }

            Debug.DrawLine(origin, hit.point, Color.red, 0.5f);
        }
        else
        {
            Debug.DrawRay(origin, dir * meleeRange, Color.yellow, 0.5f);
        }
    }
}