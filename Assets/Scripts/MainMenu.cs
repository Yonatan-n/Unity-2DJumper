using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] Button play;
    [SerializeField] Button options;
    [SerializeField] Button gear;
    [SerializeField] Button credits;
    [SerializeField] Button quit;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        play.onClick.AddListener(GoToPlay);
        options.onClick.AddListener(GoToOptions);
        gear.onClick.AddListener(GoToGear);
        credits.onClick.AddListener(GoToCredits); // maybe not needed
        quit.onClick.AddListener(Quit);//
    }

    void GoToCredits()
    {
        SceneLoader.Instance.LoadSceneByName("CreditsPage");
    }

    void Quit()
    {
        Application.Quit();
    }

    void GoToOptions()
    {
        SceneLoader.Instance.LoadSceneByName("OptionsPage");
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
