using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class WeaponManager : MonoBehaviour, IAmmo
{
    [Header("Weapons")]
    public WeaponBase[] weapons = new WeaponBase[4];
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

    private Coroutine cameraZoomCoroutine;

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

        AssignReferencesToExistingWeapons();

        bool foundWeapon = false;
        for (int i = 0; i < weapons.Length; i++)
        {
            if (weapons[i] != null)
            {
                weapons[i].SetActive(false);
                if (!foundWeapon)
                {
                    foundWeapon = true;
                    currentIndex = i;
                    weapons[i].SetActive(true);
                    currentweaponammo = weapons[i].currentAmmo;

                    if (weapons[i].weaponSprite != null && weaponImage != null)
                    {
                        weaponImage.sprite = weapons[i].weaponSprite;
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
        if (!PlayerMovement.isControlEnabled) return;

        if (Input.GetKeyDown(KeyCode.Alpha1)) TrySelectWeapon(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) TrySelectWeapon(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) TrySelectWeapon(2);
        if (Input.GetKeyDown(KeyCode.Alpha4)) TrySelectWeapon(3);

        if (currentIndex < weapons.Length && weapons[currentIndex] != null)
        {
            if (Input.GetButton("Fire1"))
            {
                weapons[currentIndex].TryShoot();
            }
            if (Input.GetButtonDown("Fire2"))
            {
                if (cameraZoomCoroutine != null) StopCoroutine(cameraZoomCoroutine);
                weapons[currentIndex].TryAim();
            }
            if (Input.GetButtonUp("Fire2"))
            {
                weapons[currentIndex].StopAim();
                if (weapons[currentIndex] is SniperRifle sniper)
                {
                    cameraZoomCoroutine = StartCoroutine(CameraZoomOutRoutine(sniper.ZoomSpeed, weapons[currentIndex].DefaultFOVValue));
                }
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
            Debug.Log("No hay arma en el slot " + index);
        }
    }

    void SelectWeapon(int index)
    {
        if (index < 0 || index >= weapons.Length || isTransitioning) return;
        if (weapons[index] == null) return;

        if (currentIndex < weapons.Length && weapons[currentIndex] != null)
        {
            weapons[currentIndex].StopAim();

            if (weapons[currentIndex] is SniperRifle sniper && playerCamera != null && playerCamera.fieldOfView != weapons[currentIndex].DefaultFOVValue)
            {
                cameraZoomCoroutine = StartCoroutine(CameraZoomOutRoutine(sniper.ZoomSpeed, weapons[currentIndex].DefaultFOVValue));
            }
        }

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

        Debug.Log("Arma actual: " + weapons[index].weaponName);
    }

    IEnumerator CameraZoomOutRoutine(float speed, float targetFOV)
    {
        if (playerCamera == null) yield break;

        float startFOV = playerCamera.fieldOfView;

        if (Mathf.Approximately(startFOV, targetFOV)) yield break;

        float startTime = Time.time;
        float duration = Mathf.Abs(targetFOV - startFOV) / speed;

        while (Time.time - startTime < duration)
        {
            float t = (Time.time - startTime) / duration;
            playerCamera.fieldOfView = Mathf.Lerp(startFOV, targetFOV, t);
            yield return null;
        }

        playerCamera.fieldOfView = targetFOV;
        cameraZoomCoroutine = null;
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

    public bool AddWeapon(WeaponBase weaponPrefab, int ammoToAdd = 0)
    {
        if (weaponPrefab == null) return false;

        int targetSlot = weaponPrefab.assignedSlot;

        if (targetSlot < 0 || targetSlot >= weapons.Length)
        {
            Debug.LogError("Slot asignado invalido para " + weaponPrefab.weaponName + ": " + targetSlot);
            return false;
        }

        if (weapons[targetSlot] != null)
        {
            if (weapons[targetSlot].weaponID == weaponPrefab.weaponID)
            {
                weapons[targetSlot].AddAmmo(ammoToAdd);
                Debug.Log("Municion anadida a " + weapons[targetSlot].weaponName + ": +" + ammoToAdd);
                return true;
            }
            else
            {
                Debug.LogWarning("Ya hay un arma diferente en el slot " + targetSlot + ". Reemplazando...");
                Destroy(weapons[targetSlot].gameObject);
                weapons[targetSlot] = null;
            }
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

        weapons[targetSlot] = newWeapon;

        if (ammoToAdd > 0)
            newWeapon.AddAmmo(ammoToAdd);

        if (weapons[currentIndex] == null)
        {
            TrySelectWeapon(targetSlot);
        }

        Debug.Log("Arma anadida: " + newWeapon.weaponName + " en slot " + targetSlot);
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

    public bool PickupWeapon(GameObject weaponPrefab, int ammoToAdd)
    {
        if (weaponPrefab == null) return false;

        WeaponBase weaponComponent = weaponPrefab.GetComponent<WeaponBase>();
        if (weaponComponent == null) return false;

        return AddWeapon(weaponComponent, ammoToAdd);
    }

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
                break;
            }
        }
    }

    public void ClearAllWeapons()
    {
        for (int i = 0; i < weapons.Length; i++)
        {
            if (weapons[i] != null)
            {
                Destroy(weapons[i].gameObject);
                weapons[i] = null;
            }
        }
        currentIndex = 0;
        currentweaponammo = 0;
    }

    public void LoadWeaponsFromMemento(List<PlayerMemento.WeaponData> weaponDataList)
    {
        Debug.Log($"Cargando {weaponDataList.Count} armas desde memento...");

        // 1. Primero desactivar todas las armas actuales
        foreach (WeaponBase weapon in weapons)
        {
            if (weapon != null)
            {
                weapon.SetActive(false);
            }
        }

        // 2. Resetear el array de armas
        for (int i = 0; i < weapons.Length; i++)
        {
            weapons[i] = null;
        }

        // 3. Buscar todas las armas que son hijos del WeaponManager
        WeaponBase[] allWeapons = GetComponentsInChildren<WeaponBase>(true);
        Debug.Log($"Encontradas {allWeapons.Length} armas como hijos del WeaponManager");

        // 4. Reconstruir el array según los datos guardados
        foreach (var data in weaponDataList)
        {
            Debug.Log($"Buscando arma con ID {data.weaponID} para slot {data.slotIndex}");

            // Buscar el arma con el weaponID correspondiente
            WeaponBase matchingWeapon = null;

            foreach (var weapon in allWeapons)
            {
                if (weapon.weaponID == data.weaponID)
                {
                    matchingWeapon = weapon;
                    break;
                }
            }

            if (matchingWeapon != null)
            {
                // Colocar el arma en su slot correcto
                int slotIndex = data.slotIndex;

                if (slotIndex >= 0 && slotIndex < weapons.Length)
                {
                    weapons[slotIndex] = matchingWeapon;
                    matchingWeapon.currentAmmo = data.currentAmmo;
                    matchingWeapon.SetActive(false);

                    Debug.Log($"Arma cargada: {matchingWeapon.weaponName} en slot {slotIndex} con {data.currentAmmo} balas");
                }
            }
            else
            {
                Debug.LogWarning($"No se encontró arma con ID {data.weaponID}. El jugador debe recoger el pickup primero.");
            }
        }

        // 5. Verificar qué armas tenemos cargadas
        Debug.Log("Estado final del array de armas:");
        for (int i = 0; i < weapons.Length; i++)
        {
            if (weapons[i] != null)
            {
                Debug.Log($"Slot {i}: {weapons[i].weaponName} ({weapons[i].currentAmmo} balas)");
            }
            else
            {
                Debug.Log($"Slot {i}: Vacío");
            }
        }
    }
}