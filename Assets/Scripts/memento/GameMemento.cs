using UnityEngine;

[System.Serializable]
public class GameMemento
{
    public Vector3 playerPosition;
    public int playerHealth;
    public int playerAmmo;
    public string currentLevel;

    public GameMemento(Vector3 pos, int hp, int ammo, string level)
    {
        playerPosition = pos;
        playerHealth = hp;
        playerAmmo = ammo;
        currentLevel = level;
    }
}