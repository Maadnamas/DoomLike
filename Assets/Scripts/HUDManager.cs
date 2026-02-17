using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI ammoText;
    [SerializeField] private Image healthBar;
    [SerializeField] private GameObject ammoPanel;

    [Header("Manager References")]
    [SerializeField] private WeaponManager weaponManager;

    private bool isWeaponZeroEquipped = false;

    void OnEnable()
    {
        StartCoroutine(SubscribeWithDelay());
    }

    void OnDisable()
    {
        UnsubscribeFromEvents();
    }

    void Start()
    {
        if (ammoPanel != null)
            ammoPanel.SetActive(false);
    }

    void OnDestroy()
    {
        UnsubscribeFromEvents();
    }

    private IEnumerator SubscribeWithDelay()
    {
        while (EventManager.Instance == null)
        {
            yield return null;
        }

        EventManager.StartListening(GameEvents.UI_UPDATE_HEALTH, OnHealthUpdated);
        EventManager.StartListening(GameEvents.UI_UPDATE_AMMO, OnAmmoUpdated);
        EventManager.StartListening(GameEvents.WEAPON_SWITCHED, OnWeaponSwitched);
        EventManager.StartListening(GameEvents.AMMO_CHANGED, OnAmmoChanged);
    }

    private void UnsubscribeFromEvents()
    {
        if (EventManager.Instance == null) return;

        EventManager.StopListening(GameEvents.UI_UPDATE_HEALTH, OnHealthUpdated);
        EventManager.StopListening(GameEvents.UI_UPDATE_AMMO, OnAmmoUpdated);
        EventManager.StopListening(GameEvents.WEAPON_SWITCHED, OnWeaponSwitched);
        EventManager.StopListening(GameEvents.AMMO_CHANGED, OnAmmoChanged);
    }

    private void OnHealthUpdated(object data)
    {
        if (data is DamageEventData healthData)
        {
            UpdateHealthBar(healthData.currentHealth, healthData.maxHealth);
        }
    }

    private void OnAmmoUpdated(object data)
    {
        if (data is AmmoEventData ammoData)
        {
            UpdateAmmoUI(ammoData.totalAmmo);
        }
    }

    private void OnWeaponSwitched(object data)
    {
        if (data is WeaponSwitchEventData weaponData)
        {
            HandleWeaponSwitch(weaponData);
        }
    }

    private void OnAmmoChanged(object data)
    {
        if (data is AmmoEventData ammoData)
        {
            if (!isWeaponZeroEquipped)
            {
                UpdateAmmoUI(ammoData.totalAmmo);
            }
        }
    }

    private void UpdateHealthBar(int currentHealth, int maxHealth)
    {
        if (healthBar != null)
            healthBar.fillAmount = (float)currentHealth / maxHealth;
    }

    private void UpdateAmmoUI(int ammoCount)
    {
        if (ammoText != null)
            ammoText.text = ammoCount.ToString();
    }

    private void HandleWeaponSwitch(WeaponSwitchEventData weaponData)
    {
        isWeaponZeroEquipped = weaponManager != null && weaponManager.currentIndex == 0;

        if (ammoPanel != null)
            ammoPanel.SetActive(!isWeaponZeroEquipped);

        if (!isWeaponZeroEquipped)
        {
            UpdateAmmoUI(weaponData.ammoCount);
        }
        else if (ammoText != null)
        {
            ammoText.text = "";
        }
    }
}