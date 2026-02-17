using System.Linq;
using UnityEngine;

public class SniperRifle : WeaponBase
{
    [Header("References")]
    [SerializeField] ParticleSystem muzzleFlash;
    [SerializeField] GameObject impactPrefab;
    [SerializeField] float range = 1000f;
    [SerializeField] LayerMask hitMask = ~0;
    [SerializeField] float impactLife = 5f;
    [SerializeField] float zoomSpeed;
    [SerializeField] float zoomAmount;
    [SerializeField] GameObject uiContainer;
    [SerializeField] GameObject handCanvas;

    private GameObject mainUI;
    private GameObject sniperUI;
    private Vector3 originalPosition;

    public float ZoomSpeed => zoomSpeed;

    protected override void Awake()
    {
        base.Awake();

        if (fpsCamera != null && defaultFOV == 0)
        {
            defaultFOV = fpsCamera.fieldOfView;
        }

        originalPosition = handCanvas.transform.localPosition;
        sniperUI = uiContainer.GetComponentsInChildren<Transform>(includeInactive: true)
                         .FirstOrDefault(t => t.CompareTag("SniperUI"))?.gameObject;
        mainUI = uiContainer.GetComponentsInChildren<Transform>(includeInactive: true)
                 .FirstOrDefault(t => t.CompareTag("MainUI"))?.gameObject;
    }

    protected override void Shoot()
    {
        if (muzzleFlash != null) muzzleFlash.Play();
        if (shootSound) AudioManager.PlaySFX2D(shootSound);

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

        currentAmmo--;
    }

    public override void TryAim()
    {
        if (zoomCoroutine != null)
            StopCoroutine(zoomCoroutine);

        if (mainUI != null) mainUI.SetActive(false);

        handCanvas.transform.localPosition = new Vector3(originalPosition.x, originalPosition.y - Screen.height * 2, originalPosition.z);

        if (sniperUI != null) sniperUI.SetActive(true);
        else Debug.LogWarning("SniperUI could not be found");

        zoomCoroutine = StartCoroutine(ZoomRoutine(zoomAmount));
    }

    public override void StopAim()
    {
        if (zoomCoroutine != null)
        {
            StopCoroutine(zoomCoroutine);
            zoomCoroutine = null;
        }

        if (mainUI != null) mainUI.SetActive(true);
        handCanvas.transform.localPosition = originalPosition;
        if (sniperUI != null) sniperUI.SetActive(false);
    }

    private System.Collections.IEnumerator ZoomRoutine(float targetFOV)
    {
        float startFOV = fpsCamera.fieldOfView;
        float startTime = Time.time;
        float duration = Mathf.Abs(targetFOV - startFOV) / zoomSpeed;

        if (duration < 0.001f)
        {
            if (fpsCamera != null)
                fpsCamera.fieldOfView = targetFOV;
            yield break;
        }

        while (Time.time - startTime < duration)
        {
            float t = (Time.time - startTime) / duration;

            if (fpsCamera != null)
                fpsCamera.fieldOfView = Mathf.Lerp(startFOV, targetFOV, t);

            yield return null;
        }

        if (fpsCamera != null)
            fpsCamera.fieldOfView = targetFOV;
    }
}