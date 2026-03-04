using UnityEngine;
using UnityEngine.UI;

public class MainMenu : ParentAwareSingleton<MainMenu>
{
    [SerializeField] Button play;
    [SerializeField] Button options;
    [SerializeField] GameObject optionsPanel;
    bool _showOptions;
    [SerializeField] Button gear;
    [SerializeField] Button achievements;
    [SerializeField] Button quit;
    [SerializeField] PlayerEquipment previewEquipment;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        play.onClick.AddListener(GoToPlay);
        options.onClick.AddListener(ToggleOptionsPanel);
        // gear.interactable = false;// hardcoded disable for now
        gear.onClick.AddListener(GoToGear);
        achievements.onClick.AddListener(GoToAchievements);
        quit.onClick.AddListener(Quit);//
        _showOptions = false;
        optionsPanel.SetActive(_showOptions);
        // PlayerData.ResetAll();
        if (PlayerData.GetBoolById(PlayerData.isFirstStart, true))
        {
            var defaultGunId = "9";   // 1911
            var gear = PlayerData.GetGearById(defaultGunId);
            PlayerData.SetOwned(gear);
            previewEquipment.Equip(gear);
            PlayerData.SetBoolById(PlayerData.isFirstStart, false);
        }
        previewEquipment.LoadFromPlayerData();
    }


    void Quit()
    {
        Application.Quit();
    }

    public void ToggleOptionsPanel()
    {
        _showOptions = !_showOptions;
        optionsPanel.SetActive(_showOptions);
        // SceneLoader.Instance.LoadSceneByName("OptionsPage");
    }

    void GoToPlay()
    {
        SceneLoader.Instance.LoadSceneByName("Runner");
    }

    void GoToGear()
    {
        SceneLoader.Instance.LoadSceneByName("GearPage");
    }

    void GoToAchievements()
    {
        SceneLoader.Instance.LoadSceneByName("AchievementsPage");
    }

    // Update is called once per frame
    void Update()
    {

    }
}
