using System;
using UnityEngine;

public class PauseGame : MonoBehaviour
{
    [SerializeField] GameObject pauseMenuPanel;
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
    }


    public static void Pause()
    {
        Instance.pauseMenuPanel.SetActive(true);
        Time.timeScale = 0f;
        isGamePaused = true;
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
