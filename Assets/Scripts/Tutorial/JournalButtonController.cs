using UnityEngine;

public class JournalButtonController : MonoBehaviour
{
    private void Start()
    {
        gameObject.SetActive(TutorialManager.Instance.HasAnyUnlocked());

        TutorialManager.Instance.OnTutorialUnlocked += CheckVisibility;
    }

    void CheckVisibility()
    {
        gameObject.SetActive(true);
    }

    private void OnDestroy()
    {
        if (TutorialManager.Instance != null)
            TutorialManager.Instance.OnTutorialUnlocked -= CheckVisibility;
    }
}