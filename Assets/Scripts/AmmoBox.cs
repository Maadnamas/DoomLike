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
    {
        PlaySound();
        EventManager.TriggerEvent(GameEvents.AMMO_PICKED_UP, new AmmoEventData
        {
            weaponType = m_AmmoType.ToString(),
            amount = m_AmmoCount,
            totalAmmo = m_AmmoCount
        });

        Destroy(gameObject);
    }

    protected override void TryCollect(Collider other)
    {
        var player = other.GetComponentInChildren<IAmmo>();

        if (player != null && player.ReloadAmmo((int)m_AmmoType, m_AmmoCount))
            Collect();
    }
}