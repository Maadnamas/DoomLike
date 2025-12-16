using UnityEngine;
using System.Collections;

public class WeaponPickup : MonoBehaviour
{
    [Header("Weapon Settings")]
    public GameObject weaponPrefab;
    public int ammoToAdd = 30;
    public bool destroyOnPickup = false; // CAMBIA ESTO A FALSE POR DEFECTO

    [Header("Visual Feedback")]
    public GameObject pickupEffect;
    public AudioClip pickupSound;

    private bool isCollected = false;
    private MeshRenderer meshRenderer;
    private Collider pickupCollider;

    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        pickupCollider = GetComponent<Collider>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (isCollected || !other.CompareTag("Player")) return;

        WeaponManager weaponManager = other.GetComponentInChildren<WeaponManager>();
        if (weaponManager == null) return;

        PickupWeapon(weaponManager);
    }

    void PickupWeapon(WeaponManager weaponManager)
    {
        if (weaponPrefab == null) return;

        bool success = weaponManager.PickupWeapon(weaponPrefab, ammoToAdd);

        if (success)
        {
            PlayPickupEffects();
            StartCoroutine(HandlePickup());
        }
    }

    void PlayPickupEffects()
    {
        if (pickupEffect != null)
            Instantiate(pickupEffect, transform.position, Quaternion.identity);

        if (pickupSound != null)
            AudioSource.PlayClipAtPoint(pickupSound, transform.position);
    }

    IEnumerator HandlePickup()
    {
        isCollected = true;

        // Solo desactivar, NO destruir
        gameObject.SetActive(false);

        // Notificar al manager
        if (WeaponPickupManager.Instance != null)
            WeaponPickupManager.Instance.MarkPickupAsCollected(this);

        yield break;
    }

    // Método para reactivar el pickup
    public void ReactivatePickup()
    {
        isCollected = false;
        gameObject.SetActive(true);

        if (meshRenderer != null) meshRenderer.enabled = true;
        if (pickupCollider != null) pickupCollider.enabled = true;
    }

    public bool IsCollected()
    {
        return isCollected;
    }
}