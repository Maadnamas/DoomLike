using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FadeController : MonoBehaviour, IFade
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private Image blackImage;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        if (canvas != null)
        {
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        }
        if (blackImage != null)
        {
            Color c = blackImage.color;
            c.a = 0f;
            blackImage.color = c;
        }
    }

    public IEnumerator FadeOut(float duration)
    {
        if (blackImage == null)
            yield break;

        float t = 0f;
        Color c = blackImage.color;
        float start = c.a;
        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            float a = Mathf.Lerp(start, 1f, t / duration);
            c.a = a;
            blackImage.color = c;
            yield return null;
        }
        c.a = 1f;
        blackImage.color = c;
    }

    public IEnumerator FadeIn(float duration)
    {
        if (blackImage == null)
            yield break;

        float t = 0f;
        Color c = blackImage.color;
        float start = c.a;
        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            float a = Mathf.Lerp(start, 0f, t / duration);
            c.a = a;
            blackImage.color = c;
            yield return null;
        }
        c.a = 0f;
        blackImage.color = c;
    }
}