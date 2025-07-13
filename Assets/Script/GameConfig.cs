using UnityEngine;

public class GameConfig
{
    // UI settings
    public const string MAIN_MENU_SCENE = "MainMenu";
    public const string GAME_SCENE = "GamePlay"; // Replace with your actual game scene name

}

[System.Flags]
public enum PlayerRole
{
    None = 0,
    PLAYER1 = 1 << 0, // 1
    PLAYER2 = 1 << 1, // 2
    ALL = PLAYER1 | PLAYER2 // 3
}
