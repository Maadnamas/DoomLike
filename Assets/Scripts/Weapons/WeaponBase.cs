using UnityEngine;

public abstract class WeaponBase : MonoBehaviour
{
    [Header("Weapon Settings")]
    public string weaponName = "Unnamed Weapon";
    public int weaponID = 00;
    public Sprite weaponSprite;
    [SerializeField] protected Transform muzzlePoint;
    [SerializeField] protected float damage = 10f;
    [SerializeField] protected float fireRate = 5f;
    [SerializeField] protected AudioSource audioSource;
    [SerializeField] protected AudioClip shootSound;
    [SerializeField] protected Camera fpsCamera;
    [SerializeField] public int MaxAmmo;
    public int currentAmmo;
    public System.Action OnShoot;

    protected float nextTimeToFire;

    protected virtual void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        currentAmmo = MaxAmmo;
    }

    public virtual void TryShoot()
    {
        if (Time.time >= nextTimeToFire && currentAmmo > 0)
        {
            nextTimeToFire = Time.time + 1f / fireRate;
            Shoot();
            OnShoot?.Invoke();

            // EVENTO AGREGADO
            EventManager.TriggerEvent(GameEvents.WEAPON_FIRED, new WeaponEventData
            {
                weaponName = weaponName,
                ammoRemaining = currentAmmo
            });
        }
    }

    protected abstract void Shoot();

    public virtual void SetActive(bool active)
    {
        gameObject.SetActive(active);
    }

    public virtual void AddAmmo(int ammoAmount)
    {
        currentAmmo += ammoAmount;
        if (currentAmmo > MaxAmmo)
        {
            currentAmmo = MaxAmmo;
        }

        // EVENTO AGREGADO
        EventManager.TriggerEvent(GameEvents.AMMO_PICKED_UP, new AmmoEventData
        {
            weaponType = weaponName,
            amount = ammoAmount,
            totalAmmo = currentAmmo
        });
    }
}