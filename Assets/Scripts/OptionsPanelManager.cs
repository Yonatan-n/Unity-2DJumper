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
        get => PlayerPrefs.GetInt("isGodMode") == 1;
        set { PlayerPrefs.SetInt("isGodMode", value ? 1 : 0); }
    }
    public bool IsScreenShake
    {
        get => PlayerPrefs.GetInt("IsScreenShake") == 1;
        set { PlayerPrefs.SetInt("IsScreenShake", value ? 1 : 0); }
    }
    public bool IsEarRinging
    {
        get => PlayerPrefs.GetInt("IsEarRinging") == 1;
        set { PlayerPrefs.SetInt("IsEarRinging", value ? 1 : 0); }
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
