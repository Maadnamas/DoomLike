using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movimiento")]
    public float moveSpeed = 5f;
    public float runSpeed = 8f;

    [Header("Rotación con Mouse")]
    public float mouseSensitivity = 2f;

    [Header("Gravedad y Salto")]
    public float gravity = -9.81f;
    public float jumpHeight = 2f;
    public float groundCheckDistance = 0.3f;
    public LayerMask groundMask;
    public KeyCode jumpKey = KeyCode.Space;

    private CharacterController controller;
    private Camera playerCamera;
    private float xRotation = 0f;
    private Vector3 velocity;
    private bool isGrounded;

    public static bool isControlEnabled = true;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        playerCamera = Camera.main;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (!isControlEnabled)
            return;

        CheckGround();
        Movimiento();
        RotacionMouse();
        AplicarGravedad();
    }

    void CheckGround()
    {
        Vector3 checkPosition = transform.position + Vector3.down * (controller.height / 2f + 0.1f);
        isGrounded = Physics.CheckSphere(checkPosition, groundCheckDistance, groundMask);
    }

    void Movimiento()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : moveSpeed;

        controller.Move(move * currentSpeed * Time.deltaTime);

        if (isGrounded && Input.GetKeyDown(jumpKey))
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }

    void AplicarGravedad()
    {
        if (isGrounded && velocity.y < 0)
            velocity.y = -5f;

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    void RotacionMouse()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        transform.Rotate(Vector3.up * mouseX);

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }

    void OnDrawGizmosSelected()
    {
        if (controller == null) return;
        Gizmos.color = Color.yellow;
        Vector3 checkPosition = transform.position + Vector3.down * (controller.height / 2f - 0.1f);
        Gizmos.DrawWireSphere(checkPosition, groundCheckDistance);
    }

    public PlayerMemento SaveState(PlayerHealth health, WeaponManager weaponManager)
    {
        if (health == null)
        {
            Debug.LogError("PlayerHealth es null en SaveState");
            return null;
        }

        if (weaponManager == null)
        {
            Debug.LogError("WeaponManager es null en SaveState");
            return null;
        }

        string sceneName = SceneManager.GetActiveScene().name;

        List<PlayerMemento.WeaponData> weaponList = new List<PlayerMemento.WeaponData>();

        for (int i = 0; i < weaponManager.weapons.Length; i++)
        {
            if (weaponManager.weapons[i] != null)
            {
                var data = new PlayerMemento.WeaponData
                {
                    weaponID = weaponManager.weapons[i].weaponID,
                    currentAmmo = weaponManager.weapons[i].currentAmmo,
                    slotIndex = i
                };
                weaponList.Add(data);
            }
        }

        // Obtener cantidad de medkits
        int medkitCount = health.GetMedkitCount();

        PlayerMemento memento = new PlayerMemento(
            sceneName,
            transform.position,
            transform.rotation,
            xRotation,
            health.GetCurrentHealth(),
            medkitCount,
            weaponList,
            weaponManager.currentIndex
        );

        // Guardar estado de pickups
        if (WeaponPickupManager.Instance != null)
        {
            WeaponPickupManager.Instance.SavePickupStates(memento);
        }

        // Guardar estado de larvas
        if (LarvaManager.Instance != null)
        {
            LarvaManager.Instance.SaveLarvaStates(memento);
        }

        Debug.Log("Partida guardada correctamente");
        return memento;
    }

    public void LoadState(PlayerMemento memento, PlayerHealth health, WeaponManager weaponManager)
    {
        if (memento == null)
        {
            Debug.LogError("Memento es null en LoadState");
            return;
        }

        if (health == null)
        {
            Debug.LogError("PlayerHealth es null en LoadState");
            return;
        }

        if (weaponManager == null)
        {
            Debug.LogError("WeaponManager es null en LoadState");
            return;
        }

        CharacterController controller = GetComponent<CharacterController>();
        if (controller == null) return;

        // Deshabilitar controlador temporalmente para mover al jugador
        bool wasEnabled = controller.enabled;
        controller.enabled = false;

        // Restaurar posición y rotación
        transform.position = memento.position;
        transform.rotation = memento.playerRotation;

        // Restaurar rotación de cámara
        xRotation = memento.cameraPitch;
        if (playerCamera != null)
            playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
        else if (Camera.main)
            Camera.main.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);

        // Reactivar controlador
        controller.enabled = wasEnabled;

        // Restaurar salud (esto debería actualizar el HUD automáticamente)
        health.SetCurrentHealth(memento.health);

        // Restaurar medkits
        RestoreMedkits(health, memento.medkitCount);

        // Cargar armas
        weaponManager.LoadWeaponsFromMemento(memento.weapons);

        // Equipar arma correcta
        EquipWeaponFromSave(weaponManager, memento.equippedWeaponIndex);

        // Restaurar pickups (solo activar/desactivar)
        if (WeaponPickupManager.Instance != null)
        {
            WeaponPickupManager.Instance.LoadPickupStates(memento);
        }

        // Restaurar larvas (solo activar/desactivar)
        if (LarvaManager.Instance != null)
        {
            LarvaManager.Instance.LoadLarvaStates(memento);
        }

        Debug.Log("Partida cargada correctamente");
    }

    // Método para restaurar medkits
    private void RestoreMedkits(PlayerHealth health, int targetCount)
    {
        try
        {
            // Intentar usar SetMedkitCount si existe
            var setMethod = health.GetType().GetMethod("SetMedkitCount");
            if (setMethod != null)
            {
                setMethod.Invoke(health, new object[] { targetCount });
                return;
            }

            // Si no existe SetMedkitCount, intentar con AddMedkit
            var addMethod = health.GetType().GetMethod("AddMedkit");
            if (addMethod != null)
            {
                // Primero resetear a 0
                var currentMedkitsMethod = health.GetType().GetMethod("GetMedkitCount");
                if (currentMedkitsMethod != null)
                {
                    int current = (int)currentMedkitsMethod.Invoke(health, null);

                    // Agregar la diferencia
                    for (int i = current; i < targetCount; i++)
                    {
                        addMethod.Invoke(health, null);
                    }
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning("No se pudo restaurar medkits: " + e.Message);
        }
    }

    // Método para equipar arma al cargar
    private void EquipWeaponFromSave(WeaponManager weaponManager, int weaponIndex)
    {
        if (weaponManager == null) return;

        try
        {
            // Intentar usar TrySelectWeapon si es público
            var trySelectMethod = typeof(WeaponManager).GetMethod("TrySelectWeapon");
            if (trySelectMethod != null)
            {
                trySelectMethod.Invoke(weaponManager, new object[] { weaponIndex });
                return;
            }

            // Intentar usar SelectWeapon si es público
            var selectMethod = typeof(WeaponManager).GetMethod("SelectWeapon");
            if (selectMethod != null)
            {
                selectMethod.Invoke(weaponManager, new object[] { weaponIndex });
                return;
            }

            // Si no hay métodos públicos, hacerlo manualmente
            if (weaponIndex >= 0 && weaponIndex < weaponManager.weapons.Length &&
                weaponManager.weapons[weaponIndex] != null)
            {
                // Desactivar todas las armas
                for (int i = 0; i < weaponManager.weapons.Length; i++)
                {
                    if (weaponManager.weapons[i] != null)
                    {
                        weaponManager.weapons[i].SetActive(i == weaponIndex);
                    }
                }

                // Actualizar índice actual
                weaponManager.currentIndex = weaponIndex;

                // Actualizar munición actual
                if (weaponManager.weapons[weaponIndex] != null)
                {
                    weaponManager.currentweaponammo = weaponManager.weapons[weaponIndex].currentAmmo;
                }

                // Disparar evento para actualizar HUD
                EventManager.TriggerEvent(GameEvents.WEAPON_SWITCHED, new WeaponSwitchEventData
                {
                    weaponName = weaponManager.weapons[weaponIndex].weaponName,
                    ammoCount = weaponManager.weapons[weaponIndex].currentAmmo
                });

                Debug.Log($"Arma equipada desde guardado: índice {weaponIndex}");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning("Error al equipar arma: " + e.Message);
        }
    }
}