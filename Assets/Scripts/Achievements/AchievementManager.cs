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
    [SerializeField] private AchievementPopup popup;

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


}

public static class AchievementBootstrapper
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Register()
    {
        AchievementManager._preRegistered.Clear(); // ← add this line
        // ── Kills ─────────────────────────────────────────────────────────
        AchievementManager.Register("kill_1st", "First Blood", "Kill your first enemy");
        AchievementManager.Register("kill_100_total", "Body Count", "Kill 100 enemies", progressTarget: 100);
        AchievementManager.Register("kill_500_total", "Walking Disaster", "Kill 500 enemies", progressTarget: 500);
        AchievementManager.Register("kill_1000_total", "Genocide", "Kill 500 enemies", progressTarget: 1000);
        AchievementManager.Register("kill_100_flying", "MiG-15 down", "Shoot down 100 flying enemies", progressTarget: 100);
        AchievementManager.Register("kill_100_shield", "Can Opener", "Destroy 100 shielded enemies", progressTarget: 100);

        // ── Deaths ────────────────────────────────────────────────────────
        AchievementManager.Register("die_to_1st", "Didn't See That", "Die to the first obstacle");
        AchievementManager.Register("die_10", "Trial & Error", "Die 10 times", progressTarget: 10);
        AchievementManager.Register("die_100", "Frequent Flyer", "Die 100 times", progressTarget: 100);

        // ── Coins ─────────────────────────────────────────────────────────
        AchievementManager.Register("coins_10000", "Loaded", "Collect 10,000 coins", progressTarget: 10000);
        AchievementManager.Register("coins_100000", "Money Printer", "Collect 100,000 coins", progressTarget: 100000);
        AchievementManager.Register("spend_50000", "Retail Therapy", "Spend 50,000 coins", progressTarget: 50000);

        // ── Movement ──────────────────────────────────────────────────────
        AchievementManager.Register("jump_4", "To the Moon", "Perform a quadruple jump", progressTarget: 4);
        AchievementManager.Register("jump_1000", "Jumping to Conclusions", "Jump 1,000 times", progressTarget: 1000);

        // ── Weapons ───────────────────────────────────────────────────────
        AchievementManager.Register("unlock_3_guns", "Collector", "Unlock 3 different guns", progressTarget: 3);
        AchievementManager.Register("killstreak_5", "Killstreak", "Shoot 5 magazines without missing");

        // ── Levels ────────────────────────────────────────────────────────
        AchievementManager.Register("level_1", "Warm Up", "Clear level 1");
        AchievementManager.Register("level_2", "Oops I did it again", "Clear level 2");
        AchievementManager.Register("level_3", "Halfway There", "Clear level 3");
        AchievementManager.Register("level_4", "No Turning Back", "Clear level 4");
        AchievementManager.Register("level_5", "Veteran", "Clear level 5");
        AchievementManager.Register("level_6", "Elite", "Clear level 6");
        AchievementManager.Register("level_7", "The Bitter End", "Clear level 7");

        // ── Challenge runs ────────────────────────────────────────────────
        AchievementManager.Register("level_4_no_shoot", "Pacifist", "Clear level 4 without shooting");
        AchievementManager.Register("level_4_no_extra_lives", "Flawless", "Clear level 4 without buying extra lives");
        AchievementManager.Register("level_4_no_extra_ammo", "Small mags, big d-", "Clear level 4 without buying extra ammo");
        AchievementManager.Register("level_4_no_extra_jumps", "Jumper? I hardly know her", "Clear level 4 without buying extra jumps");
        AchievementManager.Register("level_4_no_upgrades", "Factory Settings", "Clear level 4 buying nothing at all");

        // ── Survival / misc ───────────────────────────────────────────────
        AchievementManager.Register("life_5", "Cat-ish", "Have 5 lives", progressTarget: 5);
        AchievementManager.Register("play_30_sessions", "Marathon", "Play 30 runs", progressTarget: 30);

    }
}
