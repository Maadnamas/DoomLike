public static class GameEvents
{
    // Sistema de armas
    public const string WEAPON_FIRED = "WEAPON_FIRED";
    public const string WEAPON_SWITCHED = "WEAPON_SWITCHED";
    public const string AMMO_PICKED_UP = "AMMO_PICKED_UP";
    public const string AMMO_CHANGED = "AMMO_CHANGED";

    // Sistema de vida
    public const string PLAYER_DAMAGED = "PLAYER_DAMAGED";
    public const string PLAYER_HEALED = "PLAYER_HEALED";
    public const string PLAYER_DIED = "PLAYER_DIED";
    public const string ENEMY_DIED = "ENEMY_DIED";

    // Sistema de progreso
    public const string LEVEL_STARTED = "LEVEL_STARTED";
    public const string LEVEL_COMPLETED = "LEVEL_COMPLETED";
    public const string CHECKPOINT_REACHED = "CHECKPOINT_REACHED";

    // UI
    public const string UI_UPDATE_HEALTH = "UI_UPDATE_HEALTH";
    public const string UI_UPDATE_AMMO = "UI_UPDATE_AMMO";

    // 🔥 NUEVOS EVENTOS para ScreenManager
    public const string GAME_START = "GAME_START";
    public const string GAME_OVER = "GAME_OVER";
    public const string GAME_VICTORY = "GAME_VICTORY";
}