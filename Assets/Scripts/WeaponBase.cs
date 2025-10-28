using UnityEngine;

public abstract class WeaponBase : MonoBehaviour
{
    [Header("General Stats")]
    public string weaponName = "Unnamed Weapon";
    public float damage = 10f;
    public float fireRate = 5f;
    public AudioSource audioSource;
    public AudioClip shootSound;

    protected float nextTimeToFire;

    /// <summary>
    /// Llamado por el WeaponManager cuando el jugador dispara.
    /// </summary>
    public virtual void TryShoot()
    {
        if (Time.time >= nextTimeToFire)
        {
            nextTimeToFire = Time.time + 1f / fireRate;
            Shoot();
        }
    }

    /// <summary>
    /// Implementado por cada tipo de arma.
    /// </summary>
    protected abstract void Shoot();

    /// <summary>
    /// Activa o desactiva el arma visualmente.
    /// </summary>
    public virtual void SetActive(bool active)
    {
        gameObject.SetActive(active);
    }
}
