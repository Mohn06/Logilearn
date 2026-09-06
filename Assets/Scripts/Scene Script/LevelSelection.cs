using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class LevelSelectUI : MonoBehaviour
{
    public AudioClip buttonSFX;

    [Header("Fade Transition")]
    public Image fadeImage;   // Assign black UI Image in Inspector
    public float fadeDuration = 1f;

    [Header("Level Select Pages")]
    public GameObject[] pages;   // Drag Page1, Page2, Page3 here
    public int currentPage = 0;

    private bool isTransitioning = false;

    void Start()
    {
        ApplySafeArea();

        // Start invisible
        if (fadeImage != null)
        {
            Color color = fadeImage.color;
            fadeImage.color = new Color(color.r, color.g, color.b, 0f);
        }

        ShowPage(currentPage);
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

    void ShowPage(int pageIndex)
    {
        if (pages == null || pages.Length == 0)
            return;

        for (int i = 0; i < pages.Length; i++)
        {
            if (pages[i] != null)
                pages[i].SetActive(i == pageIndex);
        }
    }

    public void NextPage()
    {
        PlaySFX();

        if (pages == null || pages.Length == 0)
            return;

        if (currentPage < pages.Length - 1)
        {
            currentPage++;
            ShowPage(currentPage);
        }
    }

    public void PreviousPage()
    {
        PlaySFX();

        if (pages == null || pages.Length == 0)
            return;

        if (currentPage > 0)
        {
            currentPage--;
            ShowPage(currentPage);
        }
    }

    public void GoToPage(int pageIndex)
    {
        PlaySFX();

        if (pages == null || pages.Length == 0)
            return;

        if (pageIndex >= 0 && pageIndex < pages.Length)
        {
            currentPage = pageIndex;
            ShowPage(currentPage);
        }
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

    public void LoadLevel(string levelName)
    {
        PlaySFX();
        StartCoroutine(FadeAndLoad(levelName));
    }

    public void BackToMenu()
    {
        PlaySFX();
        StartCoroutine(FadeAndLoad("MAIN MENU 2"));
    }

    public void BacktoLevelSelect()
    {
        PlaySFX();
        StartCoroutine(FadeAndLoad("LEVEL SELECTION"));
    }

    public void Leveleasy()
    {
        PlaySFX();
        StartCoroutine(FadeAndLoad("EASY"));
    }

    public void Levelmedium()
    {
        PlaySFX();
        StartCoroutine(FadeAndLoad("MEDIUM"));
    }

    public void Levelhard()
    {
        PlaySFX();
        StartCoroutine(FadeAndLoad("HARD"));
    }

    void PlaySFX()
    {
        if (AudioManager.Instance != null && buttonSFX != null)
            AudioManager.Instance.PlaySFX(buttonSFX);
    }
}