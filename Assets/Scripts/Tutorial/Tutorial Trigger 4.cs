using UnityEngine;

public class TutorialTrigger42D : MonoBehaviour
{
    public string tutorialID;
    public bool destroyAfterTrigger = true;

    private bool triggered = false;

    public TutorialPopupTextOnly tutorialPopup;

    private void Start()
    {
        if (TutorialManager.Instance.IsUnlocked(tutorialID))
            Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered) return;

        if (other.CompareTag("Player"))
        {
            triggered = true;

            Debug.Log("Player triggered tutorial: " + tutorialID);

            if (tutorialPopup != null)
            {
                tutorialPopup.ShowTutorial();
            }
            else
            {
                Debug.LogError("TutorialPopupUI reference is missing!");
            }

            TutorialManager.Instance.Unlock(tutorialID);

            var journalUI = FindFirstObjectByType<TutorialJournalUI>();
            if (journalUI != null)
                journalUI.RefreshJournal();

            if (destroyAfterTrigger)
                Destroy(gameObject);
        }
    }
}