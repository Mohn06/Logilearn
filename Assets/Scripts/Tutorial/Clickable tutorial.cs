using UnityEngine;
using UnityEngine.InputSystem;

public class TutorialTapTrigger2D : MonoBehaviour
{
    public string tutorialID;
    public bool destroyAfterTrigger = true;

    public GameObject tutorialPanel; // UI popup panel

    private bool triggered = false;

    void Start()
    {
        if (TutorialManager.Instance.IsUnlocked(tutorialID))
            Destroy(gameObject);
    }

    void Update()
    {
        if (triggered) return;

        // Mobile Touch
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
        {
            Vector2 touchPos = Camera.main.ScreenToWorldPoint(
                Touchscreen.current.primaryTouch.position.ReadValue());

            CheckTap(touchPos);
        }

        // Mouse Click (for Unity editor testing)
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(
                Mouse.current.position.ReadValue());

            CheckTap(mousePos);
        }
    }

    void CheckTap(Vector2 position)
    {
        RaycastHit2D hit = Physics2D.Raycast(position, Vector2.zero);

        if (hit.collider != null && hit.collider.gameObject == gameObject)
        {
            TriggerTutorial();
        }
    }

    void TriggerTutorial()
    {
        triggered = true;

        Debug.Log("Tutorial triggered: " + tutorialID);

        // Show tutorial popup
        if (tutorialPanel != null)
            tutorialPanel.SetActive(true);

        // Unlock tutorial
        TutorialManager.Instance.Unlock(tutorialID);

        var journalUI = FindFirstObjectByType<TutorialJournalUI>();

        if (journalUI != null)
            journalUI.RefreshJournal();

        if (destroyAfterTrigger)
            Destroy(gameObject);
    }
}