using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] Button play;
    [SerializeField] Button options;
    [SerializeField] Button gear;
    [SerializeField] Button quit;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        play.onClick.AddListener(GoToPlay);
        options.onClick.AddListener(GoToOptions);
        gear.onClick.AddListener(GoToGear);
        quit.onClick.AddListener(Quit);

    }

    void Quit()
    {
        Application.Quit();
    }

    void GoToOptions()
    {
        SceneLoader.LoadSceneByName("OptionsPage");
    }

    void GoToPlay()
    {
        SceneLoader.LoadSceneByName("Runner");
    }

    void GoToGear()
    {
        SceneLoader.LoadSceneByName("GearPage");
    }

    // Update is called once per frame
    void Update()
    {

    }
}
