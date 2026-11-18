using System;
using UnityEngine;

// Lightweight static event bus for gameplay moments that several unrelated systems care about
// (run stats, damage numbers, camera shake...). Publishers don't know who is listening.
public static class GameEvents
{
    // Position of the enemy, and whether it was an elite.
    public static event Action<Vector3, bool> OnEnemyKilled;
    // Position and amount of damage an enemy received.
    public static event Action<Vector3, float> OnEnemyDamaged;
    // Amount of damage the player received.
    public static event Action<float> OnPlayerDamaged;
    public static event Action OnPlayerDied;

    public static void EnemyKilled(Vector3 position, bool isElite) => OnEnemyKilled?.Invoke(position, isElite);
    public static void EnemyDamaged(Vector3 position, float amount) => OnEnemyDamaged?.Invoke(position, amount);
    public static void PlayerDamaged(float amount) => OnPlayerDamaged?.Invoke(amount);
    public static void PlayerDied() => OnPlayerDied?.Invoke();

    // Static state survives play-mode restarts when domain reload is disabled, so clear it explicitly.
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetSubscribers()
    {
        OnEnemyKilled = null;
        OnEnemyDamaged = null;
        OnPlayerDamaged = null;
        OnPlayerDied = null;
    }
}
