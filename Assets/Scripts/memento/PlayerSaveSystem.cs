using UnityEngine;
using System.IO;
using System.Collections.Generic;

public static class PlayerSaveSystem
{
    private static string path = Application.persistentDataPath + "/player_save.json";

    public static void Save(PlayerMemento memento)
    {
        // Convertir a formato serializable para JSON
        var serializableData = new SerializablePlayerMemento(memento);
        string json = JsonUtility.ToJson(serializableData, true);
        File.WriteAllText(path, json);
        Debug.Log("Partida guardada en: " + path);
    }

    public static PlayerMemento Load()
    {
        if (!File.Exists(path))
        {
            Debug.LogWarning("No se encontró guardado en " + path);
            return null;
        }

        string json = File.ReadAllText(path);
        var serializableData = JsonUtility.FromJson<SerializablePlayerMemento>(json);
        return serializableData.ToPlayerMemento();
    }

    // Clase auxiliar para serialización de datos complejos
    [System.Serializable]
    private class SerializablePlayerMemento
    {
        public string sceneName;
        public SerializableVector3 position;
        public SerializableQuaternion playerRotation;
        public float cameraPitch;
        public int health;
        public List<PlayerMemento.WeaponData> weapons = new List<PlayerMemento.WeaponData>();
        public int equippedWeaponIndex;
        public List<SerializableKeyValuePair> additionalData = new List<SerializableKeyValuePair>();

        public SerializablePlayerMemento(PlayerMemento memento)
        {
            sceneName = memento.sceneName;
            position = new SerializableVector3(memento.position);
            playerRotation = new SerializableQuaternion(memento.playerRotation);
            cameraPitch = memento.cameraPitch;
            health = memento.health;
            weapons = memento.weapons;
            equippedWeaponIndex = memento.equippedWeaponIndex;

            // Convertir diccionario a lista serializable
            foreach (var kvp in memento.additionalData)
            {
                additionalData.Add(new SerializableKeyValuePair(kvp.Key, kvp.Value));
            }
        }

        public PlayerMemento ToPlayerMemento()
        {
            var memento = new PlayerMemento(
                sceneName,
                position.ToVector3(),
                playerRotation.ToQuaternion(),
                cameraPitch,
                health,
                weapons,
                equippedWeaponIndex
            );

            // Restaurar datos adicionales
            foreach (var item in additionalData)
            {
                memento.SetAdditionalData(item.key, item.value);
            }

            return memento;
        }
    }

    [System.Serializable]
    private class SerializableKeyValuePair
    {
        public string key;
        public object value;

        public SerializableKeyValuePair(string k, object v)
        {
            key = k;
            value = v;
        }
    }

    [System.Serializable]
    private class SerializableVector3
    {
        public float x, y, z;

        public SerializableVector3(Vector3 v)
        {
            x = v.x;
            y = v.y;
            z = v.z;
        }

        public Vector3 ToVector3()
        {
            return new Vector3(x, y, z);
        }
    }

    [System.Serializable]
    private class SerializableQuaternion
    {
        public float x, y, z, w;

        public SerializableQuaternion(Quaternion q)
        {
            x = q.x;
            y = q.y;
            z = q.z;
            w = q.w;
        }

        public Quaternion ToQuaternion()
        {
            return new Quaternion(x, y, z, w);
        }
    }
}