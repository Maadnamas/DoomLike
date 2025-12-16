using UnityEngine;

public class BreathingCameraEffect : MonoBehaviour
{
    [Header("Configuración de Respiración")]
    [Tooltip("Intensidad del movimiento de respiración")]
    [SerializeField][Range(0f, 0.5f)] private float breathingIntensity = 0.1f;

    [Tooltip("Velocidad de la respiración")]
    [SerializeField][Range(0.1f, 5f)] private float breathingSpeed = 1f;

    [Header("Configuración del FOV")]
    [Tooltip("FOV inicial")]
    [SerializeField] private float startFOV = 7f;

    [Tooltip("FOV final")]
    [SerializeField] private float endFOV = 60f;

    [Tooltip("Tiempo para llegar al FOV final")]
    [SerializeField][Range(0.5f, 10f)] private float fovTransitionTime = 3f;

    [Header("Configuración del Efecto")]
    [Tooltip("Duración total del efecto (0 = infinito)")]
    [SerializeField] private float totalDuration = 0f;

    [Tooltip("Iniciar automáticamente")]
    [SerializeField] private bool startOnAwake = true;

    [Header("Ejes de Movimiento")]
    [Tooltip("Movimiento en eje X")]
    [SerializeField] private bool moveX = true;

    [Tooltip("Movimiento en eje Y")]
    [SerializeField] private bool moveY = true;

    [Tooltip("Movimiento en eje Z")]
    [SerializeField] private bool moveZ = false;

    // Componentes
    private Camera cam;
    private Vector3 originalPosition;

    // Variables de control
    private float breathingTimer = 0f;
    private float fovTimer = 0f;
    private float effectTimer = 0f;
    private bool isActive = true;
    private bool fovCompleted = false;

    void Start()
    {
        cam = GetComponent<Camera>();

        if (cam == null)
        {
            Debug.LogError("BreathingCameraEffect necesita un componente Camera");
            enabled = false;
            return;
        }

        originalPosition = transform.localPosition;

        // Configurar FOV inicial
        cam.fieldOfView = startFOV;

        if (startOnAwake)
        {
            StartEffect();
        }
        else
        {
            isActive = false;
        }
    }

    void Update()
    {
        if (!isActive) return;

        // Actualizar temporizadores
        breathingTimer += Time.deltaTime * breathingSpeed;
        fovTimer += Time.deltaTime;
        effectTimer += Time.deltaTime;

        // Controlar duración total
        if (totalDuration > 0 && effectTimer >= totalDuration)
        {
            StopEffect();
            return;
        }

        // Aplicar efectos
        ApplyBreathingMovement();
        ApplyFOVTransition();
    }

    void ApplyBreathingMovement()
    {
        if (breathingIntensity <= 0) return;

        // Patrón de respiración suave usando seno
        float breathCycle = Mathf.Sin(breathingTimer);

        // Movimiento más natural con suavizado
        float smoothBreath = Mathf.SmoothStep(-1f, 1f, (breathCycle + 1f) / 2f);

        // Calcular desplazamiento
        Vector3 offset = Vector3.zero;

        if (moveX) offset.x = smoothBreath * breathingIntensity * 0.5f;
        if (moveY) offset.y = smoothBreath * breathingIntensity;
        if (moveZ) offset.z = smoothBreath * breathingIntensity * 0.3f;

        // Aplicar con suavizado adicional
        transform.localPosition = Vector3.Lerp(
            transform.localPosition,
            originalPosition + offset,
            Time.deltaTime * 5f
        );
    }

    void ApplyFOVTransition()
    {
        if (fovCompleted || fovTransitionTime <= 0) return;

        // Calcular progreso de transición (0 a 1)
        float progress = Mathf.Clamp01(fovTimer / fovTransitionTime);

        // Usar curva suave para transición más natural
        float smoothProgress = SmoothStep(progress);

        // Interpolar FOV
        cam.fieldOfView = Mathf.Lerp(startFOV, endFOV, smoothProgress);

        // Comprobar si completó la transición
        if (progress >= 1f)
        {
            fovCompleted = true;
            cam.fieldOfView = endFOV; // Asegurar valor exacto
        }
    }

    float SmoothStep(float t)
    {
        // Curva personalizada para transición suave
        return t * t * (3f - 2f * t);
    }

    // Métodos públicos para control
    public void StartEffect()
    {
        isActive = true;
        breathingTimer = 0f;
        fovTimer = 0f;
        effectTimer = 0f;
        fovCompleted = false;
        transform.localPosition = originalPosition;

        if (cam != null)
        {
            cam.fieldOfView = startFOV;
        }
    }

    public void StopEffect()
    {
        isActive = false;

        // Regresar suavemente a posición original
        StopAllCoroutines();
        StartCoroutine(ReturnToOriginalPosition());

        Debug.Log("Efecto de respiración detenido");
    }

    System.Collections.IEnumerator ReturnToOriginalPosition()
    {
        float returnTime = 0.5f;
        float timer = 0f;
        Vector3 startPos = transform.localPosition;

        while (timer < returnTime)
        {
            timer += Time.deltaTime;
            float progress = timer / returnTime;
            transform.localPosition = Vector3.Lerp(startPos, originalPosition, progress);
            yield return null;
        }

        transform.localPosition = originalPosition;
    }

    public void SetBreathingIntensity(float intensity)
    {
        breathingIntensity = Mathf.Clamp(intensity, 0f, 0.5f);
    }

    public void SetBreathingSpeed(float speed)
    {
        breathingSpeed = Mathf.Clamp(speed, 0.1f, 5f);
    }

    // Métodos para el inspector
    [ContextMenu("Iniciar Efecto")]
    void StartEffectContext()
    {
        if (Application.isPlaying)
        {
            StartEffect();
            Debug.Log("Efecto iniciado");
        }
    }

    [ContextMenu("Detener Efecto")]
    void StopEffectContext()
    {
        if (Application.isPlaying)
        {
            StopEffect();
            Debug.Log("Efecto detenido");
        }
    }

    [ContextMenu("Reiniciar Efecto")]
    void ResetEffectContext()
    {
        if (Application.isPlaying)
        {
            StartEffect();
            Debug.Log("Efecto reiniciado");
        }
    }

    // Visualización en el editor
    void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying) return;

        // Mostrar rango de movimiento
        Gizmos.color = Color.cyan;
        Vector3 maxOffset = new Vector3(
            moveX ? breathingIntensity * 0.5f : 0,
            moveY ? breathingIntensity : 0,
            moveZ ? breathingIntensity * 0.3f : 0
        );

        Vector3 currentPos = transform.position;
        Gizmos.DrawWireSphere(currentPos - maxOffset, 0.02f);
        Gizmos.DrawWireSphere(currentPos + maxOffset, 0.02f);
        Gizmos.DrawLine(currentPos - maxOffset, currentPos + maxOffset);
    }
}