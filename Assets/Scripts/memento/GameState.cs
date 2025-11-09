using UnityEngine;

public class GameState : MonoBehaviour
{
    public Transform player;
    public int playerHealth = 100;
    public int playerAmmo = 10;

    public string currentLevel = "Level_01";

    public GameMemento CreateMemento()
    {
        return new GameMemento(player.position, playerHealth, playerAmmo, currentLevel);
    }
    public void RestoreFromMemento(GameMemento memento)
    {
        player.position = memento.playerPosition;
        playerHealth = memento.playerHealth;
        playerAmmo = memento.playerAmmo;
        currentLevel = memento.currentLevel;
    }
}
