using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class PauseMenu : MonoBehaviour
{
    public static bool gameIsPaused = false;

    public GameObject pauseMenuUI;

    void Start()
    {
        GameEvents.onGameOver += OnGameOver;
        InputManager.instance.onEscapeButtonPressStarted += EscapePressed;
    }

    void EscapePressed()
    {
            if (gameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
    }

    void OnGameOver()
    {
        InputManager.instance.onEscapeButtonPressStarted -= EscapePressed;
        GameEvents.onGameOver -= OnGameOver;
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        gameIsPaused = false;
    }
    
    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        gameIsPaused = true;
    }

    public void LoadMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("tittle");
    }

    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
    }
}
