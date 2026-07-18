using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public enum BoolSetting
{
    GodMode,
    ScreenShake,
    EarRinging
}

public class OptionsPanelManager : ParentAwareSingleton<OptionsPanelManager>
{
    [SerializeField] private Transform contentTransform;
    [SerializeField] private OptionRowUI toggleRowPrefab;
    [SerializeField] Button closeBtn;
    private Coroutine previewVolume;
    public bool IsGodMode
    {
        get => PlayerData.GetBoolById(PlayerData.isGodMode, false);
        set { PlayerData.SetBoolById(PlayerData.isGodMode, value); }
    }
    public bool IsScreenShake
    {
        get => PlayerData.GetBoolById(PlayerData.IsScreenShake, true);
        set { PlayerData.SetBoolById(PlayerData.IsScreenShake, value); }
    }
    public bool IsEarRinging
    {
        get => PlayerData.GetBoolById(PlayerData.IsEarRinging, true);
        set { PlayerData.SetBoolById(PlayerData.IsEarRinging, value); }
    }
    public bool IsStartingGrace
    {
        get => PlayerData.GetBoolById(PlayerData.IsStartingGrace, true);
        set { PlayerData.SetBoolById(PlayerData.IsStartingGrace, value); }
    }
    public float MasterVolume
    {
        get => PlayerData.GetFloatById(PlayerData.MasterVolume, 0.5f);
        set { PlayerData.SetFloatById(PlayerData.MasterVolume, value); }
    }


    void Start()
    {
        closeBtn.onClick.AddListener(MainMenu.Instance.ToggleOptionsPanel);
        CreateToggle("Screen Shake", IsScreenShake, setIsScreenShake);
        CreateToggle("Ear Ringing", IsEarRinging, setIsEarRinging);
        CreateToggle("3 Seconds Starting Delay", IsStartingGrace, setIsStartingGrace);
        CreateToggle("God Mode", IsGodMode, setIsGodMode);
        CreateSlider("Volume", MasterVolume, SetMasterVolume);
    }

    void SetMasterVolume(float volume)
    {
        MasterVolume = volume;
        AudioManager.Instance.SetMasterVolume(MasterVolume);
        previewVolume ??= StartCoroutine(PlaySoundPreview());
    }
    private IEnumerator PlaySoundPreview()
    {
        AudioManager.Instance.Shoot();
        yield return new WaitWhile(() => AudioManager.Instance.audioSource.isPlaying);
        previewVolume = null;
    }
    public void OnPointerUp(UnityEngine.EventSystems.PointerEventData eventData)
    {
        AudioManager.Instance.Shoot();
    }

    void setIsStartingGrace(bool value)
    {
        IsStartingGrace = value;
    }
    void setIsGodMode(bool value)
    {
        IsGodMode = value;
        var gems = PlayerData.GetIntById(PlayerData.GemsId);
        if (IsGodMode)
        {
            gems += 999;
            PlayerData.SetIntById(PlayerData.GemsId, gems);
            AchievementManager.Instance.UnlockAllAchievements();
        }
        else
        {
            gems = Mathf.Max(gems - 999, 0);
            PlayerData.SetIntById(PlayerData.GemsId, gems);
            AchievementManager.Instance.ResetAllAchievements();
        }
    }
    void setIsScreenShake(bool value)
    {
        IsScreenShake = value;
    }
    void setIsEarRinging(bool value)
    {
        IsEarRinging = value;
    }


    private void CreateToggle(string label, bool defaultValue, UnityEngine.Events.UnityAction<bool> callback)
    {
        OptionRowUI row = Instantiate(toggleRowPrefab, contentTransform);
        row.SetupToggle(label, defaultValue, callback);
    }

    private void CreateSlider(string label, float defaultValue, UnityEngine.Events.UnityAction<float> callback)
    {
        OptionRowUI row = Instantiate(toggleRowPrefab, contentTransform);
        row.SetupSlider(label, defaultValue, callback);
    }


}
