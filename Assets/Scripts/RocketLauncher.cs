using UnityEngine;

public class RocketLauncher : WeaponBase
{
    [Header("References")]
    public Transform muzzlePoint;
    public GameObject rocketPrefab;
    public float rocketSpeed = 50f;

    protected override void Shoot()
    {
        if (audioSource && shootSound)
            audioSource.PlayOneShot(shootSound);

        if (rocketPrefab && muzzlePoint)
        {
            GameObject rocket = Instantiate(rocketPrefab, muzzlePoint.position, muzzlePoint.rotation);
            Rigidbody rb = rocket.GetComponent<Rigidbody>();
            if (rb) rb.velocity = muzzlePoint.forward * rocketSpeed;
        }
    }
}
