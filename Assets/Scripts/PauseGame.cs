using System;
using UnityEngine;
using UnityEngine.UI;


public class PauseGame : MonoBehaviour
{
    [SerializeField] GameObject pauseMenuPanel;
    [SerializeField] GameObject shopPanel;
    [SerializeField] GameObject gameOverPanel;
    [SerializeField] GameObject inGamePauseBtn;
    // pause 
    [SerializeField] Button resumeBtn;
    [SerializeField] Button mainMenuBtn;
    [SerializeField] Button jumpBtn;
    [SerializeField] Button shootBtn;
    // game over
    [SerializeField] Button restartGameBtn;
    [SerializeField] Button backToMenuBtn;
    // shop



    public bool isGamePaused = false;
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
        Instance.gameOverPanel.SetActive(false);
        Instance.inGamePauseBtn.SetActive(true);
        Instance.isGamePaused = false; // running 
        // Instance.shopPanel.SetActive(false);

        Time.timeScale = 1f;
        // pause panel
        Instance.resumeBtn.onClick.AddListener(TogglePaused);
        Instance.mainMenuBtn.onClick.AddListener(LoadMenu);
        // game over panel
        Instance.restartGameBtn.onClick.AddListener(RestartRunner);
        Instance.backToMenuBtn.onClick.AddListener(LoadMenu);
        // shop
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
        isGamePaused = false;
        Instance.EnablePlayerButtons(true);
        if (panel == PausePanel.showPausePanel)
        {
            Instance.pauseMenuPanel.SetActive(false);
        }
        else if (panel == PausePanel.showShopPanel)
        {
            Instance.shopPanel.SetActive(false);
            Instance.inGamePauseBtn.SetActive(true); // show in game button
        }
        else if (panel == PausePanel.showGameOverPanel)
        {
            Instance.gameOverPanel.SetActive(false);
        }
    }

    public void Pause(PausePanel panel)
    {
        Time.timeScale = 0f;
        isGamePaused = true;
        Instance.EnablePlayerButtons(false);
        if (panel == PausePanel.showPausePanel)
        {
            Instance.pauseMenuPanel.SetActive(true);
        }
        else if (panel == PausePanel.showShopPanel)
        {
            Instance.shopPanel.SetActive(true);
            Instance.inGamePauseBtn.SetActive(false); // temp hide in game button

        }
        else if (panel == PausePanel.showGameOverPanel)
        {
            Instance.gameOverPanel.SetActive(true);
            Instance.inGamePauseBtn.SetActive(false); // no more in game button
        }
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

public enum PausePanel
{
    showPausePanel,
    showGameOverPanel,
    showShopPanel,
}