using UnityEngine;

public class RocketLauncher : WeaponBase
{
    [Header("References")]

    [SerializeField] protected GameObject rocketPrefab;
    [SerializeField] protected float rocketSpeed = 50f;

    protected override void Shoot()
    {
        if (audioSource && shootSound)
            audioSource.PlayOneShot(shootSound);

        if (rocketPrefab && muzzlePoint)
        {

            Vector3 shootDir = fpsCamera ? fpsCamera.transform.forward : muzzlePoint.forward;

            GameObject rocket = Instantiate(rocketPrefab, muzzlePoint.position, Quaternion.LookRotation(shootDir));
            Rigidbody rb = rocket.GetComponent<Rigidbody>();
            if (rb) rb.velocity = shootDir * rocketSpeed;
        }

        currentAmmo --;
    }
}
