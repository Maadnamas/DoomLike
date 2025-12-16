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

    private System.Collections.Generic.List<AudioSource> backgroundSources = new System.Collections.Generic.List<AudioSource>();

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
                SetupBackgroundMusic();
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

    private void SetupBackgroundMusic()
    {
        foreach (AudioClip clip in backgroundMusicClips)
        {
            if (clip != null)
            {
                AudioSource source = gameObject.AddComponent<AudioSource>();
                source.clip = clip;
                source.loop = true;
                source.spatialBlend = 0f;
                source.volume = 1f;
                source.Play();
                backgroundSources.Add(source);
            }
        }
    }

    public static void StopBackgroundMusic()
    {
        if (Instance != null)
        {
            foreach (AudioSource source in Instance.backgroundSources)
            {
                if (source != null && source.isPlaying)
                {
                    source.Stop();
                }
            }
        }
    }

    public static void PauseBackgroundMusic()
    {
        if (Instance != null)
        {
            foreach (AudioSource source in Instance.backgroundSources)
            {
                if (source != null && source.isPlaying) 
                {
                    source.Pause();
                }
            }
        }
    }

    public static void ResumeBackgroundMusic()
    {
        if (Instance != null)
        {
            foreach (AudioSource source in Instance.backgroundSources)
            {

                if (source != null && !source.isPlaying)
                {
                    source.UnPause();
                }
            }
        }
    }
}