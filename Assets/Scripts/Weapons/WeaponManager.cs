using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WeaponManager : MonoBehaviour
{
    [Header("Weapons")]
    public WeaponBase[] weapons;
    public int currentIndex = 0;
    public int currentweaponammo = 0;

    [Header("UI")]
    public Image weaponImage;
    public Animator weaponUIAnimator;
    public float transitionSpeed = 400f;
    [SerializeField] float downamount = 200f;

    private RectTransform weaponImageRect;
    private Vector2 originalPos;
    private bool isTransitioning;

    void Start()
    {
        if (weaponImage)
        {
            weaponImageRect = weaponImage.GetComponent<RectTransform>();
            originalPos = weaponImageRect.anchoredPosition;
        }

        System.Array.Sort(weapons, (a, b) => a.weaponID.CompareTo(b.weaponID));

        SelectWeapon(0);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) SelectWeapon(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SelectWeapon(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) SelectWeapon(2);
        if (Input.GetKeyDown(KeyCode.Alpha4)) SelectWeapon(3);

        if (Input.GetButton("Fire1"))
        {
            weapons[currentIndex].TryShoot();
        }
    }

    void SelectWeapon(int index)
    {
        if (index < 0 || index >= weapons.Length || isTransitioning) return;

        for (int i = 0; i < weapons.Length; i++)
            weapons[i].SetActive(i == index);

        StartCoroutine(ChangeWeaponUI(index));

        currentIndex = index;
        currentweaponammo = weapons[currentIndex].currentAmmo;

        if (weaponUIAnimator)
        {
            weaponUIAnimator.SetInteger("weaponID", weapons[currentIndex].weaponID);
            weaponUIAnimator.SetTrigger("ChangeW");
        }

        weapons[currentIndex].OnShoot += () =>
        {
            if (weaponUIAnimator)
                weaponUIAnimator.SetTrigger("Shoot");

            // Disparar evento de munición cambiada
            TriggerAmmoChangedEvent();
        };

        // EVENTO DE CAMBIO DE ARMA
        EventManager.TriggerEvent(GameEvents.WEAPON_SWITCHED, new WeaponSwitchEventData
        {
            weaponName = weapons[currentIndex].weaponName,
            ammoCount = weapons[currentIndex].currentAmmo
        });

        // EVENTO DE ACTUALIZACIÓN DE MUNICIÓN
        TriggerAmmoChangedEvent();

        Debug.Log($"arma actual: {weapons[index].weaponName}");
    }

    IEnumerator ChangeWeaponUI(int newIndex)
    {
        isTransitioning = true;

        if (weaponImageRect)
        {
            while (weaponImageRect.anchoredPosition.y > originalPos.y - downamount)
            {
                weaponImageRect.anchoredPosition -= new Vector2(0, transitionSpeed * Time.deltaTime);
                yield return null;
            }

            if (weapons[newIndex].weaponSprite != null)
                weaponImage.sprite = weapons[newIndex].weaponSprite;

            while (weaponImageRect.anchoredPosition.y < originalPos.y)
            {
                weaponImageRect.anchoredPosition += new Vector2(0, transitionSpeed * Time.deltaTime);
                yield return null;
            }

            weaponImageRect.anchoredPosition = originalPos;
        }

        isTransitioning = false;
    }

    public bool ReloadAmmo(int ammoType, int ammoAmount)
    {
        if (weapons[ammoType].currentAmmo < weapons[ammoType].MaxAmmo)
        {
            weapons[ammoType].AddAmmo(ammoAmount);

            // Actualizar munición actual si es el arma equipada
            if (ammoType == currentIndex)
            {
                currentweaponammo = weapons[ammoType].currentAmmo;
            }

            // Disparar evento de recarga
            EventManager.TriggerEvent(GameEvents.AMMO_PICKED_UP, new AmmoEventData
            {
                weaponType = weapons[ammoType].weaponName,
                amount = ammoAmount,
                totalAmmo = weapons[ammoType].currentAmmo
            });

            // Disparar evento de cambio de munición
            TriggerAmmoChangedEvent();

            return true;
        }
        else return false;
    }

    private void TriggerAmmoChangedEvent()
    {
        // Disparar evento general de cambio de munición
        EventManager.TriggerEvent(GameEvents.AMMO_CHANGED, new AmmoEventData
        {
            weaponType = weapons[currentIndex].weaponName,
            amount = 0, // No específico para este evento
            totalAmmo = weapons[currentIndex].currentAmmo
        });

        // Disparar evento específico para UI
        EventManager.TriggerEvent(GameEvents.UI_UPDATE_AMMO, new AmmoEventData
        {
            weaponType = weapons[currentIndex].weaponName,
            amount = 0,
            totalAmmo = weapons[currentIndex].currentAmmo
        });
    }
}