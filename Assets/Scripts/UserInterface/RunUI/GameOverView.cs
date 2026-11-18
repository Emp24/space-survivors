using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Shown when the player dies: run summary, personal bests, retry / main menu.
public class GameOverView : MonoBehaviour
{
    private GameObject panel;
    private TMP_Text statsText;
    private TMP_Text bestText;

    void Awake()
    {
        Canvas canvas = RuntimeUI.CreateCanvas(transform, "GameOverCanvas", 100);

        Image dim = RuntimeUI.CreateImage(canvas.transform, "GameOverPanel", new Color(0.02f, 0.02f, 0.08f, 0.85f));
        RuntimeUI.Stretch(dim.rectTransform);
        panel = dim.gameObject;

        TMP_Text title = RuntimeUI.CreateText(panel.transform, "Title", "SHIP DESTROYED", 96, TextAlignmentOptions.Center);
        title.rectTransform.anchoredPosition = new Vector2(0, 260);
        title.rectTransform.sizeDelta = new Vector2(1400, 140);
        title.fontStyle = FontStyles.Bold;
        title.color = new Color(1f, 0.35f, 0.35f);

        statsText = RuntimeUI.CreateText(panel.transform, "Stats", "", 44, TextAlignmentOptions.Center);
        statsText.rectTransform.anchoredPosition = new Vector2(0, 70);
        statsText.rectTransform.sizeDelta = new Vector2(1200, 240);

        bestText = RuntimeUI.CreateText(panel.transform, "Best", "", 32, TextAlignmentOptions.Center);
        bestText.rectTransform.anchoredPosition = new Vector2(0, -90);
        bestText.rectTransform.sizeDelta = new Vector2(1200, 60);

        RuntimeUI.CreateButton(panel.transform, "RetryButton", "Retry  [R]", new Vector2(-200, -220), Retry);
        RuntimeUI.CreateButton(panel.transform, "MenuButton", "Main Menu", new Vector2(200, -220), GoToMainMenu);

        panel.SetActive(false);
        GameManager.instance.OnGameStateChanged += HandleGameStateChanged;
    }

    void OnDestroy()
    {
        if (GameManager.instance != null)
            GameManager.instance.OnGameStateChanged -= HandleGameStateChanged;
    }

    void Update()
    {
        if (panel.activeSelf && Input.GetKeyDown(KeyCode.R)) Retry();
    }

    private void HandleGameStateChanged(GameState state)
    {
        if (state == GameState.GameOver) Show();
    }

    private void Show()
    {
        Time.timeScale = 0f;
        RunStats stats = RunStats.Instance;
        PlayerController player = FindAnyObjectByType<PlayerController>();

        int level = player != null ? player.playerLevel : 1;
        statsText.text =
            $"Survived  <b>{RunStats.FormatTime(stats.TimeSurvived)}</b>\n" +
            $"Kills  <b>{stats.Kills}</b>   <size=80%>(elites {stats.EliteKills})</size>\n" +
            $"Level  <b>{level}</b>";

        bestText.text = stats.IsNewBestTime
            ? "<color=#FFD54A>NEW BEST TIME!</color>"
            : $"Best time {RunStats.FormatTime(stats.BestTime)}   ·   Best kills {stats.BestKills}";

        panel.SetActive(true);
    }

    // State is set directly (without the event) so stale scene listeners don't trigger a second load.
    private void Retry()
    {
        Time.timeScale = 1f;
        GameManager.instance.gameState = GameState.StartGame;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void GoToMainMenu()
    {
        Time.timeScale = 1f;
        GameManager.instance.gameState = GameState.MainMenu;
        SceneManager.LoadScene("MainMenu");
    }
}
