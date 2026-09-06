using UnityEngine;

public class LevelComplete : MonoBehaviour
{
    public int currentLevel;

    public void UnlockNextLevel()
    {
        int unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 1);

        if (currentLevel >= unlockedLevel)
        {
            PlayerPrefs.SetInt("UnlockedLevel", currentLevel + 1);
        }
    }
}
