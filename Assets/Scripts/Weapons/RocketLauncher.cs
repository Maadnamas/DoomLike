using UnityEngine;

public class RocketLauncher : WeaponBase
{
    [Header("References")]
    [SerializeField] protected RocketFactory rocketFactory;
    [SerializeField] protected float rocketSpeed = 50f;

    protected override void Shoot()
    {
        if (shootSound)
            AudioManager.PlaySFX2D(shootSound);

        if (rocketFactory && muzzlePoint)
        {
            Vector3 shootDir = fpsCamera ? fpsCamera.transform.forward : muzzlePoint.forward;

            GameObject rocket = rocketFactory.CreateRocket(muzzlePoint.position, Quaternion.LookRotation(shootDir));

            Rigidbody rb = rocket.GetComponent<Rigidbody>();
            if (rb)
                rb.velocity = shootDir * rocketSpeed;

            var rocketProj = rocket.GetComponent<RocketProjectile>();
            if (rocketProj != null)
                rocketProj.ResetProjectile();
        }

        currentAmmo--;
    }
}