using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class WeaponPickupManager : MonoBehaviour
{
    public List<WeaponPickup> weaponPickups = new List<WeaponPickup>();
    private Dictionary<string, bool> pickupCollectionState = new Dictionary<string, bool>();

    public static WeaponPickupManager Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        InitializePickups();
    }

    void InitializePickups()
    {
        // Buscar todos los pickups en la escena
        if (weaponPickups.Count == 0)
        {
            WeaponPickup[] foundPickups = FindObjectsOfType<WeaponPickup>(true);
            weaponPickups.AddRange(foundPickups);
        }

        // Inicializar todos como no recolectados
        foreach (WeaponPickup pickup in weaponPickups)
        {
            if (pickup != null)
            {
                string key = GetPickupKey(pickup);
                pickupCollectionState[key] = false;

                // Asegurar que estén activos al inicio
                pickup.gameObject.SetActive(true);
            }
        }
    }

    string GetPickupKey(WeaponPickup pickup)
    {
        return pickup.gameObject.GetInstanceID().ToString();
    }

    public void MarkPickupAsCollected(WeaponPickup pickup)
    {
        if (pickup == null) return;

        string key = GetPickupKey(pickup);
        pickupCollectionState[key] = true;
    }

    public void SavePickupStates(PlayerMemento memento)
    {
        List<string> collectedKeys = new List<string>();

        foreach (var kvp in pickupCollectionState)
        {
            if (kvp.Value) // true = recolectado
            {
                collectedKeys.Add(kvp.Key);
            }
        }

        string serializedKeys = string.Join("|", collectedKeys.ToArray());
        memento.SetAdditionalData("collectedPickups", serializedKeys);
    }

    public void LoadPickupStates(PlayerMemento memento)
    {
        string serializedKeys = memento.GetAdditionalData<string>("collectedPickups", "");

        // Resetear todos los estados a no recolectados
        foreach (var key in pickupCollectionState.Keys.ToList())
        {
            pickupCollectionState[key] = false;
        }

        // Marcar los que fueron recolectados según el guardado
        if (!string.IsNullOrEmpty(serializedKeys))
        {
            string[] collectedKeys = serializedKeys.Split('|');
            foreach (string key in collectedKeys)
            {
                if (pickupCollectionState.ContainsKey(key))
                {
                    pickupCollectionState[key] = true;
                }
            }
        }

        // Aplicar los estados a los GameObjects
        ApplyPickupStates();
    }

    void ApplyPickupStates()
    {
        foreach (WeaponPickup pickup in weaponPickups)
        {
            if (pickup != null)
            {
                string key = GetPickupKey(pickup);

                if (pickupCollectionState.ContainsKey(key))
                {
                    if (pickupCollectionState[key]) // Recolectado
                    {
                        pickup.gameObject.SetActive(false);
                    }
                    else // No recolectado
                    {
                        pickup.ReactivatePickup();
                    }
                }
            }
        }
    }
}