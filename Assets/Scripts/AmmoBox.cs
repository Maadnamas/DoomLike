using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

enum AmmoType
{
    Pistol,
    Rocket,
    Sniper
}

public class AmmoBox : MonoBehaviour, ICollectable
{
    [SerializeField] private AmmoType m_AmmoType;
    [SerializeField] private int m_AmmoCount;
    public void Collect()
    {
        Destroy(gameObject);
    }

    public void OnTriggerEnter (Collider other)
    {
        WeaponManager pj = other.gameObject.GetComponentInChildren<WeaponManager>();
        if (pj != null)
        {
            if (pj.ReloadAmmo(((int)m_AmmoType+1), m_AmmoCount))
            {
                Collect();
            }

        }
    }
}
