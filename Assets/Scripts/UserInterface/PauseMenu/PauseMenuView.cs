using UnityEngine;
using UnityEngine.UI;

public class PauseMenuView : MonoBehaviour
{
    public GameObject pauseMenuPanel;
    public Button resumeButton;
    public void Awake()
    {
        GameManager.instance.OnGameStateChanged += HandleGameStateChanged;
        resumeButton.onClick.AddListener(() => { GameManager.instance.ChangeGameState(GameState.Playing); });
    }
    public void PauseGame()
    {
        Time.timeScale = 0;
        pauseMenuPanel.SetActive(true);
    }
    public void ResumeGame()
    {
        Time.timeScale = 1;
        pauseMenuPanel.SetActive(false);
    }
    public void HandleGameStateChanged(GameState state)
    {
        switch (state)
        {
            case GameState.Pause:
                PauseGame();
                break;
            case GameState.Playing:
                ResumeGame();
                break;
        }
    }
}
