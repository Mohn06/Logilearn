using UnityEngine;

public class TutorialTrigger2D : MonoBehaviour
{
    public string tutorialID;
    public bool destroyAfterTrigger = true;

    private bool triggered = false;
    public GameObject tutorialPanel;

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
            tutorialPanel.SetActive(true);

            Debug.Log("Player triggered tutorial: " + tutorialID);

            TutorialManager.Instance.Unlock(tutorialID);

            var journalUI = FindFirstObjectByType<TutorialJournalUI>();

            if (journalUI != null)
                journalUI.RefreshJournal();

            if (destroyAfterTrigger)
                Destroy(gameObject);
        }
    }
}