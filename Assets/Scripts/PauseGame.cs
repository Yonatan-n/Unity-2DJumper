using System;
using UnityEngine;
using UnityEngine.UI;


public class PauseGame : MonoBehaviour
{
    [SerializeField] GameObject pauseMenuPanel;
    [SerializeField] Button resumeBtn;
    [SerializeField] Button mainMenuBtn;
    [SerializeField] Button jumpBtn;
    [SerializeField] Button shootBtn;

    public static bool isGamePaused = false;
    public static PauseGame Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this.gameObject);
        else
            Instance = this;
    }

    void Start()
    {
        Instance.pauseMenuPanel.SetActive(false);
        Time.timeScale = 1f;
        Instance.resumeBtn.onClick.AddListener(TogglePaused);
        Instance.mainMenuBtn.onClick.AddListener(LoadMenu);
        EnablePlayerButtons(true);
    }

    private void EnablePlayerButtons(bool isEnabled)
    {
        shootBtn.interactable = isEnabled;
        jumpBtn.interactable = isEnabled;
    }
    public static void TogglePaused()
    {
        (isGamePaused ? (Action)Resume : Pause)();
    }
    public static void Resume()
    {
        Instance.pauseMenuPanel.SetActive(false);
        Time.timeScale = 1f;
        isGamePaused = false;
        Instance.EnablePlayerButtons(true);
    }


    public static void Pause()
    {
        Instance.pauseMenuPanel.SetActive(true);
        Time.timeScale = 0f;
        isGamePaused = true;
        Instance.EnablePlayerButtons(false);
    }

    public void LoadMenu()
    {
        Time.timeScale = 1f;
        SceneLoader.LoadSceneByName("MainMenu");
    }

    public void QuitGame()
    {
        Debug.Log("quit game");
        Application.Quit();
    }
}
