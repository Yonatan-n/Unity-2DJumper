using UnityEngine;

public class StatsTracker : MonoBehaviour
{
    public static StatsTracker Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private static void Unlock(AchievementID id) =>
        AchievementManager.Instance.Unlock(id.ToID());

    private static void Progress(AchievementID id, int amount = 1) =>
        AchievementManager.Instance.ReportProgress(id.ToID(), amount);

    // ── Public API ────────────────────────────────────────────────

    public void OnEnemyKilled()
    {
        Unlock(AchievementID.kill_1st);
        Progress(AchievementID.kill_100_total);
        Progress(AchievementID.kill_500_total);
        Progress(AchievementID.kill_1000_total);
    }

    public void OnFlyingEnemyKilled()
    {
        OnEnemyKilled();
        Progress(AchievementID.kill_100_flying);
    }

    public void OnShieldEnemyKilled()
    {
        OnEnemyKilled();
        Progress(AchievementID.kill_100_shield);
    }

    public void OnPlayerDiedToFirst()
    {
        Unlock(AchievementID.die_to_1st);
    }

    public void OnPlayerDied()
    {
        Progress(AchievementID.die_10);
        Progress(AchievementID.die_100);
    }

    public void OnCoinCollected(int amount)
    {
        Progress(AchievementID.coins_10000, amount);
        Progress(AchievementID.coins_100000, amount);
    }

    public void OnCoinSpent(int amount)
    {
        Progress(AchievementID.spend_50000, amount);
    }
    public void OnShoot()
    {
        Progress(AchievementID.shoot_10000);
    }
    public void OnJump()
    {
        Progress(AchievementID.jump_1000);
    }

    public void OnQuadJump()
    {
        Unlock(AchievementID.jump_4);
    }

    public void OnLevelCleared(int level)
    {
        Unlock((AchievementID)System.Enum.Parse(typeof(AchievementID), $"level_{level}"));
    }

    public void OnSessionStarted()
    {
        Progress(AchievementID.play_42_sessions);
    }

    public void OnGunUnlocked()
    {
        Progress(AchievementID.unlock_3_guns);
    }

    public void OnLevelClearedNoShoot() => Unlock(AchievementID.level_4_no_shoot);
    public void OnLevelClearedNoLives() => Unlock(AchievementID.level_4_no_extra_lives);
    public void OnLevelClearedNoAmmo() => Unlock(AchievementID.level_4_no_extra_ammo);
    public void OnLevelClearedNoJumps() => Unlock(AchievementID.level_4_no_extra_jumps);
    public void OnLevelClearedNoUpgrades() => Unlock(AchievementID.level_4_no_upgrades);
    public void OnKillstreak() => Unlock(AchievementID.killstreak_5);
    public void OnLifeGained(int current) { if (current >= 5) Unlock(AchievementID.life_5); }
}