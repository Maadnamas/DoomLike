using UnityEngine;

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

    void Start()
    {
        controller = GetComponent<CharacterController>();
        playerCamera = Camera.main;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        CheckGround();
        Movimiento();
        RotacionMouse();
        AplicarGravedad();
    }

    void CheckGround()
    {
        // El punto de chequeo un poco más abajo del centro
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
        // Si está en el suelo y la velocidad vertical es negativa, la reseteamos suavemente
        if (isGrounded && velocity.y < 0)
            velocity.y = -5f; // en vez de -2 o 0, esto ayuda a mantener pegado al suelo

        // Aplicar gravedad acumulativa
        velocity.y += gravity * Time.deltaTime;

        // Mover
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
}