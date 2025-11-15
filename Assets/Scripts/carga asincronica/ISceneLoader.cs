using System.Collections;
public interface ISceneLoader
{
    IEnumerator LoadScene(string sceneName, float minimumWaitSeconds, IFade fade, float fadeDuration);
}
