using System;
using System.Collections.Generic;
using UnityEngine;

// A single draftable upgrade. Upgrades are pure data + an Apply callback against the player,
// so adding a new one is a single entry in UpgradeLibrary.
public sealed class Upgrade
{
    public string Id;
    public string Title;
    public string Description;
    public int MaxStacks = int.MaxValue;
    public Action<PlayerController> Apply;
    // Optional extra gate (e.g. "only offer healing when damaged").
    public Func<PlayerController, bool> IsAvailable;
}

public static class UpgradeLibrary
{
    public static readonly IReadOnlyList<Upgrade> All = new List<Upgrade>
    {
        new Upgrade
        {
            Id = "damage", Title = "Overcharged Rounds", Description = "+30% damage",
            Apply = p => p.damage *= 1.3f,
        },
        new Upgrade
        {
            Id = "firerate", Title = "Rapid Fire", Description = "Fire 15% faster",
            IsAvailable = p => p.FireInterval > PlayerController.MinFireInterval,
            Apply = p => p.FireInterval *= 0.85f,
        },
        new Upgrade
        {
            Id = "multishot", Title = "Split Shot", Description = "+1 projectile per volley",
            MaxStacks = 4,
            Apply = p => p.ProjectileCount++,
        },
        new Upgrade
        {
            Id = "speed", Title = "Thrusters", Description = "+12% movement speed",
            MaxStacks = 5,
            Apply = p => p.speed *= 1.12f,
        },
        new Upgrade
        {
            Id = "hull", Title = "Hull Plating", Description = "+25 max health and repair 25",
            Apply = p => { p.MaxHealth += 25f; p.Heal(25f); },
        },
        new Upgrade
        {
            Id = "regen", Title = "Repair Nanites", Description = "Regenerate +1 health per second",
            MaxStacks = 5,
            Apply = p => p.HealthRegen += 1f,
        },
        new Upgrade
        {
            Id = "magnet", Title = "Tractor Beam", Description = "+40% XP pickup radius",
            MaxStacks = 5,
            Apply = p => p.MagnetRadius *= 1.4f,
        },
        new Upgrade
        {
            Id = "dash", Title = "Afterburner", Description = "Dash recharges 25% faster",
            MaxStacks = 4,
            Apply = p => p.DashCooldown *= 0.75f,
        },
        new Upgrade
        {
            Id = "repair", Title = "Emergency Repair", Description = "Restore 50% of max health",
            IsAvailable = p => p.health < p.MaxHealth * 0.75f,
            Apply = p => p.Heal(p.MaxHealth * 0.5f),
        },
    };

    // Picks up to `count` distinct upgrades the player can still take.
    public static List<Upgrade> Draft(PlayerController player, int count)
    {
        List<Upgrade> candidates = new List<Upgrade>();
        foreach (Upgrade upgrade in All)
        {
            if (player.GetUpgradeStacks(upgrade.Id) >= upgrade.MaxStacks) continue;
            if (upgrade.IsAvailable != null && !upgrade.IsAvailable(player)) continue;
            candidates.Add(upgrade);
        }

        // Partial Fisher-Yates shuffle.
        int picks = Mathf.Min(count, candidates.Count);
        for (int i = 0; i < picks; i++)
        {
            int j = UnityEngine.Random.Range(i, candidates.Count);
            (candidates[i], candidates[j]) = (candidates[j], candidates[i]);
        }
        return candidates.GetRange(0, picks);
    }
}
