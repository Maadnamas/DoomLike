using UnityEngine;

enum AmmoType
{
    Pistol = 1,
    Rocket = 2,
    Sniper = 3
}

public class AmmoBox : CollectableObject
{
    [SerializeField] AmmoType m_AmmoType;
    [SerializeField] int m_AmmoCount;

    public override void Collect()
    {                                                                                                                //uli participó
        PlaySound();                                                                                                 //uli participó
        EventManager.TriggerEvent(GameEvents.AMMO_PICKED_UP, new AmmoEventData                                       //uli participó
        {                                                                                                            //uli participó
            weaponType = m_AmmoType.ToString(),
            amount = m_AmmoCount,
            totalAmmo = m_AmmoCount
        });

        Destroy(gameObject);
    }

    protected override void TryCollect(Collider other)
    {
        var pj = other.GetComponentInChildren<IAmmo>();

        if (pj != null && pj.ReloadAmmo((int)m_AmmoType, m_AmmoCount))
            Collect();
    }
}
