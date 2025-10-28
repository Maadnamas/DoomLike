using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    [Header("Weapons")]
    public WeaponBase[] weapons;
    private int currentIndex = 0;

    void Start()
    {
        SelectWeapon(0); // empieza con la primera (Pistol)
    }

    void Update()
    {
        // Cambiar arma con números
        if (Input.GetKeyDown(KeyCode.Alpha1)) SelectWeapon(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SelectWeapon(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) SelectWeapon(2);

        // Disparo
        if (Input.GetButton("Fire1"))
        {
            weapons[currentIndex].TryShoot();
        }
    }

    void SelectWeapon(int index)
    {
        if (index < 0 || index >= weapons.Length) return;
        for (int i = 0; i < weapons.Length; i++)
            weapons[i].SetActive(i == index);

        currentIndex = index;
        Debug.Log($"Arma actual: {weapons[index].weaponName}");
    }
}
