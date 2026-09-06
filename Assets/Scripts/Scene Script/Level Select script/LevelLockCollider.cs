using UnityEngine;
using UnityEngine.EventSystems;

public class LevelLockCollider : MonoBehaviour, IPointerClickHandler
{
    public int levelNumber;

    void Start()
    {
        int unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 1);

        Debug.Log("LevelLockCollider START | Level: " + levelNumber +
                  " | UnlockedLevel: " + unlockedLevel);

        if (levelNumber <= unlockedLevel)
        {
            Debug.Log("Level " + levelNumber + " UNLOCKED → Destroying collider");
            Destroy(gameObject);
        }
        else
        {
            Debug.Log("Level " + levelNumber + " is LOCKED → Collider active");
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("CLICK DETECTED on Level " + levelNumber);

        int unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 1);

        if (levelNumber > unlockedLevel)
        {
            Debug.Log("Level is LOCKED → Showing toast");

            if (ToastMessage.Instance != null)
                ToastMessage.Instance.ShowToast("Level locked \nFinish the previous level first.", 2f);
        }
        else
        {
            Debug.Log("Level is already UNLOCKED (this should not happen if collider exists)");
        }
    }
}