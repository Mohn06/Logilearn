using UnityEngine;
using UnityEngine.SceneManagement;

public class ReturnToMainMenu : MonoBehaviour
{
    public AudioClip buttonSFX;

    void PlaySFX()
    {
        if (AudioManager.Instance != null && buttonSFX != null)
        {
            AudioManager.Instance.PlaySFX(buttonSFX);
        }
    }

    public void GoToMainMenu()
    {
        PlaySFX();

        // Make sure game is not paused
        Time.timeScale = 1f;

        // Load main menu scene
        SceneManager.LoadScene("MAIN MENU");
    }
}