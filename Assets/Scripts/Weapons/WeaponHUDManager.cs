using UnityEngine;
using UnityEngine.UI;

public class WeaponHUDSimple : MonoBehaviour
{
    [Header("Referencias HUD")]
    public Image[] weaponIcons = new Image[4];

    private WeaponManager weaponManager;

    void Start()
    {

        weaponManager = GetComponent<WeaponManager>();
        if (weaponManager == null)
        {
            weaponManager = FindObjectOfType<WeaponManager>();
        }
        ClearAllIcons();
    }

    void Update()
    {
        if (weaponManager == null) return;

        UpdateHUD();
    }

    void UpdateHUD()
    {
        for (int i = 0; i < weaponIcons.Length; i++)
        {
            if (weaponIcons[i] == null) continue;
            bool hasWeapon = i < weaponManager.weapons.Length &&
                            weaponManager.weapons[i] != null;

            if (hasWeapon)
            {
                WeaponBase weapon = weaponManager.weapons[i];
                Sprite icon = weapon.weaponIcon;

                weaponIcons[i].sprite = icon;
                weaponIcons[i].color = Color.white;

                if (i == weaponManager.currentIndex)
                {
                    weaponIcons[i].transform.localScale = Vector3.one * 1.2f;
                }
                else
                {
                    weaponIcons[i].transform.localScale = Vector3.one;
                }
            }
            else
            {
                weaponIcons[i].sprite = null;
                weaponIcons[i].color = new Color(1, 1, 1, 0f);
                weaponIcons[i].transform.localScale = Vector3.one;
            }
        }
    }

    void ClearAllIcons()
    {
        foreach (Image icon in weaponIcons)
        {
            if (icon != null)
            {
                icon.sprite = null;
                icon.color = new Color(1, 1, 1, 0.1f);
            }
        }
    }
    public void RefreshHUD()
    {
        UpdateHUD();
    }
}