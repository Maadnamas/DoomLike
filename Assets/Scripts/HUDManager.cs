using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HUDManager : MonoBehaviour
{
    [SerializeField] WeaponManager weaponManager;
    [SerializeField] TextMeshProUGUI ammo;

    private void Update()
    {
        if (weaponManager.currentIndex != 0)
        {
            ammo.text = weaponManager.currentweaponammo.ToString();
        }
        else
        {
            ammo.text = "";
        }
    }
}
