using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialJournalUI : MonoBehaviour
{
    private PlayerController playerController;

    public GameObject journalPanel;
    public Transform buttonParent;
    public GameObject buttonPrefab;
    public GameObject journalNewIndicator;

    public Image tutorialImage;
    public TMP_Text titleText;
    public TMP_Text descriptionText;

    public Button nextButton;
    public Button prevButton;
    public GameObject controlUI;

    private TutorialSO currentTutorial;
    private int currentPage = 0;

    public AudioClip journalbuttonSFX;
    public AudioClip closejournalSFX;
    public AudioClip navjournalSFX;
    public bool resetPlayerPrefsOnStart = false;
    public CanvasGroup journalCanvasGroup;
    public float animationDuration = 0.25f;

    public ScrollRect tutorialScrollRect;
    public float scrollSmoothTime = 8f;



    private void Start()
    {
        playerController = FindFirstObjectByType<PlayerController>();


        RefreshJournal();   // <-- important change
        journalPanel.SetActive(false);

        if (TutorialManager.Instance != null)
            TutorialManager.Instance.OnTutorialUnlocked -= OnTutorialUnlocked;
        TutorialManager.Instance.OnTutorialUnlocked += OnTutorialUnlocked;

        if (resetPlayerPrefsOnStart)
        {
            Debug.Log("⚠ Resetting all PlayerPrefs...");
            PlayerPrefs.DeleteAll();
        }

        Debug.Log("---- JOURNAL POPULATE START ----");
        if (buttonParent == null) return;


    }
    void PlaySFX()
    {
        AudioManager.Instance.PlaySFX(journalbuttonSFX);
    }
    void OnTutorialUnlocked()
    {
        UpdateJournalIndicator();

        if (journalPanel.activeSelf)
        {
            RefreshJournal();
        }
    }

    public void OpenJournal()
    {
        journalPanel.SetActive(true);
        StartCoroutine(AnimateJournal(0, 1));
        PlaySFX();

        if (TutorialManager.Instance != null)
            TutorialManager.Instance.SetJournalState(true);

        if (playerController != null)
            playerController.DisableInput();

        RefreshJournal();
        UpdateJournalIndicator();

        if (currentTutorial == null)
        {
            foreach (var tutorial in TutorialManager.Instance.allTutorials)
            {
                if (TutorialManager.Instance.IsUnlocked(tutorial.tutorialID))
                {
                    OpenTutorial(tutorial);
                    break;
                }
            }
        }
    }


    public void CloseJournal()
    {
        StartCoroutine(AnimateJournal(1, 0));
        AudioManager.Instance.PlaySFX(closejournalSFX);

        if (TutorialManager.Instance != null)
            TutorialManager.Instance.SetJournalState(false);

        if (playerController != null)
            playerController.EnableInput();
    }
    void ClearDisplay()

    {
        currentTutorial = null;
        currentPage = 0;

        if (titleText != null) titleText.text = string.Empty;
        if (descriptionText != null) descriptionText.text = string.Empty;
        if (tutorialImage != null) tutorialImage.sprite = null;

        if (prevButton != null) prevButton.interactable = false;
        if (nextButton != null) nextButton.interactable = false;
    }

    void PopulateList()
    {
        foreach (var tutorial in TutorialManager.Instance.allTutorials)
        {
            if (!TutorialManager.Instance.IsUnlocked(tutorial.tutorialID))
                continue;

            var capturedTutorial = tutorial;

            GameObject btn = Instantiate(buttonPrefab, buttonParent);

            // Get your prefab script
            TutorialButtonUI buttonUI = btn.GetComponent<TutorialButtonUI>();

            // Let prefab handle title + NEW indicator
            buttonUI.Setup(tutorial);

            Button button = btn.GetComponent<Button>();
            button.onClick.RemoveAllListeners();

            button.onClick.AddListener(() =>
            {
                OpenTutorial(capturedTutorial);

                // Mark as viewed
                TutorialManager.Instance.MarkAsViewed(capturedTutorial.tutorialID);

                // Hide NEW badge on this specific button
                buttonUI.HideNew();

                UpdateJournalIndicator();
            });
        }
    }


    void OnDestroy()
    {
        if (TutorialManager.Instance != null)
            TutorialManager.Instance.OnTutorialUnlocked -= OnTutorialUnlocked;
    }


    public void RefreshJournal()
    {
        // Destroy old buttons
        foreach (Transform child in buttonParent)
        {
            Destroy(child.gameObject);
        }

        PopulateList();

        UpdateJournalIndicator();

        // ✅ If journal is open and nothing selected, select first unlocked tutorial
        if (journalPanel.activeSelf && currentTutorial == null)
        {
            foreach (var tutorial in TutorialManager.Instance.allTutorials)
            {
                if (TutorialManager.Instance.IsUnlocked(tutorial.tutorialID))
                {
                    OpenTutorial(tutorial);
                    break;
                }
            }
        }
    }
    void UpdateJournalIndicator()
    {

        if (journalNewIndicator == null) return;

        foreach (var tutorial in TutorialManager.Instance.allTutorials)
        {
            if (TutorialManager.Instance.IsUnlocked(tutorial.tutorialID)
                && TutorialManager.Instance.IsNew(tutorial.tutorialID))
            {
                journalNewIndicator.SetActive(true);
                return;
            }
        }

        journalNewIndicator.SetActive(false);
    }
    public void OpenTutorial(TutorialSO tutorial)
    {
        currentTutorial = tutorial;
        currentPage = 0;

        // Mark tutorial as viewed permanently
        if (TutorialManager.Instance != null)
            TutorialManager.Instance.MarkAsViewed(tutorial.tutorialID);

        UpdateJournalIndicator();
        UpdateDisplay();

        foreach (Transform child in buttonParent)
        {
            TutorialButtonUI btn = child.GetComponent<TutorialButtonUI>();

            if (btn != null && btn.titleText.text == tutorial.title)
            {
                btn.SelectButton();
                btn.HideNew();

                StartCoroutine(SmoothScrollToButton(btn.GetComponent<RectTransform>()));

                break;
            }
        }
    }



    void UpdateDisplay()
    {
        if (currentTutorial == null) return;

        if (currentTutorial.pages == null || currentTutorial.pages.Length == 0)
            return;

        var page = currentTutorial.pages[currentPage];

        titleText.text = currentTutorial.title;

        if (page.image != null)
            tutorialImage.sprite = page.image;
        else
            tutorialImage.sprite = null;

        descriptionText.text = page.description;

        // Build list of unlocked tutorials
        List<TutorialSO> unlockedTutorials = new List<TutorialSO>();

        foreach (var tutorial in TutorialManager.Instance.allTutorials)
        {
            if (TutorialManager.Instance.IsUnlocked(tutorial.tutorialID))
                unlockedTutorials.Add(tutorial);
        }

        int tutorialIndex = unlockedTutorials.IndexOf(currentTutorial);

        bool hasPrevPage = currentPage > 0;
        bool hasNextPage = currentPage < currentTutorial.pages.Length - 1;

        bool hasPrevTutorial = tutorialIndex > 0;
        bool hasNextTutorial = tutorialIndex < unlockedTutorials.Count - 1;

        prevButton.interactable = hasPrevPage || hasPrevTutorial;
        nextButton.interactable = hasNextPage || hasNextTutorial;
    }
    void ScrollToButton(RectTransform target)
    {
        Canvas.ForceUpdateCanvases();

        RectTransform content = tutorialScrollRect.content;
        RectTransform viewport = tutorialScrollRect.viewport;

        Vector2 contentPos = (Vector2)viewport.InverseTransformPoint(content.position);
        Vector2 targetPos = (Vector2)viewport.InverseTransformPoint(target.position);

        float difference = contentPos.y - targetPos.y;

        Vector2 newPos = content.anchoredPosition;
        newPos.y += difference;

        content.anchoredPosition = newPos;
    }
    public void NextPage()
    {
        if (currentTutorial == null) return;
        AudioManager.Instance.PlaySFX(navjournalSFX);
        // If there is another page → go to next page
        if (currentPage < currentTutorial.pages.Length - 1)
        {
            currentPage++;
            UpdateDisplay();
            return;
        }

        // Otherwise → go to next tutorial
        List<TutorialSO> unlockedTutorials = new List<TutorialSO>();

        foreach (var tutorial in TutorialManager.Instance.allTutorials)
        {
            if (TutorialManager.Instance.IsUnlocked(tutorial.tutorialID))
                unlockedTutorials.Add(tutorial);
        }

        int currentIndex = unlockedTutorials.IndexOf(currentTutorial);

        if (currentIndex < unlockedTutorials.Count - 1)
        {
            OpenTutorial(unlockedTutorials[currentIndex + 1]);
        }
    }

    public void PrevPage()
    {
        if (currentTutorial == null) return;
        AudioManager.Instance.PlaySFX(navjournalSFX);
        // If there is a previous page → go back
        if (currentPage > 0)
        {
            currentPage--;
            UpdateDisplay();
            return;
        }

        // Otherwise → go to previous tutorial
        List<TutorialSO> unlockedTutorials = new List<TutorialSO>();

        foreach (var tutorial in TutorialManager.Instance.allTutorials)
        {
            if (TutorialManager.Instance.IsUnlocked(tutorial.tutorialID))
                unlockedTutorials.Add(tutorial);
        }

        int currentIndex = unlockedTutorials.IndexOf(currentTutorial);

        if (currentIndex > 0)
        {
            OpenTutorial(unlockedTutorials[currentIndex - 1]);
            currentPage = currentTutorial.pages.Length - 1;
            UpdateDisplay();
        }
    }

    IEnumerator AnimateJournal(float startAlpha, float endAlpha)
    {
        float time = 0;

        Vector3 startScale = (startAlpha == 0) ? Vector3.one * 0.8f : Vector3.one;
        Vector3 endScale = (endAlpha == 0) ? Vector3.one * 0.8f : Vector3.one;

        if (endAlpha == 1)
        {
            journalCanvasGroup.interactable = true;
            journalCanvasGroup.blocksRaycasts = true;
        }

        while (time < animationDuration)
        {
            time += Time.unscaledDeltaTime;
            float t = time / animationDuration;

            journalCanvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, t);
            journalPanel.transform.localScale = Vector3.Lerp(startScale, endScale, t);

            yield return null;
        }

        if (endAlpha == 0)
        {
            journalCanvasGroup.interactable = false;
            journalCanvasGroup.blocksRaycasts = false;
            journalPanel.SetActive(false);
            ClearDisplay();
        }
    }
    IEnumerator SmoothScrollToButton(RectTransform target)
    {
        Canvas.ForceUpdateCanvases();

        RectTransform content = tutorialScrollRect.content;
        RectTransform viewport = tutorialScrollRect.viewport;

        Vector3[] viewportCorners = new Vector3[4];
        Vector3[] targetCorners = new Vector3[4];

        viewport.GetWorldCorners(viewportCorners);
        target.GetWorldCorners(targetCorners);

        float viewportTop = viewportCorners[1].y;
        float viewportBottom = viewportCorners[0].y;

        float targetTop = targetCorners[1].y;
        float targetBottom = targetCorners[0].y;

        float padding = 40f; // extra space so button is fully visible
        float offset = 0f;

        if (targetTop > viewportTop)
            offset = targetTop - viewportTop + padding;

        else if (targetBottom < viewportBottom)
            offset = targetBottom - viewportBottom - padding;

        if (Mathf.Abs(offset) < 0.01f)
            yield break;

        Vector2 startPos = content.anchoredPosition;
        Vector2 targetPos = startPos - new Vector2(0, offset);

        float time = 0f;

        while (time < 1f)
        {
            time += Time.unscaledDeltaTime * scrollSmoothTime;
            content.anchoredPosition = Vector2.Lerp(startPos, targetPos, time);
            yield return null;
        }

        content.anchoredPosition = targetPos;
    }
}