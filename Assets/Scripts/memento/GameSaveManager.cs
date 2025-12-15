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

    // NUEVO: Método para buscar las referencias del jugador
    void FindPlayerReferences()
    {
        // Primero intentar encontrar el GameObject del jugador
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            playerMovement = player.GetComponent<PlayerMovement>();
            playerHealth = player.GetComponent<PlayerHealth>();
            weaponManager = player.GetComponent<WeaponManager>();
        }
        else
        {
            // Si no hay tag, usar FindObjectOfType
            playerMovement = FindObjectOfType<PlayerMovement>();
            playerHealth = FindObjectOfType<PlayerHealth>();
            weaponManager = FindObjectOfType<WeaponManager>();
        }

        // Validar que se encontraron todas las referencias
        if (playerMovement == null)
        {
            Debug.LogError("GameSaveManager: No se encontró PlayerMovement en la escena");
            Debug.Log("Asegúrate de que el prefab del jugador está instanciado en la escena");
        }

        if (playerHealth == null)
        {
            Debug.LogError("GameSaveManager: No se encontró PlayerHealth en la escena");
            Debug.Log("Asegúrate de que el GameObject del jugador tiene el componente PlayerHealth");
        }

        if (weaponManager == null)
        {
            Debug.LogError("GameSaveManager: No se encontró WeaponManager en la escena");
            Debug.Log("Asegúrate de que el GameObject del jugador tiene el componente WeaponManager");
        }
    }

    void SaveGame()
    {
        // Validar referencias antes de guardar
        if (playerMovement == null || playerHealth == null || weaponManager == null)
        {
            Debug.LogWarning("Referencias del jugador no encontradas, intentando buscar...");
            FindPlayerReferences();

            // Si aún son null, mostrar error y salir
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
            // Asegurarse de tener referencias antes de restaurar
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
        Debug.Log($"Cargando escena: {memento.sceneName}");
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(memento.sceneName);

        // Esperar a que la escena se cargue completamente
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        // Pequeña espera para asegurar que todos los objetos se instanciaron
        yield return new WaitForSeconds(0.1f);

        // Buscar referencias después de cargar la escena
        FindPlayerReferences();

        if (playerMovement != null && playerHealth != null && weaponManager != null)
        {
            Debug.Log("Referencias encontradas, restaurando juego...");
            RestoreGame(memento);
        }
        else
        {
            Debug.LogError("No se pudieron encontrar las referencias del jugador después de cargar la escena");
            Debug.Log("Asegúrate de que el prefab del jugador se instancia en la escena " + memento.sceneName);
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

    //  NUEVO: Método para probar desde el editor
    [ContextMenu("Buscar Referencias del Jugador")]
    void FindReferencesEditor()
    {
        FindPlayerReferences();

        if (playerMovement != null && playerHealth != null && weaponManager != null)
        {
            Debug.Log(" Todas las referencias encontradas correctamente");
        }
    }
}