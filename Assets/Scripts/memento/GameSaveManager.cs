using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameSaveManager : MonoBehaviour
{
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private WeaponManager weaponManager;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F5))
            SaveGame();

        if (Input.GetKeyDown(KeyCode.F8))
            LoadGame();
    }

    void SaveGame()
    {
        var memento = playerMovement.SaveState(playerHealth, weaponManager);
        PlayerSaveSystem.Save(memento);
    }

    void LoadGame()
    {
        var memento = PlayerSaveSystem.Load();
        if (memento == null) return;

        string currentScene = SceneManager.GetActiveScene().name;

        if (memento.sceneName != currentScene)
        {
            StartCoroutine(LoadSceneAndRestore(memento));
        }
        else
        {
            RestoreGame(memento);
        }
    }

    IEnumerator LoadSceneAndRestore(PlayerMemento memento)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(memento.sceneName);
        yield return new WaitUntil(() => asyncLoad.isDone);

        playerMovement = FindObjectOfType<PlayerMovement>();
        playerHealth = FindObjectOfType<PlayerHealth>();
        weaponManager = FindObjectOfType<WeaponManager>();

        RestoreGame(memento);
    }

    void RestoreGame(PlayerMemento memento)
    {
        playerMovement.LoadState(memento, playerHealth, weaponManager);
    }
}