using UnityEngine;

public abstract class WeaponBase : MonoBehaviour
{
    [Header("Weapon Settings")]
    public string weaponName = "Unnamed Weapon";
    public int weaponID = 00;
    public Sprite weaponSprite;

    [Header("HUD Icon")]
    public Sprite weaponIcon;

    [SerializeField] protected Transform muzzlePoint;
    [SerializeField] protected Camera fpsCamera;

    [SerializeField] protected float damage = 10f;
    [SerializeField] protected float fireRate = 5f;
    [SerializeField] protected AudioClip shootSound;

    [SerializeField] public int MaxAmmo;
    public int currentAmmo;
    public System.Action OnShoot;
    protected Coroutine zoomCoroutine;
    protected float defaultFOV;
    public float DefaultFOVValue => defaultFOV;

    protected float nextTimeToFire;

    public void SetCamera(Camera camera)
    {
        fpsCamera = camera;
        if (camera != null)
        {
            defaultFOV = camera.fieldOfView;
        }
    }

    public void SetMuzzlePoint(Transform muzzle)
    {
        muzzlePoint = muzzle;
    }

    protected virtual void Awake()
    {
        if (fpsCamera == null)
        {
            fpsCamera = Camera.main;
        }

        if (fpsCamera != null)
        {
            defaultFOV = fpsCamera.fieldOfView;
        }

        currentAmmo = MaxAmmo;

    }

    public virtual void TryShoot()
    {
        if (Time.time >= nextTimeToFire && currentAmmo > 0)
        {
            nextTimeToFire = Time.time + 1f / fireRate;
            Shoot();
            OnShoot?.Invoke();

            EventManager.TriggerEvent(GameEvents.WEAPON_FIRED, new WeaponEventData
            {
                weaponName = weaponName,
                ammoRemaining = currentAmmo
            });
        }
    }

    public virtual void TryAim() { }
    public virtual void StopAim() { }

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

        EventManager.TriggerEvent(GameEvents.AMMO_PICKED_UP, new AmmoEventData
        {
            weaponType = weaponName,
            amount = ammoAmount,
            totalAmmo = currentAmmo
        });
    }

    public Sprite GetWeaponIcon()
    {
        return weaponIcon != null ? weaponIcon : weaponSprite;
    }
}