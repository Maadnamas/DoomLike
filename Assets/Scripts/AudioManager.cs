using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    static AudioManager instance;
    static AudioSource sfx2DSource;

    [Header("Audio Mixer Settings")]
    [SerializeField] AudioMixerGroup sfxMixerGroup;
    [SerializeField] AudioMixerGroup musicMixerGroup;

    private System.Collections.Generic.List<AudioSource> musicSources = new System.Collections.Generic.List<AudioSource>();

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            sfx2DSource = gameObject.AddComponent<AudioSource>();
            sfx2DSource.spatialBlend = 0f; // 2D

            // Asignar el mixer group al AudioSource 2D
            if (instance.sfxMixerGroup != null)
            {
                sfx2DSource.outputAudioMixerGroup = instance.sfxMixerGroup;
            }
        }
        else Destroy(gameObject);
    }

    public static void PlaySFX2D(AudioClip clip)
    {
        if (instance == null || clip == null) return;
        sfx2DSource.PlayOneShot(clip);
    }

    public static void PlaySFX3D(AudioClip clip, Vector3 pos)
    {
        if (instance == null || clip == null) return;
        GameObject temp = new GameObject("3D_SFX");
        temp.transform.position = pos;
        AudioSource src = temp.AddComponent<AudioSource>();
        src.clip = clip;
        src.spatialBlend = 1f;         // 100% 3d
        src.minDistance = 1f;
        src.maxDistance = 25f;
        src.rolloffMode = AudioRolloffMode.Linear;

        // Asignar el mixer group al AudioSource 3D
        if (instance.sfxMixerGroup != null)
        {
            src.outputAudioMixerGroup = instance.sfxMixerGroup;
        }

        src.Play();
        Object.Destroy(temp, clip.length);
    }

    // ===== NUEVOS MÉTODOS PARA MÚSICA =====

    public static void PlayBackgroundMusic(AudioClip[] clips)
    {
        if (instance == null || clips == null || clips.Length == 0) return;

        // Detener música anterior si existe
        StopBackgroundMusic();

        // Crear AudioSources para cada clip
        foreach (AudioClip clip in clips)
        {
            if (clip != null)
            {
                AudioSource source = instance.gameObject.AddComponent<AudioSource>();
                source.clip = clip;
                source.loop = true;
                source.spatialBlend = 0f;
                source.volume = 1f;

                // Asignar el mixer group de música
                if (instance.musicMixerGroup != null)
                {
                    source.outputAudioMixerGroup = instance.musicMixerGroup;
                }

                source.Play();
                instance.musicSources.Add(source);
            }
        }
    }

    public static void StopBackgroundMusic()
    {
        if (instance == null) return;

        foreach (AudioSource source in instance.musicSources)
        {
            if (source != null)
            {
                source.Stop();
                Destroy(source);
            }
        }
        instance.musicSources.Clear();
    }

    public static void PauseBackgroundMusic()
    {
        if (instance == null) return;

        foreach (AudioSource source in instance.musicSources)
        {
            if (source != null && source.isPlaying)
            {
                source.Pause();
            }
        }
    }

    public static void ResumeBackgroundMusic()
    {
        if (instance == null) return;

        foreach (AudioSource source in instance.musicSources)
        {
            if (source != null && !source.isPlaying)
            {
                source.UnPause();
            }
        }
    }
}