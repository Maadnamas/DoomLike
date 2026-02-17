using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameSaveManager : MonoBehaviour
{
    private PlayerMovement playerMovement;
    private PlayerHealth playerHealth;
    private WeaponManager weaponManager;

    void Start()
    {
        FindPlayerReferences();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F5))
            SaveGame();

        if (Input.GetKeyDown(KeyCode.F8))
            LoadGame();
    }

    void FindPlayerReferences()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            playerMovement = player.GetComponent<PlayerMovement>();
            playerHealth = player.GetComponent<PlayerHealth>();

            weaponManager = player.GetComponent<WeaponManager>();
            if (weaponManager == null)
            {
                weaponManager = player.GetComponentInChildren<WeaponManager>();
            }
        }
        else
        {
            playerMovement = FindObjectOfType<PlayerMovement>();
            playerHealth = FindObjectOfType<PlayerHealth>();
            weaponManager = FindObjectOfType<WeaponManager>();
        }

        if (playerMovement == null)
        {
            Debug.LogError("GameSaveManager: PlayerMovement not found in scene");
        }

        if (playerHealth == null)
        {
            Debug.LogError("GameSaveManager: PlayerHealth not found in scene");
        }

        if (weaponManager == null)
        {
            Debug.LogError("GameSaveManager: WeaponManager not found in scene");
        }
    }

    void SaveGame()
    {
        if (playerMovement == null || playerHealth == null || weaponManager == null)
        {
            Debug.LogWarning("Player references not found, attempting to search...");
            FindPlayerReferences();

            if (playerMovement == null || playerHealth == null || weaponManager == null)
            {
                Debug.LogError("Cannot save: player references are missing");
                return;
            }
        }

        var memento = playerMovement.SaveState(playerHealth, weaponManager);

        if (memento != null)
        {
            PlayerSaveSystem.Save(memento);
            Debug.Log("Game saved successfully");
        }
        else
        {
            Debug.LogError("Error creating memento for saving");
        }
    }

    void LoadGame()
    {
        var memento = PlayerSaveSystem.Load();
        if (memento == null)
        {
            Debug.LogWarning("No save data found to load");
            return;
        }

        string currentScene = SceneManager.GetActiveScene().name;

        if (memento.sceneName != currentScene)
        {
            StartCoroutine(LoadSceneAndRestore(memento));
        }
        else
        {
            if (playerMovement == null || playerHealth == null || weaponManager == null)
            {
                Debug.Log("Searching for player references...");
                FindPlayerReferences();
            }

            RestoreGame(memento);
        }
    }

    IEnumerator LoadSceneAndRestore(PlayerMemento memento)
    {
        Debug.Log("Loading scene: " + memento.sceneName);
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(memento.sceneName);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        yield return new WaitForSeconds(0.1f);

        FindPlayerReferences();

        if (playerMovement != null && playerHealth != null && weaponManager != null)
        {
            Debug.Log("References found, restoring game...");
            RestoreGame(memento);
        }
        else
        {
            Debug.LogError("Could not find player references after loading the scene");
        }
    }

    void RestoreGame(PlayerMemento memento)
    {
        if (playerMovement == null || playerHealth == null || weaponManager == null)
        {
            Debug.LogError("Cannot restore data: null references");
            return;
        }

        playerMovement.LoadState(memento, playerHealth, weaponManager);
        Debug.Log("Game loaded successfully");
    }

    [ContextMenu("Find Player References")]
    void FindReferencesEditor()
    {
        FindPlayerReferences();

        if (playerMovement != null && playerHealth != null && weaponManager != null)
        {
            Debug.Log("All references found successfully");
        }
    }
}