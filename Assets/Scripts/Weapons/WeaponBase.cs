using UnityEngine;

public abstract class WeaponBase : MonoBehaviour
{
    [Header("General Stats")]
    public string weaponName = "Unnamed Weapon";
    public Sprite weaponSprite;
    [SerializeField] protected Transform muzzlePoint;
    [SerializeField] protected float damage = 10f;
    [SerializeField] protected float fireRate = 5f;
    [SerializeField] protected AudioSource audioSource;
    [SerializeField] protected AudioClip shootSound;
    [SerializeField] protected Camera fpsCamera;
    [SerializeField] protected Animator animator;

    protected float nextTimeToFire;

    protected virtual void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>(); 
    }

    public virtual void TryShoot()
    {
        if (Time.time >= nextTimeToFire)
        {
            nextTimeToFire = Time.time + 1f / fireRate;
            Shoot();

            if (animator)
            {
                animator.SetTrigger("Shoot");
            }
        }
    }

    protected abstract void Shoot();

    public virtual void SetActive(bool active)
    {
        gameObject.SetActive(active);
    }
}
