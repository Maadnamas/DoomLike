using UnityEngine;
using System.Collections;

public class Hand : WeaponBase
{
    [Header("Attack Settings")]
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float attackForce = 10f;
    [SerializeField] private LayerMask hitMask = ~0;
    [SerializeField] private GameObject hitImpactPrefab;
    [SerializeField] private float hitImpactLife = 1f;
    [SerializeField] private float punchCooldown = 0.3f;
    private bool isPunching;

    protected override void Shoot()
    {
        if (isPunching) return;
        StartCoroutine(PunchRoutine());
    }

    private IEnumerator PunchRoutine()
    {
        isPunching = true;

        if (animator)
            animator.SetTrigger("Shoot");

        if (audioSource && shootSound)
            audioSource.PlayOneShot(shootSound);

        yield return new WaitForSeconds(0.1f);

        if (fpsCamera)
        {
            Vector3 origin = fpsCamera.transform.position;
            Vector3 dir = fpsCamera.transform.forward;

            RaycastHit hit;
            if (Physics.Raycast(origin, dir, out hit, attackRange, hitMask, QueryTriggerInteraction.Ignore))
            {

                IDamageable dmg = hit.collider.GetComponentInParent<IDamageable>();
                if (dmg != null)
                {
                    dmg.TakeDamage(damage, hit.point, hit.normal);
                }

                if (hitImpactPrefab)
                {
                    GameObject fx = Instantiate(hitImpactPrefab, hit.point + hit.normal * 0.02f, Quaternion.LookRotation(hit.normal));
                    Destroy(fx, hitImpactLife);
                }

                if (hit.rigidbody)
                    hit.rigidbody.AddForce(dir * attackForce, ForceMode.Impulse);
            }
        }

        yield return new WaitForSeconds(punchCooldown);
        isPunching = false;
    }
}