using UnityEngine;
using System;
public enum GameState
{
    MainMenu,
    Playing,
    PowerMenu,
    GameOver,
    Pause,
    StartGame,
}
//Controls the flow of the game
public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public GameState gameState = GameState.MainMenu;

    public event Action<GameState> OnGameStateChanged;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void ChangeGameState(GameState state)
    {
        switch (state)
        {
            case GameState.MainMenu:
                gameState = GameState.MainMenu;
                break;
            case GameState.Playing:
                gameState = GameState.Playing;
                break;
            case GameState.PowerMenu:
                gameState = GameState.PowerMenu;
                break;
            case GameState.GameOver:
                gameState = GameState.GameOver;
                break;
            case GameState.Pause:
                gameState = GameState.Pause;
                break;
            case GameState.StartGame:
                gameState = GameState.StartGame;
                break;
        }
        OnGameStateChanged?.Invoke(gameState);
    }
}
