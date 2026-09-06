using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TutorialPopupTextOnly : MonoBehaviour
{
    [System.Serializable]
    public class TutorialPage
    {
        public string title;

        [TextArea(3, 8)]
        public string description;
    }

    [Header("Pages")]
    public List<TutorialPage> pages = new List<TutorialPage>();

    [Header("Popup UI")]
    public GameObject popupPanel;
    public GameObject backgroundOverlay;
    public TMP_Text titleText;
    public TMP_Text descriptionText;

    [Header("Sliding Content Root")]
    public RectTransform contentRoot;

    [Header("Buttons")]
    public Button nextButton;
    public Button prevButton;
    public Button closeButton;

    [Header("Dots")]
    public Transform dotsParent;
    public GameObject dotPrefab;
    public Color activeDotColor = Color.white;
    public Color inactiveDotColor = Color.gray;

    [Header("Fade Settings")]
    public float fadeDuration = 0.25f;

    [Header("Slide Settings")]
    public float slideDuration = 0.25f;
    public float slideDistance = 700f;

    private int currentPage = 0;
    private List<Image> dots = new List<Image>();
    private float previousTimeScale = 1f;
    private bool isShowing = false;
    private bool isAnimating = false;

    private CanvasGroup popupCanvasGroup;
    private CanvasGroup backgroundCanvasGroup;
    private CanvasGroup contentCanvasGroup;

    private Coroutine fadeCoroutine;
    private Coroutine slideCoroutine;

    private void Awake()
    {
        if (nextButton != null)
            nextButton.onClick.AddListener(NextPage);

        if (prevButton != null)
            prevButton.onClick.AddListener(PrevPage);

        if (closeButton != null)
            closeButton.onClick.AddListener(CloseTutorial);

        SetupCanvasGroups();
        CreateDots();

        if (popupPanel != null)
            popupPanel.SetActive(false);

        if (backgroundOverlay != null)
            backgroundOverlay.SetActive(false);
    }

    private void SetupCanvasGroups()
    {
        if (popupPanel != null)
        {
            popupCanvasGroup = popupPanel.GetComponent<CanvasGroup>();
            if (popupCanvasGroup == null)
                popupCanvasGroup = popupPanel.AddComponent<CanvasGroup>();

            popupCanvasGroup.alpha = 0f;
            popupCanvasGroup.interactable = false;
            popupCanvasGroup.blocksRaycasts = false;
        }

        if (backgroundOverlay != null)
        {
            backgroundCanvasGroup = backgroundOverlay.GetComponent<CanvasGroup>();
            if (backgroundCanvasGroup == null)
                backgroundCanvasGroup = backgroundOverlay.AddComponent<CanvasGroup>();

            backgroundCanvasGroup.alpha = 0f;
            backgroundCanvasGroup.interactable = false;
            backgroundCanvasGroup.blocksRaycasts = false;
        }

        if (contentRoot != null)
        {
            contentCanvasGroup = contentRoot.GetComponent<CanvasGroup>();
            if (contentCanvasGroup == null)
                contentCanvasGroup = contentRoot.gameObject.AddComponent<CanvasGroup>();

            contentCanvasGroup.alpha = 1f;
        }
    }

    private void CreateDots()
    {
        if (dotsParent == null || dotPrefab == null)
            return;

        for (int i = dotsParent.childCount - 1; i >= 0; i--)
            Destroy(dotsParent.GetChild(i).gameObject);

        dots.Clear();

        for (int i = 0; i < pages.Count; i++)
        {
            GameObject dot = Instantiate(dotPrefab, dotsParent);
            Image img = dot.GetComponent<Image>();

            if (img != null)
                dots.Add(img);
        }
    }

    public void ShowTutorial()
    {
        if (pages == null || pages.Count == 0)
        {
            Debug.LogWarning("No tutorial pages assigned.");
            return;
        }

        if (isAnimating) return;

        currentPage = 0;
        RefreshUI();

        if (contentRoot != null)
            contentRoot.anchoredPosition = Vector2.zero;

        if (backgroundOverlay != null)
            backgroundOverlay.SetActive(true);

        if (popupPanel != null)
            popupPanel.SetActive(true);

        PauseGame();

        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(FadeInRoutine());
    }

    public void CloseTutorial()
    {
        if (isAnimating) return;

        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(FadeOutRoutine());
    }

    public void NextPage()
    {
        if (isAnimating) return;
        if (pages == null || pages.Count == 0) return;
        if (currentPage >= pages.Count - 1) return;

        if (slideCoroutine != null)
            StopCoroutine(slideCoroutine);

        slideCoroutine = StartCoroutine(SlideToPage(currentPage + 1, true));
    }

    public void PrevPage()
    {
        if (isAnimating) return;
        if (pages == null || pages.Count == 0) return;
        if (currentPage <= 0) return;

        if (slideCoroutine != null)
            StopCoroutine(slideCoroutine);

        slideCoroutine = StartCoroutine(SlideToPage(currentPage - 1, false));
    }

    private IEnumerator SlideToPage(int targetPage, bool movingNext)
    {
        isAnimating = true;
        SetButtonsInteractable(false);

        if (contentRoot == null)
        {
            currentPage = targetPage;
            RefreshUI();
            SetButtonsInteractable(true);
            isAnimating = false;
            yield break;
        }

        Vector2 startPos = Vector2.zero;
        Vector2 exitPos = movingNext ? new Vector2(-slideDistance, 0f) : new Vector2(slideDistance, 0f);
        Vector2 enterPos = -exitPos;

        float time = 0f;

        while (time < slideDuration)
        {
            time += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(time / slideDuration);
            float eased = EaseInOut(t);

            contentRoot.anchoredPosition = Vector2.Lerp(startPos, exitPos, eased);

            if (contentCanvasGroup != null)
                contentCanvasGroup.alpha = Mathf.Lerp(1f, 0f, eased);

            yield return null;
        }

        currentPage = targetPage;
        RefreshUI();

        contentRoot.anchoredPosition = enterPos;
        if (contentCanvasGroup != null)
            contentCanvasGroup.alpha = 0f;

        time = 0f;

        while (time < slideDuration)
        {
            time += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(time / slideDuration);
            float eased = EaseInOut(t);

            contentRoot.anchoredPosition = Vector2.Lerp(enterPos, Vector2.zero, eased);

            if (contentCanvasGroup != null)
                contentCanvasGroup.alpha = Mathf.Lerp(0f, 1f, eased);

            yield return null;
        }

        contentRoot.anchoredPosition = Vector2.zero;

        if (contentCanvasGroup != null)
            contentCanvasGroup.alpha = 1f;

        SetButtonsInteractable(true);
        isAnimating = false;
    }

    private void RefreshUI()
    {
        if (pages == null || pages.Count == 0) return;

        currentPage = Mathf.Clamp(currentPage, 0, pages.Count - 1);

        var page = pages[currentPage];

        if (titleText != null)
            titleText.text = page.title;

        if (descriptionText != null)
            descriptionText.text = page.description;

        bool isFirstPage = currentPage == 0;
        bool isLastPage = currentPage == pages.Count - 1;
        bool onlyOnePage = pages.Count <= 1;

        if (prevButton != null)
            prevButton.gameObject.SetActive(!isFirstPage && !onlyOnePage);

        if (nextButton != null)
            nextButton.gameObject.SetActive(!isLastPage && !onlyOnePage);

        if (closeButton != null)
            closeButton.gameObject.SetActive(isLastPage || onlyOnePage);

        UpdateDots();
    }

    private void UpdateDots()
    {
        for (int i = 0; i < dots.Count; i++)
        {
            if (dots[i] != null)
                dots[i].color = (i == currentPage) ? activeDotColor : inactiveDotColor;
        }
    }

    private void SetButtonsInteractable(bool value)
    {
        if (closeButton != null)
            closeButton.interactable = value;

        if (prevButton != null)
            prevButton.interactable = value && currentPage > 0;

        if (nextButton != null)
            nextButton.interactable = value && currentPage < pages.Count - 1;
    }

    private IEnumerator FadeInRoutine()
    {
        isAnimating = true;

        float time = 0f;

        while (time < fadeDuration)
        {
            time += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(time / fadeDuration);
            float eased = EaseInOut(t);

            if (popupCanvasGroup != null)
                popupCanvasGroup.alpha = eased;

            if (backgroundCanvasGroup != null)
                backgroundCanvasGroup.alpha = eased;

            yield return null;
        }

        if (popupCanvasGroup != null)
        {
            popupCanvasGroup.interactable = true;
            popupCanvasGroup.blocksRaycasts = true;
        }

        if (backgroundCanvasGroup != null)
        {
            backgroundCanvasGroup.interactable = true;
            backgroundCanvasGroup.blocksRaycasts = true;
        }

        isAnimating = false;
    }

    private IEnumerator FadeOutRoutine()
    {
        isAnimating = true;
        SetButtonsInteractable(false);

        float startPopupAlpha = popupCanvasGroup != null ? popupCanvasGroup.alpha : 1f;
        float startBgAlpha = backgroundCanvasGroup != null ? backgroundCanvasGroup.alpha : 1f;

        float time = 0f;

        while (time < fadeDuration)
        {
            time += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(time / fadeDuration);
            float eased = EaseInOut(t);

            if (popupCanvasGroup != null)
                popupCanvasGroup.alpha = Mathf.Lerp(startPopupAlpha, 0f, eased);

            if (backgroundCanvasGroup != null)
                backgroundCanvasGroup.alpha = Mathf.Lerp(startBgAlpha, 0f, eased);

            yield return null;
        }

        if (popupPanel != null)
            popupPanel.SetActive(false);

        if (backgroundOverlay != null)
            backgroundOverlay.SetActive(false);

        ResumeGame();
        isAnimating = false;
    }

    private float EaseInOut(float t)
    {
        return t * t * (3f - 2f * t);
    }

    private void PauseGame()
    {
        if (isShowing) return;

        previousTimeScale = Time.timeScale;
        Time.timeScale = 0f;
        isShowing = true;
    }

    private void ResumeGame()
    {
        if (!isShowing) return;

        Time.timeScale = previousTimeScale;
        isShowing = false;
    }

    private void OnDestroy()
    {
        if (isShowing)
            Time.timeScale = previousTimeScale;
    }
}