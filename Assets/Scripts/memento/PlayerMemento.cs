using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerMemento
{
    public string sceneName;
    public Vector3 position;
    public Quaternion playerRotation;
    public float cameraPitch;
    public int health;

    public List<WeaponData> weapons = new List<WeaponData>();
    public int equippedWeaponIndex;

    [System.Serializable]
    public class WeaponData
    {
        public int weaponID;
        public int currentAmmo;
    }

    public PlayerMemento(string scene, Vector3 pos, Quaternion rot, float pitch, int hp,
                         List<WeaponData> weaponList, int equippedIndex)
    {
        sceneName = scene;
        position = pos;
        playerRotation = rot;
        cameraPitch = pitch;
        health = hp;
        weapons = weaponList;
        equippedWeaponIndex = equippedIndex;
    }
}