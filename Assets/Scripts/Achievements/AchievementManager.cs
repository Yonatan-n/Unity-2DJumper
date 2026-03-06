using System;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Achievements/Achievement")]
public class AchievementDefinition : ScriptableObject
{
    public string id;
    public string displayName;
    public string description;
    public Sprite icon;
    public int progressTarget = 1;

    public bool IsProgressBased => progressTarget > 1;
}

[Serializable]
public class AchievementData
{
    public bool isUnlocked;
    public string unlockDate;
    public int progress;
}


public class AchievementManager : MonoBehaviour
{
    public static AchievementManager Instance { get; private set; }

    [Header("Definitions (ScriptableObjects)")]
    [SerializeField] private List<AchievementDefinition> soDefinitions = new();

    [Header("References")]
    private AchievementPopup popup;

    private readonly List<AchievementDefinition> _allDefinitions = new();
    private readonly Dictionary<string, AchievementData> _runtimeData = new();
    private readonly Dictionary<string, List<Action>> _unlockCallbacks = new();

    public event Action<AchievementDefinition, AchievementData> OnAchievementUnlocked;

    // ── Registry (static, call before Awake) ─────────────────────

    internal static readonly List<AchievementDefinition> _preRegistered = new();

    public static AchievementDefinition Register(
        string id, string displayName, string description,
        Sprite icon = null, int progressTarget = 1)
    {
        if (_preRegistered.Exists(d => d.id == id))
        {
            Debug.LogWarning($"Achievement '{id}' already registered.");
            return _preRegistered.Find(d => d.id == id);
        }

        var def = ScriptableObject.CreateInstance<AchievementDefinition>();
        def.id = id;
        def.displayName = displayName;
        def.description = description;
        def.icon = icon;
        def.progressTarget = Mathf.Max(1, progressTarget);

        _preRegistered.Add(def);
        return def;
    }

    // ── Lifecycle ─────────────────────────────────────────────────

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        _allDefinitions.AddRange(soDefinitions);
        foreach (var def in _preRegistered)
            if (!_allDefinitions.Exists(d => d.id == def.id))
                _allDefinitions.Add(def);

        Load();
    }

    // ── Public API ────────────────────────────────────────────────

    public void RegisterUnlockCallback(string id, Action callback)
    {
        if (!_unlockCallbacks.ContainsKey(id))
            _unlockCallbacks[id] = new();
        _unlockCallbacks[id].Add(callback);
    }

    public void Unlock(string id) =>
        ReportProgress(id, GetDefinition(id)?.progressTarget ?? 1);

    public void ReportProgress(string id, int amount = 1)
    {
        if (!_runtimeData.TryGetValue(id, out var data))
        {
            Debug.LogWarning($"Achievement '{id}' not found.");
            return;
        }

        if (data.isUnlocked) return;

        var def = GetDefinition(id);
        data.progress = Mathf.Min(data.progress + amount, def.progressTarget);
        Save();

        if (data.progress >= def.progressTarget)
            CompleteAchievement(id, def, data);
    }

    public AchievementData GetData(string id)
    {
        if (_runtimeData.TryGetValue(id, out var d)) return d;
        return new AchievementData();
    }


    public IReadOnlyList<AchievementDefinition> GetAllDefinitions() => _allDefinitions;

    // ── Internal ──────────────────────────────────────────────────

    private void CompleteAchievement(string id, AchievementDefinition def, AchievementData data)
    {
        data.isUnlocked = true;
        data.unlockDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
        Save();

        OnAchievementUnlocked?.Invoke(def, data);

        if (_unlockCallbacks.TryGetValue(id, out var callbacks))
            foreach (var cb in callbacks) cb?.Invoke();

        popup = FindFirstObjectByType<AchievementPopup>();
        popup.Show(def);
    }

    private AchievementDefinition GetDefinition(string id) =>
        _allDefinitions.Find(d => d.id == id);


    private void Load()
    {
        foreach (var def in _allDefinitions)
        {
            var json = PlayerPrefs.GetString($"ach_{def.id}", null);
            var data = json != null ? JsonUtility.FromJson<AchievementData>(json) : null;
            _runtimeData[def.id] = data ?? new AchievementData();
        }
    }

    private void Save()
    {
        foreach (var (id, data) in _runtimeData)
            PlayerPrefs.SetString($"ach_{id}", JsonUtility.ToJson(data));
        PlayerPrefs.Save();
    }

    [ContextMenu("Reset All Achievements")]
    public void ResetAllAchievements()
    {
        Debug.Log($"Resetting {_allDefinitions.Count} achievements");
        foreach (var def in _allDefinitions)
        {
            _runtimeData[def.id] = new AchievementData();
            PlayerPrefs.DeleteKey($"ach_{def.id}");
        }
        PlayerPrefs.Save();
        Debug.Log("ResetAllAchievements ran");
    }

    [ContextMenu("Unlock All Achievements")]
    public void UnlockAllAchievements()
    {
        foreach (var def in _allDefinitions)
            Unlock(def.id);
    }


}

