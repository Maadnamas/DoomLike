using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("movimiento")]
    public float moveSpeed = 5f;
    public float runSpeed = 8f;

    [Header("rotacion con mouse")]
    public float mouseSensitivity = 2f;

    [Header("gravedad y salto")]
    public float gravity = -9.81f;
    public float jumpHeight = 2f;
    public float groundCheckDistance = 0.3f;
    public LayerMask groundMask;
    public KeyCode jumpKey = KeyCode.Space;

    [Header("material correr")]
    public Material runMaterial;
    public float alphaAppearSpeed = 3f;

    [Header("Sonido de Pasos")]
    public AudioClip footstepSound;
    public float footstepDelay = 0.4f;
    private float footstepTimer;

    CharacterController controller;
    Camera playerCamera;
    float xRotation;
    Vector3 velocity;
    bool isGrounded;

    bool isRunning;
    Coroutine alphaRoutine;

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
        CheckRunAlpha();
        PlayFootstepSound();
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
        float currentSpeed = isRunning ? runSpeed : moveSpeed;

        controller.Move(move * currentSpeed * Time.deltaTime);

        if (isGrounded && Input.GetKeyDown(jumpKey))
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }

    void PlayFootstepSound()
    {
        if (footstepSound == null) return;

        Vector3 horizontalVelocity = controller.velocity;
        horizontalVelocity.y = 0;
        bool isMoving = horizontalVelocity.magnitude > 0.1f;

        if (isGrounded && isMoving)
        {
            footstepTimer += Time.deltaTime;

            float speedFactor = isRunning ? 0.6f : 1f;
            float calculatedDelay = footstepDelay * speedFactor;

            if (footstepTimer >= calculatedDelay)
            {
                Debug.Log("Intentando reproducir paso.");
                AudioManager.PlaySFX2D(footstepSound);
                footstepTimer = 0f;
            }
        }
        else
        {
            footstepTimer = 0f;
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

    void CheckRunAlpha()
    {
        bool runningNow = Input.GetKey(KeyCode.LeftShift);

        if (runningNow != isRunning)
        {
            isRunning = runningNow;

            if (alphaRoutine != null)
                StopCoroutine(alphaRoutine);

            alphaRoutine = StartCoroutine(LerpAlpha(isRunning ? 1f : 0f));
        }
    }

    IEnumerator LerpAlpha(float targetAlpha)
    {
        if (runMaterial == null)
            yield break;

        float startAlpha = runMaterial.GetFloat("_Alpha");
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * alphaAppearSpeed;
            float alpha = Mathf.Lerp(startAlpha, targetAlpha, t);
            runMaterial.SetFloat("_Alpha", alpha);
            yield return null;
        }

        runMaterial.SetFloat("_Alpha", targetAlpha);
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
        if (health == null || weaponManager == null)
            return null;

        string sceneName = SceneManager.GetActiveScene().name;
        List<PlayerMemento.WeaponData> weaponList = new List<PlayerMemento.WeaponData>();

        for (int i = 0; i < weaponManager.weapons.Length; i++)
        {
            if (weaponManager.weapons[i] != null)
            {
                weaponList.Add(new PlayerMemento.WeaponData
                {
                    weaponID = weaponManager.weapons[i].weaponID,
                    currentAmmo = weaponManager.weapons[i].currentAmmo,
                    slotIndex = i
                });
            }
        }

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

        if (WeaponPickupManager.Instance != null)
            WeaponPickupManager.Instance.SavePickupStates(memento);

        if (LarvaManager.Instance != null)
            LarvaManager.Instance.SaveLarvaStates(memento);

        return memento;
    }

    public void LoadState(PlayerMemento memento, PlayerHealth health, WeaponManager weaponManager)
    {
        if (memento == null || health == null || weaponManager == null)
            return;

        bool wasEnabled = controller.enabled;
        controller.enabled = false;

        transform.position = memento.position;
        transform.rotation = memento.playerRotation;

        xRotation = memento.cameraPitch;
        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);

        controller.enabled = wasEnabled;

        health.SetCurrentHealth(memento.health);
        weaponManager.LoadWeaponsFromMemento(memento.weapons);
    }
}