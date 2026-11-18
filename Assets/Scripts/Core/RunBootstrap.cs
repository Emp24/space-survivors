using UnityEngine;
using UnityEngine.SceneManagement;

// Spawns the run-level systems (stats, run HUD, game over screen, damage numbers, camera shake)
// into any scene that contains a player, so they work without hand-wiring the scene.
public static class RunBootstrap
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Initialize()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
        SetUpRunSystems();
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode) => SetUpRunSystems();

    private static void SetUpRunSystems()
    {
        if (Object.FindAnyObjectByType<RunStats>() != null) return;

        PlayerController player = Object.FindAnyObjectByType<PlayerController>();
        if (player == null) return;

        GameObject systems = new GameObject("RunSystems");
        systems.AddComponent<RunStats>();
        systems.AddComponent<CameraShake>();
        systems.AddComponent<DamageNumbers>();
        systems.AddComponent<RunHUDView>().Initialize(player);
        systems.AddComponent<GameOverView>();
    }
}
