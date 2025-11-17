using UnityEngine;

public class AudioManager : MonoBehaviour
{
    static AudioManager instance;
    static AudioSource sfx2DSource;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            sfx2DSource = gameObject.AddComponent<AudioSource>();
            sfx2DSource.spatialBlend = 0f; // 2D
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

        src.Play();
        Object.Destroy(temp, clip.length);
    }
}
