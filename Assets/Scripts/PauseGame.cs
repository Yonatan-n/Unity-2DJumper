using System;
using UnityEngine;
using UnityEngine.UI;


public class PauseGame : ParentAwareSingleton<PauseGame>
{
    [SerializeField] GameObject pauseMenuPanel;
    [SerializeField] GameObject shopPanel;
    [SerializeField] GameObject gameOverPanel;
    [SerializeField] Button inGamePauseBtn;
    // pause 
    [SerializeField] Button resumeBtn;
    [SerializeField] Button mainMenuBtn;
    [SerializeField] Button jumpBtn;
    [SerializeField] Button shootBtn;
    // game over
    [SerializeField] Button restartGameBtn;
    [SerializeField] Button backToMenuBtn;
    // shop
    [SerializeField] Button NextLevelBtn;




    public bool isGamePaused = false;

    void Start()
    {
        Instance.pauseMenuPanel.SetActive(false);
        Instance.gameOverPanel.SetActive(false);
        Instance.shopPanel.SetActive(false);
        Instance.inGamePauseBtn.interactable = true;
        Instance.isGamePaused = false; // running 

        Time.timeScale = 1f;
        // pause panel
        Instance.resumeBtn.onClick.AddListener(TogglePaused);
        Instance.mainMenuBtn.onClick.AddListener(LoadMenu);
        // game over panel
        Instance.restartGameBtn.onClick.AddListener(RestartRunner);
        Instance.backToMenuBtn.onClick.AddListener(LoadMenu);
        // shop
        Instance.NextLevelBtn.onClick.AddListener(StartNextLevel);
        EnablePlayerButtons(true);
    }
    void RestartRunner()
    {
        SceneLoader.ReloadCurrentScene();
    }
    private void EnablePlayerButtons(bool isEnabled)
    {
        Instance.shootBtn.interactable = isEnabled;
        Instance.jumpBtn.interactable = isEnabled;
    }
    public void TogglePaused()
    {
        if (Instance.isGamePaused)
            Instance.Resume(PausePanel.showPausePanel);
        else
            Instance.Pause(PausePanel.showPausePanel);
    }
    public void Resume(PausePanel panel)
    {
        Time.timeScale = 1f;
        Instance.isGamePaused = false;
        Instance.EnablePlayerButtons(true);
        if (panel == PausePanel.showPausePanel)
        {
            Instance.pauseMenuPanel.SetActive(false);
        }
        else if (panel == PausePanel.showShopPanel)
        {
            Instance.shopPanel.SetActive(false);
            Instance.inGamePauseBtn.interactable = true;
        }
        else if (panel == PausePanel.showGameOverPanel)
        {
            Instance.gameOverPanel.SetActive(false);
        }
    }

    public void Pause(PausePanel panel)
    {
        Time.timeScale = 0f;
        Instance.isGamePaused = true;
        Instance.EnablePlayerButtons(false);
        if (panel == PausePanel.showPausePanel)
        {
            Instance.pauseMenuPanel.SetActive(true);
        }
        else if (panel == PausePanel.showShopPanel)
        {
            Instance.shopPanel.SetActive(true);
            Instance.inGamePauseBtn.interactable = false;
        }
        else if (panel == PausePanel.showGameOverPanel)
        {
            Instance.gameOverPanel.SetActive(true);
            Instance.inGamePauseBtn.interactable = false;
        }
    }
    public void StartNextLevel()
    {
        Resume(PausePanel.showShopPanel);
        Debug.Log("start level 2+");
        GameManager.Instance.LoadNextLevel();
    }
    public void LoadMenu()
    {
        SceneLoader.Instance.LoadSceneByName("MainMenu");
        Time.timeScale = 1f;
    }

    public void QuitGame()
    {
        Debug.Log("quit game");
        Application.Quit();
    }
}

public enum PausePanel
{
    showPausePanel,
    showGameOverPanel,
    showShopPanel,
}