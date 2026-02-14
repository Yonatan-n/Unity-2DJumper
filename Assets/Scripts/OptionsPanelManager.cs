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
    public bool IsGodMode
    {
        get => PlayerData.GetBoolById(PlayerData.isGodMode, false);
        set { PlayerData.SetBoolById(PlayerData.isGodMode, value); }
    }
    public bool IsScreenShake
    {
        get => PlayerData.GetBoolById(PlayerData.IsScreenShake, false);
        set { PlayerData.SetBoolById(PlayerData.IsScreenShake, value); }
    }
    public bool IsEarRinging
    {
        get => PlayerData.GetBoolById(PlayerData.IsEarRinging, false);
        set { PlayerData.SetBoolById(PlayerData.IsEarRinging, value); }
    }

    void Start()
    {
        closeBtn.onClick.AddListener(MainMenu.Instance.ToggleOptionsPanel);
        CreateToggle("Screen Shake", IsScreenShake, setIsScreenShake);
        CreateToggle("Ear Ringing", IsEarRinging, setIsEarRinging);
        CreateToggle("God Mode", IsGodMode, setIsGodMode);
        // CreateSlider("SFX Volume", false, (volume) => AudioManager.Instance.SetMasterVolume(volume));
    }
    void setIsGodMode(bool value)
    {
        IsGodMode = value;
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

    void Update()
    {

    }
}
