using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

[ExecuteInEditMode]
public class SceneSetup : MonoBehaviour
{
    [System.Serializable]
    public struct RankData
    {
        public string RankName;
        public int RequiredScore;
        public Sprite RankSprite;
        public Sprite LarvaSprite;
        public bool LarvaJumpRank;
    }

    public static SceneSetup Instance { get; private set; }

    [Header("Configuración de Niebla")]
    [SerializeField] private Color levelFogColor = new Color(166, 68, 68);
    [SerializeField] private Material fog;

    [Header("Configuración de Ranking por Niveles")]
    [Tooltip("Define los rangos ordenados de MAYOR a MENOR puntaje requerido.")]
    [SerializeField] private RankData[] ranks;

    [Header("Audio Global de Música")]
    [Tooltip("Todos estos clips se reproducirán simultáneamente y en bucle.")]
    public AudioClip[] backgroundMusicClips;

    public RankData[] Ranks => ranks;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (Application.isPlaying)
        {
            if (backgroundMusicClips != null && backgroundMusicClips.Length > 0)
            {
                // Usar AudioManager en lugar de crear AudioSources locales
                AudioManager.PlayBackgroundMusic(backgroundMusicClips);
            }
        }
    }

    void OnEnable()
    {
        if (Application.isPlaying && fog != null)
        {
            fog.SetColor("_colorniebla", levelFogColor);
        }
    }

    // Métodos estáticos que delegan al AudioManager
    public static void StopBackgroundMusic()
    {
        AudioManager.StopBackgroundMusic();
    }

    public static void PauseBackgroundMusic()
    {
        AudioManager.PauseBackgroundMusic();
    }

    public static void ResumeBackgroundMusic()
    {
        AudioManager.ResumeBackgroundMusic();
    }
}