public static class AchievementBootstrapper
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Register()
    {
        AchievementManager._preRegistered.Clear();

        // ── Kills ─────────────────────────────────────────────────────────
        AchievementManager.Register(AchievementID.kill_1st.ToID(), "First Blood", "Kill your first enemy");
        AchievementManager.Register(AchievementID.kill_100_total.ToID(), "Body Count", "Kill 100 enemies", progressTarget: 100);
        AchievementManager.Register(AchievementID.kill_500_total.ToID(), "Walking Disaster", "Kill 500 enemies", progressTarget: 500);
        AchievementManager.Register(AchievementID.kill_1000_total.ToID(), "Genocide", "Kill 1000 enemies", progressTarget: 1000);
        AchievementManager.Register(AchievementID.kill_100_flying.ToID(), "MiG-15 Down", "Shoot down 100 flying enemies", progressTarget: 100);
        AchievementManager.Register(AchievementID.kill_100_shield.ToID(), "Can Opener", "Destroy 100 shields", progressTarget: 100);

        // ── Deaths ────────────────────────────────────────────────────────
        AchievementManager.Register(AchievementID.die_to_1st.ToID(), "Didn't See That", "Die to the first obstacle");
        AchievementManager.Register(AchievementID.die_10.ToID(), "Trial & Error", "Die 10 times", progressTarget: 10);
        AchievementManager.Register(AchievementID.die_100.ToID(), "Frequent Flyer", "Die 100 times", progressTarget: 100);

        // ── Coins ─────────────────────────────────────────────────────────
        AchievementManager.Register(AchievementID.coins_10000.ToID(), "Loaded", "Collect 10,000 coins", progressTarget: 10000);
        AchievementManager.Register(AchievementID.coins_100000.ToID(), "Money Printer", "Collect 100,000 coins", progressTarget: 100000);
        AchievementManager.Register(AchievementID.spend_50000.ToID(), "Retail Therapy", "Spend 50,000 coins", progressTarget: 50000);

        // ── Movement ──────────────────────────────────────────────────────
        AchievementManager.Register(AchievementID.jump_4.ToID(), "To the Moon", "Perform a quadruple jump", progressTarget: 4);
        AchievementManager.Register(AchievementID.jump_1000.ToID(), "Jumping to Conclusions", "Jump 1,000 times", progressTarget: 1000);

        // ── Weapons ───────────────────────────────────────────────────────
        AchievementManager.Register(AchievementID.shoot_10000.ToID(), "Break-in period", "Shoot 10,000 bullets", progressTarget: 10000);
        AchievementManager.Register(AchievementID.unlock_3_guns.ToID(), "Collector", "Unlock 3 new guns", progressTarget: 3);
        AchievementManager.Register(AchievementID.killstreak_5.ToID(), "Killstreak", "Shoot 5 magazines without missing");

        // ── Levels ────────────────────────────────────────────────────────
        AchievementManager.Register(AchievementID.level_1.ToID(), "V", "Clear level 1");
        AchievementManager.Register(AchievementID.level_2.ToID(), "I", "Clear level 2");
        AchievementManager.Register(AchievementID.level_3.ToID(), "C", "Clear level 3");
        AchievementManager.Register(AchievementID.level_4.ToID(), "T", "Clear level 4");
        AchievementManager.Register(AchievementID.level_5.ToID(), "O", "Clear level 5");
        AchievementManager.Register(AchievementID.level_6.ToID(), "R", "Clear level 6");
        AchievementManager.Register(AchievementID.level_7.ToID(), "Y", "Clear level 7");

        // ── Challenge runs ────────────────────────────────────────────────
        AchievementManager.Register(AchievementID.level_4_no_shoot.ToID(), "Pacifist", "Clear level 4 without shooting");
        AchievementManager.Register(AchievementID.level_4_no_extra_lives.ToID(), "Flawless", "Clear level 4 without buying extra lives");
        AchievementManager.Register(AchievementID.level_4_no_extra_ammo.ToID(), "Small Mags, Big D-", "Clear level 4 without buying extra ammo");
        AchievementManager.Register(AchievementID.level_4_no_extra_jumps.ToID(), "Jumper? I Hardly Know Her", "Clear level 4 without buying extra jumps");
        AchievementManager.Register(AchievementID.level_4_no_upgrades.ToID(), "Factory Settings", "Clear level 4 buying nothing at all");

        // ── Survival / misc ───────────────────────────────────────────────
        AchievementManager.Register(AchievementID.life_5.ToID(), "Cat-ish", "Have 5 lives", progressTarget: 5);
        AchievementManager.Register(AchievementID.play_42_sessions.ToID(), "Marathon", "Play 42 runs", progressTarget: 30);
    }
}

public static class AchievementIDExtensions
{
    public static string ToID(this AchievementID id) => id.ToString();
}

public enum AchievementID
{
    // Kills
    kill_1st,
    kill_100_total,
    kill_500_total,
    kill_1000_total,
    kill_100_flying,
    kill_100_shield,

    // Deaths
    die_to_1st,
    die_10,
    die_100,

    // Coins
    coins_10000,
    coins_100000,
    spend_50000,

    // Movement
    jump_4,
    jump_1000,

    // Weapons
    unlock_3_guns,
    killstreak_5,
    shoot_10000,

    // Levels
    level_1,
    level_2,
    level_3,
    level_4,
    level_5,
    level_6,
    level_7,

    // Challenge runs
    level_4_no_shoot,
    level_4_no_extra_lives,
    level_4_no_extra_ammo,
    level_4_no_extra_jumps,
    level_4_no_upgrades,

    // Survival / misc
    life_5,
    play_42_sessions,
}