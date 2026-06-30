# Space Survivors

A top-down, bullet-hell **survivor-like** built in **Unity (C#)**. You pilot a lone fighter against an endless, ever-escalating swarm of enemies — dodge, shoot, vacuum up XP, level up, and pick a new power every time you do. The longer you last, the harder it gets.

> **TL;DR for reviewers:** This is a from-scratch game whose value isn't the art — it's the **engineering**. The codebase leans on classic, interview-relevant design patterns (Strategy, Object Pool, Observer/State Machine, data-driven config via ScriptableObjects) to keep gameplay systems decoupled, extensible, and performance-conscious. Adding a new weapon, enemy, or power is a matter of dropping in a new asset — not editing existing code.

---

## ▶️ Try It

1. Grab a build from the [Releases page](https://github.com/Emp24/space-survivors/releases).
2. Download and extract the zip.
3. Run `bullet-hell-test.exe`.

**Controls:** `WASD` to move · mouse to aim · left-click to fire · `Esc` to pause.

To open the source: clone the repo and open it in **Unity 2021.3.26f1** (LTS).

---

## 🎮 Gameplay Loop

| System | What it does |
|---|---|
| **Endless waves** | Enemies spawn continuously from off-screen on all sides, capped at a live concurrency limit so the action stays intense without melting the frame rate. |
| **XP & leveling** | Destroyed enemies drop XP blobs that are magnetically vacuumed toward the player within a pickup radius. Enough XP → level up. |
| **Power draft** | Each level-up pauses the game and opens a power-pick menu (more damage / more speed / more health), à la *Vampire Survivors*. |
| **Weapons** | Default rapid-fire gun, a rocket, and a **Gravity Bomb** that detonates into a black-hole field — sucking in nearby enemies and applying damage-over-time. |
| **Enemy variety** | Standard shooters that track and fire at the player, plus fast melee **Seekers** that charge straight in. |

---

## 🏗️ Architecture & Engineering Highlights

The whole project is organized around **programming to interfaces and composing behavior from data** rather than hard-coding it. Here are the parts worth a closer look.

### 1. Strategy Pattern for weapons — as hot-swappable assets

Every weapon is a `FireStrategy`, an **abstract `ScriptableObject`**. Because strategies are ScriptableObjects, they live as `.asset` files in the project and can be assigned to *any* shooter — the player and enemies share the exact same firing pipeline.

```csharp
public abstract class FireStrategy : ScriptableObject
{
    public abstract void FireGun(Transform gunPos, GameObject source);
}
```

```csharp
[CreateAssetMenu]
public class DefaultGunStrategy : FireStrategy
{
    public override void FireGun(Transform gunPos, GameObject source)
    {
        GameObject newBullet = BulletProjectilePool.SharedInstance.GetPooledObject();
        // Stats are *injected from the source* — same gun, different owner:
        projectile.damage = source.GetComponent<IDamageable>().damage;
        projectile.gameObject.layer = LayerMask.NameToLayer(source.GetComponent<IDamageable>().layer);
        ...
    }
}
```

**Why it's nice:** the player simply holds `[SerializeField] FireStrategy defaultGun;` and calls `defaultGun.FireGun(...)`. Swapping a peashooter for a rocket launcher or gravity bomb is a drag-and-drop in the Inspector — **zero code changes**, no `switch` statements, no subclassing the player. Adding a brand-new weapon = create one new file + one asset.

The same idea powers enemy AI: a `RotationStrategy` abstraction (e.g. `FacePlayerRotation`) lets any enemy's turning behavior be defined as data on its `EnemyData` asset.

### 2. Object Pooling for performance

Bullets, enemies, and XP blobs are spawned by the hundreds. Instead of `Instantiate`/`Destroy` thrashing the garbage collector, the game **recycles** GameObjects through pools behind a clean interface:

```csharp
public interface IObjectPool
{
    void InitializePool();
    GameObject GetPooledObject();
    void ReturnToPool(GameObject gameObject);
}
```

Pools (`BulletProjectilePool`, `EnemyProjectilePool`, `ExperienceBlobPool`) pre-instantiate a queue of inactive objects and hand them out on demand, returning them to the queue when they expire. The wave controller pools **enemies** the same way — destroyed enemies are reset (`ResetData()`) and re-queued rather than recreated. This is exactly the kind of allocation-avoidance a bullet-hell game needs to hold its frame rate.

### 3. Event-driven state machine for game flow

A singleton `GameManager` owns a single source of truth (`GameState`) and broadcasts transitions through a C# `event`:

```csharp
public enum GameState { MainMenu, Playing, PowerMenu, GameOver, Pause, StartGame }

public event Action<GameState> OnGameStateChanged;

public void ChangeGameState(GameState state) { ...; OnGameStateChanged?.Invoke(gameState); }
```

UI screens (`PauseMenuView`, `PickPowerMenuView`, `MainMenuView`) **subscribe** to this event and react independently — pausing time, showing/hiding panels, loading scenes. No UI component reaches into another, and nothing polls. This Observer setup keeps the menus, input, and gameplay cleanly decoupled, and views unsubscribe in `OnDestroy()` to avoid dangling handlers across scene loads.

The HUD uses the same pattern at a finer grain: the player exposes `Action onPlayerDataUpdated`, and `HUDView` subscribes to refresh health/XP/level only when something actually changes.

### 4. Interface-driven, data-driven entities

Combat is built on small, focused interfaces so unrelated systems can interact without knowing concrete types:

- **`IDamageable`** — anything with `health`, `damage`, `TakeDamage()`, and `TakeDotDamage()`. The player and every enemy implement it, so a projectile can damage a target without caring what it is.
- **`IEnemy`** — a common contract (`Movement`, `Rotation`, `ResetData`) that lets the wave controller manage `Enemy` and `Seeker` interchangeably.
- **`EnemyData` (ScriptableObject)** — health, damage, fire rate, movement speed, and rotation strategy all live as **editable data assets**. New enemy archetypes can be tuned or created entirely in the editor.

### 5. The Gravity Bomb — composition in action

The `GravityBomb` extends the base `Projectile`, then layers on its own behavior: on impact it switches collision layers, plays an expansion animation, and begins an `OverlapCircle` scan each frame that pulls in nearby enemies and applies interval-based DOT through the shared `IDamageable.TakeDotDamage()` contract. It's a tidy example of reusing the projectile/collision foundation while adding genuinely new mechanics on top.

---

## 📁 Project Structure

```
Assets/Scripts/
├── GameManager.cs              # Singleton state machine + event bus
├── PlayerController.cs         # Movement, aiming, firing, leveling, XP magnet
├── Projectile.cs               # Base projectile + collision handling
├── FireStrategy.cs             # Abstract weapon strategy (ScriptableObject)
├── DefaultGunStrategy.cs       #   ↳ concrete weapons...
├── RocketStrategy.cs
├── GravityBombStrategy.cs
├── IDamageable.cs              # Damage contract shared by player + enemies
├── Enemy/                      # Enemy.cs, Seeker.cs, EnemyData, IEnemy, spawner
├── EnemyWaves/                 # WaveControllerTime — endless pooled spawning
├── ObjectPools/                # IObjectPool + bullet/projectile/XP pools
├── Powers/                     # GravityBomb field mechanic
├── Rotation/                   # RotationStrategy abstraction + FacePlayerRotation
├── Managers/                   # InputManager
└── UserInterface/              # HUD, MainMenu, PauseMenu, PickPowerMenu (event-driven views)
```

**Tech:** Unity 2021.3.26f1 (LTS) · C# · Unity 2D · TextMesh Pro · DOTween · Aseprite import pipeline.

---

## 🧭 Design Principles

- **Open/Closed in practice** — new weapons, enemies, and rotation behaviors are *added*, not patched in. Existing code rarely needs to change.
- **Separation of concerns** — gameplay, UI, input, and state are independent and communicate through events and interfaces.
- **Performance-aware** — pooling everywhere that spawns at scale; no per-frame allocations in the hot path.
- **Data-driven** — ScriptableObjects move tuning out of code and into the editor, so balancing doesn't require a recompile.

---


*Built as a personal project to explore real-time game systems and applied software design patterns in Unity.*
