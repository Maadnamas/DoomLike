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

    [Header("Fog Configuration")]
    [SerializeField] private Color levelFogColor = new Color(166, 68, 68);
    [SerializeField] private Material fog;

    [Header("Level Ranking Configuration")]
    [Tooltip("Define ranks ordered from HIGHEST to LOWEST required score.")]
    [SerializeField] private RankData[] ranks;

    [Header("Global Music Audio")]
    [Tooltip("All these clips will play simultaneously and in a loop.")]
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