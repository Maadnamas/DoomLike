using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WeaponPickup : MonoBehaviour
{
    [Header("Weapon Settings")]
    public GameObject weaponPrefab;
    public int ammoToAdd = 30;
    public float respawnTime = 10f;
    public bool destroyOnPickup = false;

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
        if (weaponManager == null)
        {
            Debug.LogError("No se encontró WeaponManager en el jugador");
            return;
        }

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

    WeaponBase GetExistingWeapon(WeaponManager weaponManager)
    {
        if (weaponPrefab == null) return null;

        WeaponBase prefabWeaponComponent = weaponPrefab.GetComponent<WeaponBase>();
        if (prefabWeaponComponent == null) return null;

        foreach (WeaponBase weapon in weaponManager.weapons)
        {
            if (weapon != null && weapon.weaponID == prefabWeaponComponent.weaponID)
            {
                return weapon;
            }
        }
        return null;
    }

    int FindEmptySlot(WeaponManager weaponManager)
    {
        for (int i = 0; i < weaponManager.weapons.Length; i++)
        {
            if (weaponManager.weapons[i] == null)
            {
                return i;
            }
        }
        return -1;
    }

    void SortWeaponArray(WeaponManager weaponManager)
    {
        int nonNullIndex = 0;
        for (int i = 0; i < weaponManager.weapons.Length; i++)
        {
            if (weaponManager.weapons[i] != null)
            {
                if (i != nonNullIndex)
                {
                    weaponManager.weapons[nonNullIndex] = weaponManager.weapons[i];
                    weaponManager.weapons[i] = null;
                }
                nonNullIndex++;
            }
        }

        System.Array.Sort(weaponManager.weapons, (a, b) =>
        {
            if (a == null && b == null) return 0;
            if (a == null) return 1;
            if (b == null) return -1;
            return a.weaponID.CompareTo(b.weaponID);
        });
    }

    void PlayPickupEffects()
    {
        if (pickupEffect != null)
        {
            Instantiate(pickupEffect, transform.position, Quaternion.identity);
        }

        if (pickupSound != null)
        {
            AudioSource.PlayClipAtPoint(pickupSound, transform.position);
        }
    }

    IEnumerator HandlePickup()
    {
        isCollected = true;

        if (meshRenderer != null) meshRenderer.enabled = false;
        if (pickupCollider != null) pickupCollider.enabled = false;

        if (destroyOnPickup)
        {
            Destroy(gameObject);
            yield break;
        }

        if (respawnTime > 0)
        {
            yield return new WaitForSeconds(respawnTime);

            isCollected = false;
            if (meshRenderer != null) meshRenderer.enabled = true;
            if (pickupCollider != null) pickupCollider.enabled = true;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }
}