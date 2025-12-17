using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    static AudioManager instance;
    static AudioSource sfx2DSource;
    static AudioSource sfxLongSource; // Nuevo AudioSource para SFX largos detenibles

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

            // AudioSource para SFX cortos (PlayOneShot)
            sfx2DSource = gameObject.AddComponent<AudioSource>();
            sfx2DSource.spatialBlend = 0f;

            // AudioSource para SFX largos detenibles (como el contador)
            sfxLongSource = gameObject.AddComponent<AudioSource>();
            sfxLongSource.spatialBlend = 0f;
            sfxLongSource.loop = false;
            sfxLongSource.playOnAwake = false;

            if (instance.sfxMixerGroup != null)
            {
                sfx2DSource.outputAudioMixerGroup = instance.sfxMixerGroup;
                sfxLongSource.outputAudioMixerGroup = instance.sfxMixerGroup;
            }
        }
        else Destroy(gameObject);
    }

    // Para SFX cortos, no detenibles (el uso principal)
    public static void PlaySFX2D(AudioClip clip)
    {
        if (instance == null || clip == null) return;
        sfx2DSource.PlayOneShot(clip);
    }

    // Nuevo método para SFX largos o detenibles
    public static void PlaySFXLong(AudioClip clip)
    {
        if (instance == null || clip == null || sfxLongSource == null) return;

        sfxLongSource.clip = clip;
        sfxLongSource.Play();
    }

    // Nuevo método para detener el SFX largo
    public static void StopSFXLong()
    {
        if (instance == null || sfxLongSource == null) return;
        if (sfxLongSource.isPlaying)
        {
            sfxLongSource.Stop();
        }
    }

    public static void PlaySFX3D(AudioClip clip, Vector3 pos)
    {
        if (instance == null || clip == null) return;
        GameObject temp = new GameObject("3D_SFX");
        temp.transform.position = pos;
        AudioSource src = temp.AddComponent<AudioSource>();
        src.clip = clip;
        src.spatialBlend = 1f;
        src.minDistance = 1f;
        src.maxDistance = 25f;
        src.rolloffMode = AudioRolloffMode.Linear;

        if (instance.sfxMixerGroup != null)
        {
            src.outputAudioMixerGroup = instance.sfxMixerGroup;
        }

        src.Play();
        Object.Destroy(temp, clip.length);
    }

    public static void PlayBackgroundMusic(AudioClip[] clips)
    {
        if (instance == null || clips == null || clips.Length == 0) return;

        StopBackgroundMusic();

        foreach (AudioClip clip in clips)
        {
            if (clip != null)
            {
                AudioSource source = instance.gameObject.AddComponent<AudioSource>();
                source.clip = clip;
                source.loop = true;
                source.spatialBlend = 0f;
                source.volume = 1f;

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