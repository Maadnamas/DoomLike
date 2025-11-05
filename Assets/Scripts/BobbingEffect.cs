using UnityEngine;
using UnityEngine.UI;

public class BobbingEffect : MonoBehaviour
{
    [Header("Referencias")]
    public Transform cameraTransform;
    public Image targetImage;
    public CharacterController playerController;

    [Header("Configuración Bobbing - Cámara")]
    [Tooltip("velocidad del bobbing (frecuencia) mientras camina")]
    public float walkBobbingSpeed = 7f;
    [Tooltip("velocidad del bobbing (frecuencia) mientras corre")]
    public float runBobbingSpeed = 10f;
    [Tooltip("amplitud del bobbing")]
    public float bobbingAmount = 0.06f;
    [Tooltip("suavizado al interpolar la posición/rotación")]
    public float bobbingSmoothness = 8f;
    public bool enableTilt = true;
    public float tiltAmount = 1.5f;

    [Header("Configuración Bobbing - Imagen UI")]
    public bool applyToImage = false;
    public float imageBobbingMultiplier = 0.35f;
    public bool independentImageBobbing = false;
    public float imageBobbingSpeed = 8f;

    [Header("Debug")]
    public bool showDebug = false;

    // privados
    private Vector3 cameraOriginalPos;
    private Vector2 imageOriginalPos;
    private Quaternion cameraOriginalRot;
    private Quaternion imageOriginalRot;
    private float timer = 0f;
    private float imageTimer = 0f;

    void Start()
    {
        // asignar cámara si no se puso en inspector
        if (cameraTransform == null)
        {
            Camera c = Camera.main;
            if (c == null)
            {
                c = FindObjectOfType<Camera>();
            }
            if (c != null) cameraTransform = c.transform;
        }

        // asignar playerController si no está
        if (playerController == null)
        {
            playerController = GetComponentInParent<CharacterController>();
        }

        // buscar imagen si corresponde
        if (targetImage == null && applyToImage)
        {
            targetImage = GetComponentInChildren<Image>();
        }

        // guardar posiciones/rotaciones originales
        if (cameraTransform != null)
        {
            cameraOriginalPos = cameraTransform.localPosition;
            cameraOriginalRot = cameraTransform.localRotation;
        }

        if (targetImage != null)
        {
            imageOriginalPos = targetImage.rectTransform.anchoredPosition;
            imageOriginalRot = targetImage.rectTransform.localRotation;
        }

    }

    void Update()
    {
        ApplyBobbing();
    }

    bool IsMoving()
    {
        // Detectar movimiento horizontal basado en input, no en velocity del controller
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 inputVector = new Vector3(horizontal, 0f, vertical);

        // Si el jugador está en el aire, no hacer bobbing
        if (playerController != null && !playerController.isGrounded)
            return false;

        return inputVector.magnitude > 0.1f;
    }

    void ApplyBobbing()
    {
        bool moving = IsMoving();

        if (!moving)
        {
            // reset suave hacia originales
            timer = 0f; // reiniciamos el timer para que al empezar de nuevo arranque consistente
            ResetToOriginalPositions();
            return;
        }

        // elegir velocidad
        float currentBobbingSpeed = Input.GetKey(KeyCode.LeftShift) ? runBobbingSpeed : walkBobbingSpeed;

        // avanzar timer (no lo lerpeamos, eso rompe la onda)
        timer += Time.deltaTime * currentBobbingSpeed;

        float waveSlice = Mathf.Sin(timer);
        float horizontalBob = Mathf.Cos(timer * 0.5f) * bobbingAmount * 0.5f;
        float verticalBob = waveSlice * bobbingAmount;

        if (cameraTransform != null)
        {
            ApplyCameraBobbing(horizontalBob, verticalBob, timer);
        }
        else if (showDebug)
        {
            Debug.LogWarning("BobbingEffect: cameraTransform es nulo");
        }

        if (applyToImage && targetImage != null)
        {
            ApplyImageBobbing(horizontalBob, verticalBob, timer);
        }
    }

    void ApplyCameraBobbing(float horizontalBob, float verticalBob, float t)
    {
        Vector3 targetPos = cameraOriginalPos + new Vector3(horizontalBob, verticalBob, 0f);
        cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, targetPos, Time.deltaTime * bobbingSmoothness);

        if (enableTilt)
        {
            float tilt = Mathf.Sin(t * 0.7f) * tiltAmount;
            Quaternion targetRot = cameraOriginalRot * Quaternion.Euler(0f, 0f, tilt);
            cameraTransform.localRotation = Quaternion.Slerp(cameraTransform.localRotation, targetRot, Time.deltaTime * bobbingSmoothness);
        }


    }

    void ApplyImageBobbing(float horizontalBob, float verticalBob, float t)
    {
        Vector2 imageTargetPos;
        float imageTilt = 0f;

        // usar la velocidad del jugador para ajustar la velocidad de bobbing
        float currentBobbingSpeed = Input.GetKey(KeyCode.LeftShift) ? runBobbingSpeed : walkBobbingSpeed;

        if (independentImageBobbing)
        {
            // el timer de la imagen corre al mismo ritmo que el jugador
            imageTimer += Time.deltaTime * currentBobbingSpeed;
            float w = Mathf.Sin(imageTimer);
            float ih = Mathf.Cos(imageTimer * 0.5f) * bobbingAmount * 0.5f * imageBobbingMultiplier;
            float iv = w * bobbingAmount * imageBobbingMultiplier;

            imageTargetPos = imageOriginalPos + new Vector2(ih, iv);
            imageTilt = Mathf.Sin(imageTimer * 0.7f) * tiltAmount * imageBobbingMultiplier;
        }
        else
        {
            // sincronizado con el movimiento de la cámara (usa el mismo timer)
            imageTargetPos = imageOriginalPos + new Vector2(
                horizontalBob * imageBobbingMultiplier,
                verticalBob * imageBobbingMultiplier
            );
            imageTilt = Mathf.Sin(t * 0.7f) * tiltAmount * imageBobbingMultiplier;
        }

        targetImage.rectTransform.anchoredPosition = Vector2.Lerp(
            targetImage.rectTransform.anchoredPosition,
            imageTargetPos,
            Time.deltaTime * bobbingSmoothness
        );

        if (enableTilt)
        {
            Quaternion imageTargetRot = imageOriginalRot * Quaternion.Euler(0f, 0f, imageTilt);
            targetImage.rectTransform.localRotation = Quaternion.Slerp(
                targetImage.rectTransform.localRotation,
                imageTargetRot,
                Time.deltaTime * bobbingSmoothness
            );
        }
    }



    void ResetToOriginalPositions()
    {
        if (cameraTransform != null)
        {
            cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, cameraOriginalPos, Time.deltaTime * bobbingSmoothness);
            if (enableTilt)
                cameraTransform.localRotation = Quaternion.Slerp(cameraTransform.localRotation, cameraOriginalRot, Time.deltaTime * bobbingSmoothness);
        }

        if (applyToImage && targetImage != null)
        {
            targetImage.rectTransform.anchoredPosition = Vector2.Lerp(targetImage.rectTransform.anchoredPosition, imageOriginalPos, Time.deltaTime * bobbingSmoothness);
            targetImage.rectTransform.localRotation = Quaternion.Slerp(targetImage.rectTransform.localRotation, imageOriginalRot, Time.deltaTime * bobbingSmoothness);
        }
    }
}
