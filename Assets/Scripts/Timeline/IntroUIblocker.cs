using UnityEngine;

public class IntroUIBlocker : MonoBehaviour
{
    public CanvasGroup introCanvas;
    public GameObject tutorialPanel; // optional

    public void OnIntroFinished()
    {
        introCanvas.blocksRaycasts = false;
        introCanvas.interactable = false;

        // Show tutorial after intro
        if (tutorialPanel != null)
        {
            tutorialPanel.SetActive(true);
            Time.timeScale = 0f;
        }
    }
}
