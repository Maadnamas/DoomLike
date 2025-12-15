using UnityEngine;

public struct WeaponEventData
{
    public string weaponName;
    public int ammoRemaining;
}

public struct AmmoEventData
{
    public string weaponType;
    public int amount;
    public int totalAmmo;
}

public struct WeaponSwitchEventData
{
    public string weaponName;
    public int ammoCount;
}

public struct DamageEventData
{
    public int damageAmount;
    public int currentHealth;
    public int maxHealth;
}

public struct HealEventData
{
    public int healAmount;
    public int currentHealth;
}

public struct MedkitEventData
{
    public int currentMedkits;
    public int maxMedkits;
}

public struct EnemyDeathEventData
{
    public string enemyName;
    public Vector3 position;
}

public struct EnemyDamagedEventData
{
    public float damageTaken;
    public Vector3 hitPoint;
}

public struct CardEventData
{
    public string cardID;
    public bool isFoil;
}