using System;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance;

    public List<TutorialSO> allTutorials;

    private HashSet<string> unlocked = new HashSet<string>();

    public event Action OnTutorialUnlocked;

    const string UNLOCK_KEY_PREFIX = "TUT_";
    const string NEW_KEY_PREFIX = "TutorialNew_";

    public bool IsJournalOpen { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadUnlocked();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 🔓 Unlock tutorial
    public void Unlock(string tutorialID)
    {
        if (IsUnlocked(tutorialID))
            return;

        PlayerPrefs.SetInt("TUT_" + tutorialID, 1);
        PlayerPrefs.SetInt("TutorialNew_" + tutorialID, 1);

        PlayerPrefs.Save();

        unlocked.Add(tutorialID);

        Debug.Log("Unlocked tutorial: " + tutorialID);

        OnTutorialUnlocked?.Invoke(); // MUST always fire
    }


    public bool IsUnlocked(string id)
    {
        return unlocked.Contains(id);
    }

    public bool IsNew(string tutorialID)
    {
        return PlayerPrefs.GetInt(NEW_KEY_PREFIX + tutorialID, 0) == 1;
    }

    public void MarkAsViewed(string tutorialID)
    {
        Debug.Log("Marking as viewed: " + tutorialID);
        PlayerPrefs.SetInt(NEW_KEY_PREFIX + tutorialID, 0);
        PlayerPrefs.Save();
    }

    public void SetJournalState(bool state)
    {
        IsJournalOpen = state;
    }

    public bool HasAnyUnlocked()
    {
        return unlocked.Count > 0;
    }

    private void LoadUnlocked()
    {
        foreach (var t in allTutorials)
        {
            if (PlayerPrefs.GetInt(UNLOCK_KEY_PREFIX + t.tutorialID, 0) == 1)
            {
                unlocked.Add(t.tutorialID);
            }
        }
    }
}
