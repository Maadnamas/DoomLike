using UnityEngine;
using System.Collections;

public class hand : WeaponBase
{
    [Header("Melee Settings")]
    [SerializeField] float meleeRange = 2f;
    [SerializeField] float meleeRadius = 0.4f;
    [SerializeField] LayerMask hitMask = ~0;
    [SerializeField] float impactForce = 5f;
    [SerializeField] GameObject impactPrefab;
    [SerializeField] float impactLife = 2f;

    [Header("Animation / Audio")]
    [SerializeField] Animator handAnimator;          // animator del modelo de la mano / personaje
    [SerializeField] string animationTrigger = "Shoot"; // trigger que activa la animación ('Shoot' según dijiste)
    // audioSource y shootSound vienen del WeaponBase (si los asignas en el inspector)

    // IMPORTANTE: sobreescribimos TryShoot para no depender de munición
    public override void TryShoot()
    {
        if (Time.time >= nextTimeToFire) // respetar fireRate
        {
            nextTimeToFire = Time.time + 1f / fireRate;
            Shoot();
            OnShoot?.Invoke(); // notifica al WeaponManager (para UI, por ejemplo)
        }
    }

    protected override void Shoot()
    {
        // reproducir sonido de golpe si está disponible
        if (audioSource != null && shootSound != null)
        {
            audioSource.PlayOneShot(shootSound);
        }

        // activar animación local de la mano (el Animator que tengas en escena)
        if (handAnimator != null && !string.IsNullOrEmpty(animationTrigger))
        {
            handAnimator.SetTrigger(animationTrigger);
        }

        // Raycast / SphereCast para golpe cuerpo a cuerpo
        Vector3 origin = fpsCamera.transform.position;
        Vector3 dir = fpsCamera.transform.forward;

        RaycastHit hit;
        // usamos SphereCast para que sea más tolerante en precisión
        if (Physics.SphereCast(origin, meleeRadius, dir, out hit, meleeRange, hitMask, QueryTriggerInteraction.Ignore))
        {
            // dañar si tiene IDamageable en el padre o en los componentes
            IDamageable dmg = hit.collider.GetComponentInParent<IDamageable>();
            if (dmg != null)
            {
                dmg.TakeDamage(damage, hit.point, hit.normal);
            }
            else
            {
                // si no hay IDamageable, spawn de FX opcional
                if (impactPrefab)
                {
                    GameObject fx = Instantiate(impactPrefab, hit.point + hit.normal * 0.01f, Quaternion.LookRotation(hit.normal));
                    Destroy(fx, impactLife);
                }
            }

            // aplicar fuerza física si existe rigidbody
            if (hit.rigidbody != null)
            {
                hit.rigidbody.AddForceAtPosition(dir * impactForce, hit.point, ForceMode.Impulse);
            }

            // opcional: debug
            Debug.DrawLine(origin, hit.point, Color.red, 0.5f);
        }
        else
        {
            // golpe al vacío (puedes reproducir sonido o spawn de particulas de aire si quieres)
            Debug.DrawRay(origin, dir * meleeRange, Color.yellow, 0.5f);
        }

        // NOTA: no consumimos munición para la mano
    }
}