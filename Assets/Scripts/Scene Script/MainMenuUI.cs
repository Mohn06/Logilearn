using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class MainMenuUI : MonoBehaviour
{
    public AudioClip buttonSFX;

    [Header("Fade Transition")]
    public Image fadeImage;       
    public float fadeDuration = 1f;
    private bool isTransitioning = false;

    void Start()
    {
        ApplySafeArea();

        
        if (fadeImage != null)
        {
            Color color = fadeImage.color;
            fadeImage.color = new Color(color.r, color.g, color.b, 0f);
        }
    }

    void ApplySafeArea()
    {
        Rect safeArea = Screen.safeArea;

        Vector2 min = safeArea.position;
        Vector2 max = safeArea.position + safeArea.size;

        min.x /= Screen.width;
        min.y /= Screen.height;
        max.x /= Screen.width;
        max.y /= Screen.height;

        RectTransform rect = GetComponent<RectTransform>();
        rect.anchorMin = min;
        rect.anchorMax = max;
    }

    void PlaySFX()
    {
        if (AudioManager.Instance != null && buttonSFX != null)
            AudioManager.Instance.PlaySFX(buttonSFX);
    }

    IEnumerator FadeAndLoad(string sceneName)
    {
        if (isTransitioning)
            yield break;

        isTransitioning = true;

        float timer = 0f;
        Color color = fadeImage.color;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Clamp01(timer / fadeDuration);
            fadeImage.color = new Color(color.r, color.g, color.b, alpha);
            yield return null;
        }

        SceneManager.LoadScene(sceneName);
    }

    public void PlayGame()
    {
        PlaySFX();
        SceneManager.LoadScene("MAIN MENU 2");
    }

    public void LoadLevelSelect()
    {
        PlaySFX();
        StartCoroutine(FadeAndLoad("LEVEL SELECTION"));
    }

    public void LoadQuiz()
    {
        PlaySFX();
        StartCoroutine(FadeAndLoad("QUIZ LEVELS"));
    }

    public void Sandbox()
    {
        PlaySFX();
        StartCoroutine(FadeAndLoad("SANDBOX"));
    }

    public void BackToMenu()
    {
        PlaySFX();
        SceneManager.LoadScene("MAIN MENU");
    }

    public void Option()
    {
        PlaySFX();
        SceneManager.LoadScene("OPTION");
    }

    public void QuitGame()
    {
        PlaySFX();
        Debug.Log("Quit Game");
        Application.Quit();
    }
}