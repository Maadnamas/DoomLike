using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

enum AmmoType
{
    Pistol = 1,
    Rocket = 2,
    Sniper = 3
}

public class AmmoBox : MonoBehaviour, ICollectable
{
    [SerializeField] private AmmoType m_AmmoType;
    [SerializeField] private int m_AmmoCount;

    public void Collect()
    {
        // Disparar evento de recogida de munición
        EventManager.TriggerEvent(GameEvents.AMMO_PICKED_UP, new AmmoEventData
        {
            weaponType = m_AmmoType.ToString(),
            amount = m_AmmoCount,
            totalAmmo = m_AmmoCount
        });

        Destroy(gameObject);
    }

    public void OnTriggerEnter(Collider other)
    {
        WeaponManager pj = other.gameObject.GetComponentInChildren<WeaponManager>();
        if (pj != null)
        {
            if (pj.ReloadAmmo(((int)m_AmmoType), m_AmmoCount))
            {
                Collect();
            }
        }
    }
}