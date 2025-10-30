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

        if (currentIndex != 0)
        {
            currentweaponammo = weapons[currentIndex].currentAmmo;
        }
    }

    void SelectWeapon(int index)
    {
        if (index < 0 || index >= weapons.Length || isTransitioning) return;

        for (int i = 0; i < weapons.Length; i++)
            weapons[i].SetActive(i == index);

        StartCoroutine(ChangeWeaponUI(index));

        currentIndex = index;
        Debug.Log($"arma actual: {weapons[index].weaponName}");
    }

    IEnumerator ChangeWeaponUI(int newIndex)
    {
        isTransitioning = true;

        if (weaponImageRect)
        {
            // baja el icono
            while (weaponImageRect.anchoredPosition.y > originalPos.y - downamount)
            {
                weaponImageRect.anchoredPosition -= new Vector2(0, transitionSpeed * Time.deltaTime);
                yield return null;
            }

            // cambia el sprite
            if (weapons[newIndex].weaponSprite != null)
                weaponImage.sprite = weapons[newIndex].weaponSprite;

            // sube de nuevo
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
            return true;
        }
        else return false;
    }
}
