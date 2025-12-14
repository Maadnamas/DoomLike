using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WeaponManager : MonoBehaviour, IAmmo
{
    [Header("Weapons")]
    public WeaponBase[] weapons;
    public int currentIndex = 0;
    public int currentweaponammo = 0;

    [Header("Camera & References")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Transform defaultMuzzlePoint;

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

        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }

        if (weapons == null || weapons.Length == 0)
        {
            weapons = new WeaponBase[4];
        }

        AssignReferencesToExistingWeapons();

        bool foundWeapon = false;
        for (int i = 0; i < weapons.Length; i++)
        {
            if (weapons[i] != null)
            {
                weapons[i].SetActive(i == 0);
                if (i == 0)
                {
                    foundWeapon = true;
                    currentIndex = 0;
                    currentweaponammo = weapons[0].currentAmmo;

                    if (weapons[0].weaponSprite != null && weaponImage != null)
                    {
                        weaponImage.sprite = weapons[0].weaponSprite;
                    }
                }
            }
        }

        if (!foundWeapon)
        {
            Debug.LogWarning("No hay armas iniciales en el WeaponManager");
        }
    }

    void Update()
    {
        // Cambiar armas (solo si existe)
        if (Input.GetKeyDown(KeyCode.Alpha1)) TrySelectWeapon(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) TrySelectWeapon(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) TrySelectWeapon(2);
        if (Input.GetKeyDown(KeyCode.Alpha4)) TrySelectWeapon(3);

        // Disparar/aim solo si hay arma
        if (currentIndex < weapons.Length && weapons[currentIndex] != null)
        {
            if (Input.GetButton("Fire1"))
            {
                weapons[currentIndex].TryShoot();
            }
            if (Input.GetButtonDown("Fire2"))
            {
                weapons[currentIndex].TryAim();
            }
            if (Input.GetButtonUp("Fire2"))
            {
                weapons[currentIndex].StopAim();
            }
        }
    }

    void TrySelectWeapon(int index)
    {
        if (index < weapons.Length && weapons[index] != null)
        {
            SelectWeapon(index);
        }
        else
        {
            Debug.Log($"No hay arma en el slot {index}");
        }
    }

    void SelectWeapon(int index)
    {
        if (index < 0 || index >= weapons.Length || isTransitioning) return;
        if (weapons[index] == null) return;

        for (int i = 0; i < weapons.Length; i++)
        {
            if (weapons[i] != null)
                weapons[i].SetActive(i == index);
        }

        StartCoroutine(ChangeWeaponUI(index));

        currentIndex = index;
        currentweaponammo = weapons[currentIndex].currentAmmo;

        if (weaponUIAnimator)
        {
            weaponUIAnimator.SetInteger("weaponID", weapons[currentIndex].weaponID);
            weaponUIAnimator.SetTrigger("ChangeW");
        }

        weapons[currentIndex].OnShoot = null;
        weapons[currentIndex].OnShoot += () =>
        {
            if (weaponUIAnimator)
                weaponUIAnimator.SetTrigger("Shoot");

            currentweaponammo = weapons[currentIndex].currentAmmo;
            TriggerAmmoChangedEvent();
        };

        EventManager.TriggerEvent(GameEvents.WEAPON_SWITCHED, new WeaponSwitchEventData
        {
            weaponName = weapons[currentIndex].weaponName,
            ammoCount = weapons[currentIndex].currentAmmo
        });

        TriggerAmmoChangedEvent();

        Debug.Log($"Arma actual: {weapons[index].weaponName}");
    }

    IEnumerator ChangeWeaponUI(int newIndex)
    {
        if (weapons[newIndex] == null) yield break;

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

    void AssignReferencesToExistingWeapons()
    {
        foreach (WeaponBase weapon in weapons)
        {
            if (weapon != null)
            {
                AssignReferencesToWeapon(weapon);
            }
        }
    }

    void AssignReferencesToWeapon(WeaponBase weapon)
    {

        if (playerCamera != null)
        {
            weapon.SetCamera(playerCamera);
        }

        if (defaultMuzzlePoint != null)
        {
            weapon.SetMuzzlePoint(defaultMuzzlePoint);
        }
    }

    // ===== RECOLECCION =====

    public bool AddWeapon(WeaponBase weaponPrefab, int ammoToAdd = 0)
    {
        if (weaponPrefab == null) return false;

        WeaponBase existingWeapon = GetWeaponByID(weaponPrefab.weaponID);
        if (existingWeapon != null)
        {
            existingWeapon.AddAmmo(ammoToAdd);
            Debug.Log($"Munición añadida a {existingWeapon.weaponName}: +{ammoToAdd}");
            return true;
        }

        int emptySlot = -1;
        for (int i = 0; i < weapons.Length; i++)
        {
            if (weapons[i] == null)
            {
                emptySlot = i;
                break;
            }
        }

        if (emptySlot == -1)
        {
            Debug.LogWarning("Inventario lleno");
            return false;
        }

        GameObject newWeaponObj = Instantiate(weaponPrefab.gameObject, transform);
        WeaponBase newWeapon = newWeaponObj.GetComponent<WeaponBase>();

        if (newWeapon == null)
        {
            Destroy(newWeaponObj);
            return false;
        }

        AssignReferencesToWeapon(newWeapon);

        newWeaponObj.name = weaponPrefab.name;
        newWeaponObj.SetActive(false);
        weapons[emptySlot] = newWeapon;

        SortWeaponArray();

        if (ammoToAdd > 0)
            newWeapon.AddAmmo(ammoToAdd);

        if (currentIndex >= weapons.Length || weapons[currentIndex] == null)
        {
           for (int i = 0; i < weapons.Length; i++)
          {
               if (weapons[i] != null)
               {
                   TrySelectWeapon(i);
                   break;
              }
            }
        }


        Debug.Log($"Arma añadida: {newWeapon.weaponName}");
        return true;
    }

    WeaponBase GetWeaponByID(int weaponID)
    {
        foreach (WeaponBase weapon in weapons)
        {
            if (weapon != null && weapon.weaponID == weaponID)
                return weapon;
        }
        return null;
    }

    void SortWeaponArray()
    {
        int writeIndex = 0;
        for (int readIndex = 0; readIndex < weapons.Length; readIndex++)
        {
            if (weapons[readIndex] != null)
            {
                if (readIndex != writeIndex)
                {
                    weapons[writeIndex] = weapons[readIndex];
                    weapons[readIndex] = null;
                }
                writeIndex++;
            }
        }

        // ordena
        System.Array.Sort(weapons, (a, b) =>
        {
            if (a == null && b == null) return 0;
            if (a == null) return 1;
            if (b == null) return -1;
            return a.weaponID.CompareTo(b.weaponID);
        });
    }

    // ===== MUNICIÓN =====

    public bool ReloadAmmo(int ammoType, int ammoAmount)
    {
        for (int i = 0; i < weapons.Length; i++)
        {
            if (weapons[i] != null && weapons[i].weaponID == ammoType)
            {
                if (weapons[i].currentAmmo < weapons[i].MaxAmmo)
                {
                    weapons[i].AddAmmo(ammoAmount);

                    if (i == currentIndex)
                    {
                        currentweaponammo = weapons[i].currentAmmo;
                    }

                    EventManager.TriggerEvent(GameEvents.AMMO_PICKED_UP, new AmmoEventData
                    {
                        weaponType = weapons[i].weaponName,
                        amount = ammoAmount,
                        totalAmmo = weapons[i].currentAmmo
                    });

                    TriggerAmmoChangedEvent();
                    return true;
                }
                return false;
            }
        }
        return false;
    }

    void TriggerAmmoChangedEvent()
    {
        if (weapons[currentIndex] == null) return;

        EventManager.TriggerEvent(GameEvents.AMMO_CHANGED, new AmmoEventData
        {
            weaponType = weapons[currentIndex].weaponName,
            amount = 0,
            totalAmmo = weapons[currentIndex].currentAmmo
        });

        EventManager.TriggerEvent(GameEvents.UI_UPDATE_AMMO, new AmmoEventData
        {
            weaponType = weapons[currentIndex].weaponName,
            amount = 0,
            totalAmmo = weapons[currentIndex].currentAmmo
        });
    }

    // ===== PICKUP =====

    public bool PickupWeapon(GameObject weaponPrefab, int ammoToAdd)
    {
        if (weaponPrefab == null) return false;

        WeaponBase weaponComponent = weaponPrefab.GetComponent<WeaponBase>();
        if (weaponComponent == null) return false;

        return AddWeapon(weaponComponent, ammoToAdd);
    }

    // ===== METODOS AUXILIARES =====

    public bool HasWeapon(int weaponID)
    {
        return GetWeaponByID(weaponID) != null;
    }

    public void RemoveWeapon(int weaponID)
    {
        for (int i = 0; i < weapons.Length; i++)
        {
            if (weapons[i] != null && weapons[i].weaponID == weaponID)
            {
                Destroy(weapons[i].gameObject);
                weapons[i] = null;
                SortWeaponArray();
                break;
            }
        }
    }

}