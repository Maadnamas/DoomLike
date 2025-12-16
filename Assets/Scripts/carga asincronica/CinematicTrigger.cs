using System.Collections;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using UnityEngine.Events;

public class CinematicTrigger : MonoBehaviour
{
    [Header("Configuración de Video")]
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private RawImage videoOutputImage; // La imagen en el Canvas donde se ve el video
    [SerializeField] private VideoClip videoToPlay;

    [Header("Configuración de Movimiento")]
    [SerializeField] private GameObject playerObject;
    [SerializeField] private Transform teleportDestination; // Arrastra aquí un objeto vacío donde quieras que aparezca el player

    [Header("Eventos")]
    [Tooltip("Cosas que pasan al INICIO (ej: desactivar movimiento del player, pausar música)")]
    public UnityEvent OnCinematicStart;

    [Tooltip("Cosas que pasan al FINAL (ej: reactivar movimiento, abrir puertas, guardar partida)")]
    public UnityEvent OnCinematicEnd;

    private bool hasPlayed = false; // Para asegurar que solo pase una vez

    private void Start()
    {
        // Asegurarnos de que la imagen del video esté apagada al inicio
        if (videoOutputImage != null)
            videoOutputImage.enabled = false;

        // Suscribirse al evento de fin de video
        if (videoPlayer != null)
            videoPlayer.loopPointReached += OnVideoFinished;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Verificamos si es el player y si no se ha reproducido ya
        if (other.CompareTag("Player") && !hasPlayed)
        {
            hasPlayed = true;
            StartCoroutine(PlayCinematicSequence());
        }
    }

    private IEnumerator PlayCinematicSequence()
    {
        // 1. Disparar eventos de inicio (ej: quitar control al player)
        OnCinematicStart.Invoke();

        // 2. Preparar visuales
        if (videoPlayer != null && videoToPlay != null)
        {
            videoPlayer.clip = videoToPlay;
            videoPlayer.Prepare();

            // Esperar a que el video esté listo para evitar pantallazos negros
            while (!videoPlayer.isPrepared)
            {
                yield return null;
            }

            // 3. Mostrar imagen y reproducir
            if (videoOutputImage != null) videoOutputImage.enabled = true;
            videoPlayer.Play();
        }
        else
        {
            Debug.LogError("Falta asignar el VideoPlayer o el VideoClip en el inspector.");
            OnVideoFinished(videoPlayer); // Forzar finalización si hay error
        }
    }

    // Este método se llama automáticamente cuando el video termina
    private void OnVideoFinished(VideoPlayer vp)
    {
        StartCoroutine(FinishSequence());
    }

    private IEnumerator FinishSequence()
    {
        // Ocultar la pantalla de video
        if (videoOutputImage != null)
            videoOutputImage.enabled = false;

        // 4. Teletransportar al Player
        TeleportPlayer();

        // Pequeña espera para asegurar que las físicas se asienten (opcional)
        yield return new WaitForSeconds(0.1f);

        // 5. Disparar eventos finales (ej: devolver control, activar boss, etc.)
        OnCinematicEnd.Invoke();
    }

    private void TeleportPlayer()
    {
        if (playerObject == null || teleportDestination == null) return;

        // NOTA IMPORTANTE: Si usas CharacterController, debes desactivarlo antes de moverlo
        CharacterController cc = playerObject.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;

        // Mover el objeto
        playerObject.transform.position = teleportDestination.position;
        playerObject.transform.rotation = teleportDestination.rotation;

        // Reactivar CharacterController
        if (cc != null) cc.enabled = true;

        Debug.Log("Player teletransportado a: " + teleportDestination.name);
    }

    private void OnDestroy()
    {
        if (videoPlayer != null)
            videoPlayer.loopPointReached -= OnVideoFinished;
    }
}