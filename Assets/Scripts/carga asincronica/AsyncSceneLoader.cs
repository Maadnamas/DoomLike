using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AsyncSceneLoader : MonoBehaviour, ISceneLoader
{
    [SerializeField] private float defaultFadeDuration = 0.5f;
    private bool skipRequested = false;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void ForceSkip()
    {
        skipRequested = true;
    }

    public IEnumerator LoadScene(string sceneName, float minimumWaitSeconds, IFade fade, float fadeDuration)
    {
        skipRequested = false;

        if (fade == null)
            fade = GetComponent<IFade>();
        if (fadeDuration <= 0f)
            fadeDuration = defaultFadeDuration;

        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        op.allowSceneActivation = false;

        float timer = 0f;
        while (timer < minimumWaitSeconds && !skipRequested)
        {
            timer += Time.unscaledDeltaTime;
            yield return null;
        }

        while (op.progress < 0.9f)
        {
            yield return null;
        }

        yield return fade.FadeOut(fadeDuration);

        op.allowSceneActivation = true;

        while (!op.isDone)
        {
            yield return null;
        }

        yield return fade.FadeIn(fadeDuration);
    }
}