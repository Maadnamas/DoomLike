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
            Debug.LogError("GameSaveManager: No se encontro PlayerMovement en la escena");
        }

        if (playerHealth == null)
        {
            Debug.LogError("GameSaveManager: No se encontro PlayerHealth en la escena");
        }

        if (weaponManager == null)
        {
            Debug.LogError("GameSaveManager: No se encontro WeaponManager en la escena");
        }
    }

    void SaveGame()
    {
        if (playerMovement == null || playerHealth == null || weaponManager == null)
        {
            Debug.LogWarning("Referencias del jugador no encontradas, intentando buscar...");
            FindPlayerReferences();

            if (playerMovement == null || playerHealth == null || weaponManager == null)
            {
                Debug.LogError("No se puede guardar: faltan referencias del jugador");
                return;
            }
        }

        var memento = playerMovement.SaveState(playerHealth, weaponManager);

        if (memento != null)
        {
            PlayerSaveSystem.Save(memento);
            Debug.Log("Juego guardado correctamente");
        }
        else
        {
            Debug.LogError("Error al crear el memento para guardar");
        }
    }

    void LoadGame()
    {
        var memento = PlayerSaveSystem.Load();
        if (memento == null)
        {
            Debug.LogWarning("No hay datos guardados para cargar");
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
                Debug.Log("Buscando referencias del jugador...");
                FindPlayerReferences();
            }

            RestoreGame(memento);
        }
    }

    IEnumerator LoadSceneAndRestore(PlayerMemento memento)
    {
        Debug.Log("Cargando escena: " + memento.sceneName);
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(memento.sceneName);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        yield return new WaitForSeconds(0.1f);

        FindPlayerReferences();

        if (playerMovement != null && playerHealth != null && weaponManager != null)
        {
            Debug.Log("Referencias encontradas, restaurando juego...");
            RestoreGame(memento);
        }
        else
        {
            Debug.LogError("No se pudieron encontrar las referencias del jugador despues de cargar la escena");
        }
    }

    void RestoreGame(PlayerMemento memento)
    {
        if (playerMovement == null || playerHealth == null || weaponManager == null)
        {
            Debug.LogError("No se pueden restaurar los datos: referencias nulas");
            return;
        }

        playerMovement.LoadState(memento, playerHealth, weaponManager);
        Debug.Log("Juego cargado correctamente");
    }

    [ContextMenu("Buscar Referencias del Jugador")]
    void FindReferencesEditor()
    {
        FindPlayerReferences();

        if (playerMovement != null && playerHealth != null && weaponManager != null)
        {
            Debug.Log("Todas las referencias encontradas correctamente");
        }
    }
}