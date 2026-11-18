using UnityEngine;

// Tracks the current run (time survived, kills) and persists personal bests.
public class RunStats : MonoBehaviour
{
    private const string BestTimeKey = "BestSurvivalTime";
    private const string BestKillsKey = "BestKills";

    public static RunStats Instance { get; private set; }

    public float TimeSurvived { get; private set; }
    public int Kills { get; private set; }
    public int EliteKills { get; private set; }
    public float BestTime => PlayerPrefs.GetFloat(BestTimeKey, 0f);
    public int BestKills => PlayerPrefs.GetInt(BestKillsKey, 0);
    public bool IsNewBestTime { get; private set; }
    public bool IsRunOver { get; private set; }

    void Awake()
    {
        Instance = this;
        GameEvents.OnEnemyKilled += HandleEnemyKilled;
        GameEvents.OnPlayerDied += HandlePlayerDied;
    }

    void OnDestroy()
    {
        GameEvents.OnEnemyKilled -= HandleEnemyKilled;
        GameEvents.OnPlayerDied -= HandlePlayerDied;
        if (Instance == this) Instance = null;
    }

    void Update()
    {
        // Scaled delta time, so pause and the power menu don't count towards the run.
        if (!IsRunOver) TimeSurvived += Time.deltaTime;
    }

    private void HandleEnemyKilled(Vector3 position, bool isElite)
    {
        if (IsRunOver) return;
        Kills++;
        if (isElite) EliteKills++;
    }

    private void HandlePlayerDied()
    {
        if (IsRunOver) return;
        IsRunOver = true;
        IsNewBestTime = TimeSurvived > BestTime;
        if (IsNewBestTime) PlayerPrefs.SetFloat(BestTimeKey, TimeSurvived);
        if (Kills > BestKills) PlayerPrefs.SetInt(BestKillsKey, Kills);
        PlayerPrefs.Save();
    }

    public static string FormatTime(float seconds)
    {
        int total = Mathf.FloorToInt(seconds);
        return $"{total / 60:00}:{total % 60:00}";
    }
}
