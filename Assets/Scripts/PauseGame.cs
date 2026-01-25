using System;
using UnityEngine;

public class PauseGame : MonoBehaviour
{
    public static GameObject pauseMenuPanel;
    public static bool isGamePaused = false;

    void Start()
    {
        pauseMenuPanel.SetActive(false);
        Time.timeScale = 1f;
    }
    public static void TogglePaused()
    {
        (isGamePaused ? (Action)Resume : Pause)();
    }
    public static void Resume()
    {
        pauseMenuPanel.SetActive(false);
        Time.timeScale = 1f;
        isGamePaused = false;
    }


    public static void Pause()
    {
        pauseMenuPanel.SetActive(true);
        Time.timeScale = 0f;
        isGamePaused = true;
    }

    void LoadMenu()
    {
        Time.timeScale = 1f;
        SceneLoader.LoadSceneByName("MainMenu");
    }

    void QuitGame()
    {
        Debug.Log("quit game");
        Application.Quit();
    }
}
