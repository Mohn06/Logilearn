using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelButton : MonoBehaviour
{
    public int levelNumber;      // Set this per button
    public Button button;

    void Start()
    {
        int unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 1);

        if (levelNumber <= unlockedLevel)
            button.interactable = true;
        else
            button.interactable = false;
    }

    public void LoadLevel()
    {
        SceneManager.LoadScene("Level " + levelNumber);
    }
}
