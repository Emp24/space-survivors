using UnityEngine;
using UnityEngine.UI;

public class PauseMenuView : MonoBehaviour
{
    public GameObject pauseMenuPanel;
    public Button resumeButton;
    public Button exitButton;
    public void Awake()
    {
        GameManager.instance.OnGameStateChanged += HandleGameStateChanged;
        resumeButton.onClick.AddListener(() => { GameManager.instance.ChangeGameState(GameState.Playing); });
        exitButton.onClick.AddListener(() => { GameManager.instance.ChangeGameState(GameState.MainMenu); });
        pauseMenuPanel = GameObject.Find("PauseMenu");
        Debug.Log(pauseMenuPanel);
        pauseMenuPanel.SetActive(false);
    }
    public void OnDestroy()
    {
        GameManager.instance.OnGameStateChanged -= HandleGameStateChanged;
    }
    public void PauseGame()
    {
        Time.timeScale = 0;
        pauseMenuPanel.gameObject.SetActive(true);
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
