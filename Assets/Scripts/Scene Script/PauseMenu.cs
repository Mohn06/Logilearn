using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pausePanel;
    public GameObject settingsPanel;
    public GameObject controlUI;
    public GameObject Journal;

    public AudioClip buttonSFX;
    
    void PlaySFX()
    {
        AudioManager.Instance.PlaySFX(buttonSFX);
    }

    private bool isPaused = false;

    void Start()
    {
        pausePanel.SetActive(false);
        settingsPanel.SetActive(false);
    }

    public void TogglePause()
    {
        PlaySFX();
        if (isPaused)
            Resume();
        else
            Pause();
    }

    public void Pause()
    {
        PlaySFX();
        pausePanel.SetActive(true);
        controlUI.SetActive(false);
        Journal.SetActive(false);

        Time.timeScale = 0f;
        isPaused = true;
    }

    public void Resume()
    {
        PlaySFX();
        pausePanel.SetActive(false);
        settingsPanel.SetActive(false);
        Journal.SetActive(true);
        controlUI.SetActive(true);

        Time.timeScale = 1f;
        isPaused = false;
    }

    // OPEN SETTINGS PANEL
    public void OpenSettings()
    {
        PlaySFX();
        pausePanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    // BACK TO PAUSE MENU
    public void BackToPause()
    {
        PlaySFX();
        settingsPanel.SetActive(false);
        pausePanel.SetActive(true);
    }

    // RESET LEVEL
    public void ResetLevel()
    {
        PlaySFX();
        Time.timeScale = 1f; // unpause game

        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.buildIndex);
    }

    public void ExitToMenu()
    {
        PlaySFX();
        Time.timeScale = 1f;
        SceneManager.LoadScene("MAIN MENU");
    }
}