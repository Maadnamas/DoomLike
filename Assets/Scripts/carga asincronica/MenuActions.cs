using UnityEngine;

public class MenuActions : MonoBehaviour
{
    private AsyncSceneLoader loader;
    private FadeController fade;

    public void Awake()
    {
        Cursor.lockState = CursorLockMode.None;
    }
    public void LoadScene(string sceneName)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
