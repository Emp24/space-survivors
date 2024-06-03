using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuView : MonoBehaviour
{
    public Button startGame;

    public void Awake()
    {
        startGame.onClick.AddListener(() => { StartGame(); });
        GameManager.instance.OnGameStateChanged += HandleGameStateChanged;
    }

    public void StartGame()
    {
        GameManager.instance.ChangeGameState(GameState.StartGame);
    }
    public void HandleGameStateChanged(GameState state)
    {
        switch (state)
        {
            case GameState.StartGame:
                SceneManager.LoadScene("SampleScene");
                Time.timeScale = 1;
                break;
            case GameState.MainMenu:
                SceneManager.LoadScene("MainMenu");
                break;
        }

    }
}
