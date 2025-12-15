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

    // 🔹 NUEVO: bandera global para pausar el control del jugador
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
        // 🔹 Si el control está deshabilitado, no mover ni rotar
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

    // guardar
    public PlayerMemento SaveState(PlayerHealth health, WeaponManager weaponManager)
    {
        // 🔹 NUEVO: Validar parámetros
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

        // 🔹 NUEVO: Validar array de armas
        List<PlayerMemento.WeaponData> weaponList = new List<PlayerMemento.WeaponData>();
        if (weaponManager.weapons != null)
        {
            foreach (var weapon in weaponManager.weapons)
            {
                if (weapon != null) // Validar cada arma
                {
                    var data = new PlayerMemento.WeaponData
                    {
                        weaponID = weapon.weaponID,
                        currentAmmo = weapon.currentAmmo
                    };
                    weaponList.Add(data);
                }
            }
        }

        // Crear snapshot
        return new PlayerMemento(
            sceneName,
            transform.position,
            transform.rotation,
            xRotation,
            health.GetCurrentHealth(),
            weaponList,
            weaponManager.currentIndex
        );
    }

    public void LoadState(PlayerMemento memento, PlayerHealth health, WeaponManager weaponManager)
    {
        // 🔹 NUEVO: Validar memento
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

        if (controller.enabled) controller.enabled = false;

        transform.position = memento.position;
        transform.rotation = memento.playerRotation;

        // Restaurar rotación vertical (cámara)
        xRotation = memento.cameraPitch;
        if (playerCamera != null)
            playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
        else if (Camera.main)
            Camera.main.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);

        controller.enabled = true;

        // Restaurar vida
        health.SetCurrentHealth(memento.health);

        // Restaurar armas y munición
        if (weaponManager.weapons != null && memento.weapons != null)
        {
            for (int i = 0; i < weaponManager.weapons.Length && i < memento.weapons.Count; i++)
            {
                if (weaponManager.weapons[i] != null)
                {
                    weaponManager.weapons[i].currentAmmo = memento.weapons[i].currentAmmo;
                }
            }
        }

        weaponManager.currentIndex = memento.equippedWeaponIndex;

        // Forzar selección del arma
        weaponManager.SendMessage("SelectWeapon", weaponManager.currentIndex,
            SendMessageOptions.DontRequireReceiver);
    }
}