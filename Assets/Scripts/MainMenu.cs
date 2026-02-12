using UnityEngine;
using UnityEngine.UI;

public class MainMenu : ParentAwareSingleton<MainMenu>
{
    [SerializeField] Button play;
    [SerializeField] Button options;
    [SerializeField] GameObject optionsPanel;
    bool _showOptions;
    [SerializeField] Button gear;
    [SerializeField] Button quit;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        play.onClick.AddListener(GoToPlay);
        options.onClick.AddListener(ToggleOptionsPanel);
        gear.interactable = false;// hardcoded disable for now
        gear.onClick.AddListener(GoToGear);
        quit.onClick.AddListener(Quit);//
        _showOptions = false;
        optionsPanel.SetActive(_showOptions);
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

    // Update is called once per frame
    void Update()
    {

    }
}
