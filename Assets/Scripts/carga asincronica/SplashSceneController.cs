using System.Collections;
using UnityEngine;
public class SplashSceneController : MonoBehaviour
{
    [SerializeField] private string sceneToLoad = "Menu";
    [SerializeField] private float minimumWaitSeconds = 4f;
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private AsyncSceneLoader sceneLoader;
    [SerializeField] private FadeController fadeController;
    private void Start()
    {
        if (sceneLoader == null)
            sceneLoader = FindObjectOfType<AsyncSceneLoader>();
        if (fadeController == null)
            fadeController = FindObjectOfType<FadeController>();
        StartCoroutine(BeginSequence());
    }
    private IEnumerator BeginSequence()
    {
        yield return StartCoroutine(sceneLoader.LoadScene(sceneToLoad, minimumWaitSeconds, fadeController, fadeDuration));
    }
}
