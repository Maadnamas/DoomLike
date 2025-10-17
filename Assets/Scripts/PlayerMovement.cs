using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movimiento")]
    public float moveSpeed = 5f;

    [Header("Rotación con Mouse")]
    public float mouseSensitivity = 2f;

    private CharacterController controller;
    private Camera playerCamera;
    private float xRotation = 0f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        playerCamera = Camera.main;

        // Ocultar y bloquear el cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        Movimiento();
        RotacionMouse();
    }

    void Movimiento()
    {
        float moveX = Input.GetAxis("Horizontal");  // A/D o Flechas
        float moveZ = Input.GetAxis("Vertical");    // W/S o Flechas

        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        controller.Move(move * moveSpeed * Time.deltaTime);
    }

    void RotacionMouse()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Rotar el cuerpo del jugador en eje Y (izq-der)
        transform.Rotate(Vector3.up * mouseX);

        // Rotar la cámara en eje X (arriba-abajo)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }
}