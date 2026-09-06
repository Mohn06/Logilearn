using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TutorialSlideshow : MonoBehaviour
{
    [System.Serializable]
    public class Slide
    {
        public Sprite image;      // Tutorial image
        public string description; // Tutorial text
    }

    public Slide[] slides;

    [Header("UI References")]
    public Image tutorialImage;
    public TextMeshProUGUI descriptionText;
    public GameObject tutorialPanel;

    private int currentSlide = 0;

    void Start()
    {
        StartTutorial();
    }

    public void StartTutorial()
    {
        tutorialPanel.SetActive(true);
        Time.timeScale = 0f; // pause game
        ShowSlide(0);
    }

    void ShowSlide(int index)
    {
        currentSlide = index;

        tutorialImage.sprite = slides[index].image;
        descriptionText.text = slides[index].description;
    }

    public void NextStep()
    {
        currentSlide++;

        if (currentSlide >= slides.Length)
        {
            EndTutorial();
            return;
        }

        ShowSlide(currentSlide);
    }

    void EndTutorial()
    {
        tutorialPanel.SetActive(false);
        Time.timeScale = 1f; // resume game
    }
}
